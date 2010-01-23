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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Runtime.InteropServices;
using SharpPcap.Packets;

namespace SharpPcap
{
    /// <summary>
    /// Base class for all pcap devices
    /// </summary>
    public abstract partial class PcapDevice
    {
        protected PcapInterface m_pcapIf;

        protected IntPtr       m_pcapDumpHandle    = IntPtr.Zero;
        protected IntPtr       m_pcapAdapterHandle = IntPtr.Zero;
        protected int          m_pcapPacketCount   = Pcap.INFINITE;
        private CaptureMode    m_pcapMode          = CaptureMode.Packets;

        /// <summary>
        /// Fires whenever a new packet is processed, either when the packet arrives
        /// from the network device or when the packet is read from the on-disk file.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Capture" mode.
        /// </summary>
        public event PacketArrivalEventHandler OnPacketArrival;

        /// <summary>
        /// Fires whenever a new pcap statistics is available for this Pcap Device.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Statistics" mode.
        /// </summary>
        public event StatisticsModeEventHandler OnPcapStatistics;

        /// <summary>
        /// Fired when the capture process of this pcap device is stopped
        /// </summary>
        public event CaptureStoppedEventHandler OnCaptureStopped;

        /// <value>
        /// Low level pcap device values
        /// </value>
        public PcapInterface Interface
        {
            get { return m_pcapIf; }
        }

        /// <summary>
        /// Return a value indicating if this adapter is opened
        /// </summary>
        public virtual bool Opened
        {
            get{ return (PcapHandle != IntPtr.Zero); }
        }

        /// <summary>
        /// Gets a value indicating wether pcap dump file is already associated with this device
        /// </summary>
        public virtual bool DumpOpened
        {
            get { return m_pcapDumpHandle!=IntPtr.Zero; }
        }

        /// <summary>
        /// Gets the name of the device
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <value>
        /// Description of the device
        /// </value>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// Return the pcap link layer value of an adapter. 
        /// </summary>
        public virtual LinkLayers PcapDataLink
        {
            get
            {
                ThrowIfNotOpen("Cannot get datalink, the pcap device is not opened");
                return (LinkLayers)SafeNativeMethods.pcap_datalink(PcapHandle);
            }
        }

        /// <value>
        /// WinPcap specific property
        /// </value>
        public virtual CaptureMode Mode
        {
            get
            {
                return m_pcapMode;
            }

            set
            {
                ThrowIfNotWinPcap();
                ThrowIfNotOpen("Mode");

                m_pcapMode = value;
                int result = SafeNativeMethods.pcap_setmode(this.PcapHandle , (int)m_pcapMode);
                if (result < 0)
                    throw new PcapException("Error setting PcapDevice mode. : " + LastError);
            }
        }

        /// <summary>
        /// The underlying pcap device handle
        /// </summary>
        internal virtual IntPtr PcapHandle
        {
            get { return m_pcapAdapterHandle; }
            set { m_pcapAdapterHandle = value; }
        }

        /// <summary>
        /// Retrieve the last error string for a given pcap_t* device
        /// </summary>
        /// <param name="deviceHandle">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        internal static string GetLastError(IntPtr deviceHandle)
        {
            IntPtr err_ptr = SafeNativeMethods.pcap_geterr(deviceHandle);
            return Marshal.PtrToStringAnsi(err_ptr);
        }

        /// <summary>
        /// The last pcap error associated with this pcap device
        /// </summary>
        public string LastError
        {
            get { return GetLastError(PcapHandle);  }
        }

        /// <summary>
        /// Open the device with class specific options
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Closes this adapter
        /// </summary>
        public virtual void Close()
        {
            if(PcapHandle==IntPtr.Zero)
                return;

            if (Started)
            {
                StopCapture();
            }
            SafeNativeMethods.pcap_close(PcapHandle);
            PcapHandle = IntPtr.Zero;

            //Remove event handlers
            if ( OnPacketArrival != null)
            {
                foreach(PacketArrivalEventHandler pa in OnPacketArrival.GetInvocationList())
                {
                    OnPacketArrival -= pa;
                }
            }
            if ( OnPcapStatistics != null)
            {
                foreach(StatisticsModeEventHandler pse in OnPcapStatistics.GetInvocationList())
                {
                    OnPcapStatistics -= pse;
                }
            }
        }

        /// <summary>
        /// Notify the OnPacketArrival delegates about a newly captured packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="SharpPcap.Packets.Packet"/>
        /// </param>
        private void SendPacketArrivalEvent(Packet p)
        {
            if(Mode == CaptureMode.Packets)
            {
                var handler = OnPacketArrival;
                if(handler != null )
                {
                    //Invoke the packet arrival event
                    handler(this, new CaptureEventArgs(p, (LivePcapDevice)this));
                }
            }
            else if(Mode == CaptureMode.Statistics)
            {
                var handler = OnPcapStatistics;
                if(handler != null)
                {
                    //Invoke the pcap statistics event
                    handler(this, new StatisticsModeEventArgs(p, (LivePcapDevice)this));
                }
            }
        }

        /// <summary>
        /// Notify the delegates that are subscribed to the capture stopped event
        /// </summary>
        /// <param name="error">
        /// A <see cref="System.Boolean"/>
        /// </param>
        private void SendCaptureStoppedEvent(bool error)
        {
            var handler = OnCaptureStopped;
            if(handler != null)
            {
                handler(this, error);
            }
        }

        /// <summary>
        /// Gets the next packet captured on this device
        /// </summary>
        /// <returns>The next packet captured on this device</returns>
        public virtual Packet GetNextPacket()
        {
            Packet p;
            int res = GetNextPacket( out p );
            if(res==-1)
                throw new PcapException("Error receiving packet.");
            return p;
        }

        /// <summary>
        /// Gets the next packet captured on this device
        /// </summary>
        /// <param name="p">A packet reference</param>
        /// <returns>A reference to a packet object</returns>
        public virtual int GetNextPacket(out Packet p)
        {
            //Pointer to a packet info struct
            IntPtr header = IntPtr.Zero;

            //Pointer to a packet struct
            IntPtr data = IntPtr.Zero;
            int res = 0;

            // using an invalid PcapHandle can result in an unmanaged segfault
            // so check for that here
            ThrowIfNotOpen("Device must be opened via Open() prior to use");

            //Get a packet from winpcap
            res = SafeNativeMethods.pcap_next_ex( PcapHandle, ref header, ref data);
            p = null;

            if(res>0)
            {
                //Marshal the packet
                if ( (header != IntPtr.Zero) && (data != IntPtr.Zero) )
                {
                    p = MarshalPacket(header, data);
                }
            }
            return res;
        }

        /// <summary>
        /// Gets the next packet captured on this device
        /// </summary>
        /// <returns>The next packet captured on this device</returns>
        public virtual RawPacket GetNextRawPacket()
        {
            RawPacket p;
            int res = GetNextPacket( out p );
            if(res==-1)
                throw new PcapException("Error receiving packet.");
            return p;
        }

        /// <summary>
        /// Retrieve the next packet as a RawPacket. Method is temporary until
        /// packet parsing code is removed from SharpPcap when Packet.net is mature enough
        /// </summary>
        /// <param name="p">
        /// A <see cref="SharpPcap.Packets.RawPacket"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public virtual int GetNextPacket(out RawPacket p)
        {
            //Pointer to a packet info struct
            IntPtr header = IntPtr.Zero;
            //Pointer to a packet struct
            IntPtr data = IntPtr.Zero;
            int res = 0;

            // using an invalid PcapHandle can result in an unmanaged segfault
            // so check for that here
            ThrowIfNotOpen("Device must be opened via Open() prior to use");

            //Get a packet from winpcap
            res = SafeNativeMethods.pcap_next_ex( PcapHandle, ref header, ref data);
            p = null;

            if(res>0)
            {
                //Marshal the packet
                if ( (header != IntPtr.Zero) && (data != IntPtr.Zero) )
                {
                    p = MarshalRawPacket(header, data);
                }
            }
            return res;
        }

        /// <summary>
        /// Pcap_loop callback method.
        /// </summary>
        protected virtual void PacketHandler(IntPtr param, IntPtr /* pcap_pkthdr* */ header, IntPtr data)
        {
            Packet p = MarshalPacket(header, data);
            SendPacketArrivalEvent(p);
        }

        protected virtual Packet MarshalPacket(IntPtr /* pcap_pkthdr* */ header, IntPtr data)
        {
            Packet p;

            // marshal the header
            var pcapHeader = new PcapHeader(header);

            byte[] pkt_data = new byte[pcapHeader.CaptureLength];
            Marshal.Copy(data, pkt_data, 0, (int)pcapHeader.CaptureLength);

            p = Packets.PacketFactory.dataToPacket(PcapDataLink, pkt_data,
                                                   new Packets.Util.Timeval(pcapHeader.Seconds,
                                                                            pcapHeader.MicroSeconds));
            p.pcapHeader = pcapHeader;

            return p;
        }

        protected virtual RawPacket MarshalRawPacket(IntPtr /* pcap_pkthdr* */ header, IntPtr data)
        {
            RawPacket p;

            // marshal the header
            var pcapHeader = new PcapHeader(header);

            byte[] pkt_data = new byte[pcapHeader.CaptureLength];
            Marshal.Copy(data, pkt_data, 0, (int)pcapHeader.CaptureLength);

            p = new RawPacket(PcapDataLink,
                              new Packets.Util.Timeval(pcapHeader.Seconds,
                                                       pcapHeader.MicroSeconds),
                              pkt_data);

            return p;
        }

        #region Dump methods
        /// <summary>
        /// Opens a file for packet writings
        /// </summary>
        /// <param name="fileName"></param>
        public void DumpOpen(string fileName)
        {
            ThrowIfNotOpen("Dump requires an open device");

            if(DumpOpened)
            {
                throw new PcapException("A dump file is already opened");
            }
            m_pcapDumpHandle = SafeNativeMethods.pcap_dump_open(PcapHandle, fileName);
            if(!DumpOpened)
                throw new PcapException("Error openning dump file.");
        }

        /// <summary>
        /// Closes the opened dump file
        /// </summary>
        public void DumpClose()
        {
            if(DumpOpened)
            {
                SafeNativeMethods.pcap_dump_close(m_pcapDumpHandle);
                m_pcapDumpHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Flushes all write buffers of the opened dump file
        /// </summary>
        public void DumpFlush()
        {
            if (DumpOpened)
            {
                int result = SafeNativeMethods.pcap_dump_flush(m_pcapDumpHandle);
                if (result < 0)
                    throw new PcapException("Error writing buffer to dumpfile. " + LastError);
            }
        }

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        /// <param name="p">The packet to write</param>
        public void Dump(byte[] p, PcapHeader h)
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

            SafeNativeMethods.pcap_dump(m_pcapDumpHandle, hdrPtr, pktPtr);

            Marshal.FreeHGlobal(pktPtr);
            Marshal.FreeHGlobal(hdrPtr);
        }

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        /// <param name="p">The packet to write</param>
        public void Dump(byte[] p)
        {
            Dump(p, new PcapHeader(0, 0, (uint)p.Length, (uint)p.Length));
        }

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        /// <param name="p">The packet to write</param>
        public void Dump(Packet p)
        {
            Dump(p.Bytes, p.PcapHeader);
        }

        #endregion

        /// <summary>
        /// Helper method for ensuring we are running in winpcap. Throws
        /// a PcapWinPcapRequiredException() if not on a windows platform
        /// </summary>
        internal static void ThrowIfNotWinPcap()
        {
            if((Environment.OSVersion.Platform != PlatformID.Win32NT) &&
               (Environment.OSVersion.Platform != PlatformID.Win32Windows))
            {
                throw new WinPcapRequiredException("only supported in winpcap");
            }
        }

        /// <summary>
        /// Helper method for checking that the adapter is open, throws an
        /// exception with a string of ExceptionString if the device isn't open
        /// </summary>
        /// <param name="ExceptionString">
        /// A <see cref="System.String"/>
        /// </param>
        protected void ThrowIfNotOpen(string ExceptionString)
        {
            if(!Opened)
            {
                throw new DeviceNotReadyException(ExceptionString);
            }
        }

        /// <summary>
        /// Override the default ToString() implementation
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString ()
        {
            return "interface: " + m_pcapIf.ToString() + "\n";
        }
    }
}
