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

namespace SharpPcap
{
    /// <summary>
    /// Base class for all pcap devices
    /// </summary>
    public abstract partial class PcapDevice
    {
        /// <summary>
        /// Low level interface object that contains device specific information
        /// </summary>
        protected PcapInterface m_pcapIf;

        /// <summary>
        /// Handle to an open dump file, not equal to IntPtr.Zero if a dump file is open
        /// </summary>
        protected IntPtr       m_pcapDumpHandle    = IntPtr.Zero;

        /// <summary>
        /// Handle to a pcap adapter, not equal to IntPtr.Zero if an adapter is open
        /// </summary>
        protected IntPtr       m_pcapAdapterHandle = IntPtr.Zero;

        /// <summary>
        /// Number of packets that this adapter should capture
        /// </summary>
        protected int          m_pcapPacketCount   = Pcap.InfinitePacketCount;

        private CaptureMode    m_pcapMode          = CaptureMode.Packets;
        private int          m_mask  = 0; //for filter expression

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
        public virtual PacketDotNet.LinkLayers PcapDataLink
        {
            get
            {
                ThrowIfNotOpen("Cannot get datalink, the pcap device is not opened");
                return (PacketDotNet.LinkLayers)SafeNativeMethods.pcap_datalink(PcapHandle);
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
        /// Retrieves pcap statistics
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public abstract PcapStatistics Statistics();

        /// <summary>
        /// Notify the OnPacketArrival delegates about a newly captured packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="PacketDotNet.RawPacket"/>
        /// </param>
        private void SendPacketArrivalEvent(PacketDotNet.RawPacket p)
        {
            if(Mode == CaptureMode.Packets)
            {
                var handler = OnPacketArrival;
                if(handler != null )
                {
                    //Invoke the packet arrival event
                    handler(this, new CaptureEventArgs(p, this));
                }
            }
            else if(Mode == CaptureMode.Statistics)
            {
                var handler = OnPcapStatistics;
                if(handler != null)
                {
                    //Invoke the pcap statistics event
                    handler(this, new StatisticsModeEventArgs(p, this));
                }
            }
        }

        /// <summary>
        /// Notify the delegates that are subscribed to the capture stopped event
        /// </summary>
        /// <param name="status">
        /// A <see cref="CaptureStoppedEventStatus"/>
        /// </param>
        private void SendCaptureStoppedEvent(CaptureStoppedEventStatus status)
        {
            var handler = OnCaptureStopped;
            if(handler != null)
            {
                handler(this, status);
            }
        }

        /// <summary>
        /// Gets the next packet captured on this device
        /// </summary>
        /// <returns>The next packet captured on this device</returns>
        public virtual PacketDotNet.RawPacket GetNextPacket()
        {
            PacketDotNet.RawPacket p;
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
        public virtual int GetNextPacket(out PacketDotNet.RawPacket p)
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
            var p = MarshalRawPacket(header, data);
            SendPacketArrivalEvent(p);
        }

        /// <summary>
        /// Convert an unmanaged packet into a managed PacketDotNet.RawPacket
        /// </summary>
        /// <param name="header">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <param name="data">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="PacketDotNet.RawPacket"/>
        /// </returns>
        protected virtual PacketDotNet.RawPacket MarshalRawPacket(IntPtr /* pcap_pkthdr* */ header, IntPtr data)
        {
            PacketDotNet.RawPacket p;

            // marshal the header
            var pcapHeader = new PcapHeader(header);

            var pkt_data = new byte[pcapHeader.CaptureLength];
            Marshal.Copy(data, pkt_data, 0, (int)pcapHeader.CaptureLength);

            p = new PacketDotNet.RawPacket(PcapDataLink,
                              new PacketDotNet.PosixTimeval(pcapHeader.Seconds,
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
        public void Dump(PacketDotNet.RawPacket p)
        {
            var data = p.Data;
            var timeval = p.Timeval;
            var header = new PcapHeader(timeval.Seconds, timeval.MicroSeconds,
                                        (uint)data.Length, (uint)data.Length);
            Dump(data, header);
        }

        #endregion

        #region Filtering
        /// <summary>
        /// Deprecated: Use the Filter property instead. This api will be removed in
        /// the next release
        /// </summary>
        /// <param name="filterExpression">The filter expression to compile</param>
        public void SetFilter(string filterExpression)
        {
            int res;
            IntPtr bpfProgram;
            string errorString;

            // pcap_setfilter() requires a valid pcap_t which isn't present if
            // the device hasn't been opened
            ThrowIfNotOpen("device is not open");

            // attempt to compile the program
            if(!CompileFilter(PcapHandle, filterExpression, (uint)m_mask, out bpfProgram, out errorString))
            {
                string err = string.Format("Can't compile filter ({0}) : {1} ", filterExpression, errorString);
                throw new PcapException(err);
            }

            //associate the filter with this device
            res = SafeNativeMethods.pcap_setfilter( PcapHandle, bpfProgram );

            // Free the program whether or not we were successful in setting the filter
            // we don't want to leak unmanaged memory if we throw an exception.
            FreeBpfProgram(bpfProgram);

            //watch for errors
            if(res < 0)
            {
                errorString = string.Format("Can't set filter ({0}) : {1}", filterExpression, LastError);
                throw new PcapException(errorString);
            }
        }

        private string _filterString;

        /// <summary>
        /// Kernel level filtering expression associated with this device.
        /// For more info on filter expression syntax, see:
        /// http://www.winpcap.org/docs/docs31/html/group__language.html
        /// </summary>
        public virtual string Filter
        {
            get
            {
                return _filterString;
            }

            set
            {
                _filterString = value;
                SetFilter(_filterString);
            }
        }

        // If CompileFilter() returns true bpfProgram must be freed by passing it to FreeBpfProgram()
        /// or unmanaged memory will be leaked
        private static bool CompileFilter(IntPtr pcapHandle,
                                          string filterExpression,
                                          uint mask,
                                          out IntPtr bpfProgram,
                                          out string errorString)
        {
            int result;
            string err = String.Empty;

            bpfProgram = IntPtr.Zero;
            errorString = null;

            //Alocate an unmanaged buffer
            bpfProgram = Marshal.AllocHGlobal( Marshal.SizeOf(typeof(PcapUnmanagedStructures.bpf_program)));

            //compile the expressions
            result = SafeNativeMethods.pcap_compile(pcapHandle,
                                                    bpfProgram,
                                                    filterExpression,
                                                    1,
                                                    mask);

            if(result < 0)
            {
                err = GetLastError(pcapHandle);

                // free up the program memory
                Marshal.FreeHGlobal(bpfProgram);            
                bpfProgram = IntPtr.Zero; // make sure not to pass out a valid pointer

                // set the error string
                errorString = err;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Free memory allocated in CompileFilter()
        /// </summary>
        /// <param name="bpfProgram">
        /// A <see cref="IntPtr"/>
        /// </param>
        private static void FreeBpfProgram(IntPtr bpfProgram)
        {
            // free any pcap internally allocated memory from pcap_compile()
            SafeNativeMethods.pcap_freecode(bpfProgram);

            // free allocated buffers
            Marshal.FreeHGlobal(bpfProgram);            
        }

        /// <summary>
        /// Returns true if the filter expression was able to be compiled into a
        /// program without errors
        /// </summary>
        public static bool CheckFilter(string filterExpression,
                                       out string errorString)
        {
            IntPtr bpfProgram;
            IntPtr fakePcap = SafeNativeMethods.pcap_open_dead((int)PacketDotNet.LinkLayers.Ethernet, Pcap.MAX_PACKET_SIZE);

            uint mask = 0;
            if(!CompileFilter(fakePcap, filterExpression, mask, out bpfProgram, out errorString))
            {
                SafeNativeMethods.pcap_close(fakePcap);
                return false;
            }

            FreeBpfProgram(bpfProgram);

            SafeNativeMethods.pcap_close(fakePcap);
            return true;
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
