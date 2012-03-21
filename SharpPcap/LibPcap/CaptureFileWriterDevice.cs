/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Copyright 2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Create or write to a pcap capture file
    ///
    /// NOTE: Appending to a capture file is not currently supported
    /// </summary>
    public class CaptureFileWriterDevice : PcapDevice
    {
        private string m_pcapFile;

        /// <summary>
        /// Handle to an open dump file, not equal to IntPtr.Zero if a dump file is open
        /// </summary>
        protected IntPtr       m_pcapDumpHandle    = IntPtr.Zero;

        /// <summary>
        /// Whether dump file is open or not
        /// </summary>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        protected bool DumpOpened
        {
            get
            {
                return (m_pcapDumpHandle != IntPtr.Zero);
            }
        }

        /// <value>
        /// The name of the capture file
        /// </value>
        public override string Name
        {
            get
            {
                return m_pcapFile;
            }
        }

        /// <value>
        /// Description of the device
        /// </value>
        public override string Description
        {
            get
            {
                return "Capture file reader device";
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="captureFilename">
        /// A <see cref="System.String"/>
        /// </param>
        public CaptureFileWriterDevice (string captureFilename) : this(captureFilename, FileMode.OpenOrCreate)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="captureFilename">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="mode">
        /// A <see cref="FileMode"/>
        /// </param>
        public CaptureFileWriterDevice(string captureFilename, FileMode mode) :
            this(PacketDotNet.LinkLayers.Ethernet, Pcap.MAX_PACKET_SIZE,
                 captureFilename, mode)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device">
        /// A <see cref="LibPcapLiveDevice"/>
        /// </param>
        /// <param name="captureFilename">
        /// A <see cref="System.String"/>
        /// </param>
        public CaptureFileWriterDevice(LibPcapLiveDevice device,
                                       string captureFilename) :
            this((PacketDotNet.LinkLayers)LibPcapSafeNativeMethods.pcap_datalink(device.PcapHandle),
                 LibPcapSafeNativeMethods.pcap_snapshot(device.PcapHandle),
                 captureFilename,
                 FileMode.OpenOrCreate)

        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device">
        /// A <see cref="LibPcapLiveDevice"/>
        /// </param>
        /// <param name="captureFilename">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="mode">
        /// A <see cref="FileMode"/>
        /// </param>
        public CaptureFileWriterDevice(LibPcapLiveDevice device,
                                       string captureFilename,
                                       FileMode mode) :
            this((PacketDotNet.LinkLayers)LibPcapSafeNativeMethods.pcap_datalink(device.PcapHandle),
                 LibPcapSafeNativeMethods.pcap_snapshot(device.PcapHandle),
                 captureFilename,
                 mode)

        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="linkLayerType">
        /// A <see cref="PacketDotNet.LinkLayers"/>
        /// </param>
        /// <param name="snapshotLength">
        /// A <see cref="System.Nullable&lt;System.Int32&gt;"/>
        /// </param>
        /// <param name="captureFilename">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="mode">
        /// A <see cref="FileMode"/>
        /// </param>
        public CaptureFileWriterDevice(PacketDotNet.LinkLayers linkLayerType,
                                       int? snapshotLength,
                                       string captureFilename,
                                       FileMode mode)
        {
            m_pcapFile = captureFilename;

            // append isn't possible without some difficulty and not implemented yet
            if(mode == FileMode.Append)
            {
                throw new System.InvalidOperationException("FileMode.Append is not supported, please contact the developers if you are interested in helping to implementing it");
            }

            if(!snapshotLength.HasValue)
            {
                snapshotLength = Pcap.MAX_PACKET_SIZE;
            } else if(snapshotLength > Pcap.MAX_PACKET_SIZE)
            {
                throw new System.InvalidOperationException("snapshotLength > Pcap.MAX_PACKET_SIZE");
            }

            // set the device handle
            PcapHandle = LibPcapSafeNativeMethods.pcap_open_dead((int)linkLayerType, snapshotLength.Value);

            m_pcapDumpHandle = LibPcapSafeNativeMethods.pcap_dump_open(PcapHandle, captureFilename);
            if(m_pcapDumpHandle == IntPtr.Zero)
                throw new PcapException("Error opening dump file '" + LastError + "'");
        }

        /// <summary>
        /// Close the capture file
        /// </summary>
        public override void Close()
        {
            if (!Opened)
                return;

            base.Close();

            // close the dump handle
            if (m_pcapDumpHandle != IntPtr.Zero)
            {
                LibPcapSafeNativeMethods.pcap_dump_close(m_pcapDumpHandle);
                m_pcapDumpHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Open the device
        /// </summary>
        public override void Open()
        {
            // Nothing to do here, device is already opened upon construction
        }

        /// <summary>
        /// Retrieves pcap statistics
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public override ICaptureStatistics Statistics
        {
            get
            {
                throw new NotSupportedOnCaptureFileException("Statistics not supported on a capture file");
            }
        }

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        public void Write(byte[] p, PcapHeader h)
        {
            ThrowIfNotOpen("Cannot dump packet, device is not opened");
            if(!DumpOpened)
                throw new DeviceNotReadyException("Cannot dump packet, dump file is not opened");

            //Marshal packet
            IntPtr pktPtr;
            pktPtr = Marshal.AllocHGlobal(p.Length);
            Marshal.Copy(p, 0, pktPtr, p.Length);

            //Marshal header
            IntPtr hdrPtr = h.MarshalToIntPtr();

            LibPcapSafeNativeMethods.pcap_dump(m_pcapDumpHandle, hdrPtr, pktPtr);

            Marshal.FreeHGlobal(pktPtr);
            Marshal.FreeHGlobal(hdrPtr);
        }

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        /// <param name="p">The packet to write</param>
        public void Write(byte[] p)
        {
            Write(p, new PcapHeader(0, 0, (uint)p.Length, (uint)p.Length));
        }

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        /// <param name="p">The packet to write</param>
        public void Write(RawCapture p)
        {
            var data = p.Data;
            var timeval = p.Timeval;
            var header = new PcapHeader((uint)timeval.Seconds, (uint)timeval.MicroSeconds,
                                        (uint)data.Length, (uint)data.Length);
            Write(data, header);
        }
    }
}
