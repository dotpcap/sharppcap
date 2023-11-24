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
    public class CaptureFileWriterDevice : PcapDevice, IInjectionDevice
    {
        private readonly string m_pcapFile;
        private readonly FileMode fileMode;

        /// <summary>
        /// Handle to an open dump file, not equal to IntPtr.Zero if a dump file is open
        /// </summary>
        protected IntPtr m_pcapDumpHandle = IntPtr.Zero;

        /// <summary>
        /// Whether dump file is open or not
        /// </summary>
        /// <returns>
        /// A <see cref="bool"/>
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
        public CaptureFileWriterDevice(string captureFilename, System.IO.FileMode mode = FileMode.OpenOrCreate)
        {
            m_pcapFile = captureFilename;
            fileMode = mode;

            if (mode == FileMode.Append && Pcap.LibpcapVersion < new Version(1, 7, 2))
            {
                throw new PlatformNotSupportedException("FileMode.Append is not supported");
            }
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
        public override void Open(DeviceConfiguration configuration)
        {
            // set the device handle
            var has_open_dead_with_tstamp_precision_support = Pcap.LibpcapVersion >= new Version(1, 5, 1);
            var resolution = configuration.TimestampResolution ?? TimestampResolution.Microsecond;
            if (has_open_dead_with_tstamp_precision_support)
            {
                Handle = LibPcapSafeNativeMethods.pcap_open_dead_with_tstamp_precision((int)configuration.LinkLayerType,
                    configuration.Snaplen,
                    (uint)resolution);
            }
            else
            {
                if (resolution != TimestampResolution.Microsecond)
                {
                    configuration.RaiseConfigurationFailed(
                        nameof(configuration.TimestampResolution),
                        PcapError.PlatformNotSupported,
                        "pcap version is < 1.5.1, needs pcap_open_dead_with_tstamp_precision()"
                    );
                }

                Handle = LibPcapSafeNativeMethods.pcap_open_dead((int)configuration.LinkLayerType, configuration.Snaplen);
            }

            if (fileMode == FileMode.Append)
            {
                m_pcapDumpHandle = LibPcapSafeNativeMethods.pcap_dump_open_append(Handle, m_pcapFile);
            }
            else
            {
                m_pcapDumpHandle = LibPcapSafeNativeMethods.pcap_dump_open(Handle, m_pcapFile);
            }

            if (m_pcapDumpHandle == IntPtr.Zero)
                throw new PcapException("Error opening dump file '" + LastError + "'");

            base.Open(configuration);
        }

        /// <summary>
        /// Retrieves pcap statistics
        ///
        /// Not currently supported for this device
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public override ICaptureStatistics Statistics => null;

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        /// <param name="p">P.</param>
        /// <param name="h">The height.</param>
        public void Write(ReadOnlySpan<byte> p, ref PcapHeader h)
        {
            ThrowIfNotOpen("Cannot dump packet, device is not opened");
            if (!DumpOpened)
                throw new DeviceNotReadyException("Cannot dump packet, dump file is not opened");

            //Marshal header
            IntPtr hdrPtr = h.MarshalToIntPtr(TimestampResolution);

            unsafe
            {
                fixed (byte* p_packet = p)
                {
                    LibPcapSafeNativeMethods.pcap_dump(m_pcapDumpHandle, hdrPtr, new IntPtr(p_packet));
                }
            }

            Marshal.FreeHGlobal(hdrPtr);
        }

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        /// <param name="p">The packet to write</param>
        public void Write(ReadOnlySpan<byte> p)
        {
            var header = new PcapHeader(0, 0, (uint)p.Length, (uint)p.Length);
            Write(p, ref header);
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
            Write(data, ref header);
        }

        void IInjectionDevice.SendPacket(ReadOnlySpan<byte> p, ICaptureHeader header)
        {
            Write(p);
        }
    }
}
