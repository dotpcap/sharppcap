/*
Copyright (c) 2005 Tamir Gal, http://www.tamirgal.com, All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice,
        this list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright 
        notice, this list of conditions and the following disclaimer in 
        the documentation and/or other materials provided with the distribution.

    3. The names of the authors may not be used to endorse or promote products
        derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using SharpPcap.Util;
using SharpPcap.Packets;
using System.Runtime.InteropServices;

namespace SharpPcap
{
    /// <summary>
    /// Capture live packets from a network device
    /// </summary>
    public class PcapDevice
    {
        /// <summary>
        /// The working mode of a Pcap device
        /// </summary>
        public enum PcapMode
        {
            /// <summary>
            /// Set a Pcap device to Capture mode (MODE_CAPT)
            /// </summary>
            Capture,

            /// <summary>
            /// Set a Pcap device to Statistics mode (MODE_STAT)
            /// </summary>
            Statistics
        };

        private Pcap.PcapInterface m_pcapIf;
        public Pcap.PcapInterface Interface
        {
            get { return m_pcapIf; }
        }

        private IntPtr      m_pcapAdapterHandle = IntPtr.Zero;
        private IntPtr      m_pcapDumpHandle    = IntPtr.Zero;
        private bool        m_pcapStarted       = false;
        private PcapMode    m_pcapMode          = PcapMode.Capture;
        private int         m_pcapPacketCount   = Pcap.INFINITE;//Infinite
        private int         m_mask  = 0;//for filter expression

        /// <summary>
        /// Constructs a new PcapDevice based on a 'pcapIf' struct
        /// </summary>
        /// <param name="pcapIf">A 'pcapIf' struct representing
        /// the pcap device
        /// <summary>
        internal PcapDevice( Pcap.PcapInterface pcapIf )
        {
            m_pcapIf = pcapIf;
        }

        /// <summary>
        /// Default contructor for subclasses
        /// </summary>
        protected PcapDevice()
        {
        }

        /// <summary>
        /// Fires whenever a new packet is received on this Pcap Device.<br>
        /// This event is invoked only when working in "PcapMode.Capture" mode.
        /// </summary>
        public event Pcap.PacketArrivalEvent OnPacketArrival;

        /// <summary>
        /// Fires whenever a new pcap statistics is available for this Pcap Device.<br>
        /// This event is invoked only when working in "PcapMode.Statistics" mode.
        /// </summary>
        public event Pcap.PcapStatisticsEvent OnPcapStatistics;

        /// <summary>
        /// Fired when the capture process of this pcap device is stopped
        /// </summary>
        public event Pcap.PcapCaptureStoppedEvent OnCaptureStopped;
        
        /// <summary>
        /// Gets the pcap name of this network device
        /// </summary>
        public virtual string Name
        {
            get { return m_pcapIf.Name; }
        }

        public virtual List<Pcap.PcapAddress> Addresses
        {
            get { return m_pcapIf.Addresses; }
        }

        /// <summary>
        /// Gets the pcap description of this device
        /// </summary>
        public virtual string Description
        {
            get{ return m_pcapIf.Description; }
        }

        public virtual uint Flags
        {
            get{ return m_pcapIf.Flags; }
        }

        public virtual bool Loopback
        {
            get{ return (Flags & Pcap.PCAP_IF_LOOPBACK)==1; }
        }

        /// <summary>
        /// The underlying pcap device handle
        /// </summary>
        internal virtual IntPtr PcapHandle
        {
            get{return m_pcapAdapterHandle;}
            set{m_pcapAdapterHandle=value;}
        }

        /// <summary>
        /// Return the pcap link layer value of an adapter. 
        /// </summary>
        public virtual int PcapDataLink
        {
            get
            {
                if(!Opened)
                    throw new InvalidOperationException("Cannot get datalink, the pcap device is not opened");
                return SafeNativeMethods.pcap_datalink(PcapHandle);
            }
        }

        /// <summary>
        /// Return a value indicating if this adapter is opened
        /// </summary>
        public virtual bool Opened
        {
            get{return (PcapHandle != IntPtr.Zero);}
        }

        /// <summary>
        /// Return a value indicating if the capturing process of this adapter is started
        /// </summary>
        public virtual bool Started
        {
            get{return m_pcapStarted;}
        }

        public virtual PcapMode Mode
        {
            get{return m_pcapMode;}
            set
            {
                if(!Opened)
                    throw new PcapException
                        ("Can't set PcapMode, the device is not opened");

                m_pcapMode = value;
                int mode = ( m_pcapMode==PcapMode.Capture ? 
                             Pcap.MODE_CAPT : 
                             Pcap.MODE_STAT);
                SafeNativeMethods.pcap_setmode(this.PcapHandle ,mode);
            }
        }

        /// <summary>
        /// Open the device with default values of: promiscuous_mode=false, read_timeout=1000
        /// To start capturing call the 'PcapStartCapture' function
        /// </summary>
        public virtual void Open()
        {
            this.Open(false);
        }

        /// <summary>
        /// Open the device. To start capturing call the 'PcapStartCapture' function
        /// </summary>
        /// <param name="promiscuous_mode">A value indicating wether to open the
        ///  device in promiscuous mode (true = capture *all* packets on the network,
        ///  including packets not for me)</param>
        public virtual void Open(bool promiscuous_mode)
        {
            this.Open( promiscuous_mode, 1000 );
        }

        /// <summary>
        /// Open the device. To start capturing call the 'PcapStartCapture' function
        /// </summary>
        /// <param name="promiscuous_mode">A value indicating wether to open the
        ///  device in promiscuous mode (true = capture *all* packets on the network,
        ///  including packets not for me)</param>
        /// <param name="read_timeout">The timeout in miliseconds to wait for a  packet arrival</param>
        public virtual void Open(bool promiscuous_mode, int read_timeout)
        {
            short mode = 0;
            if (promiscuous_mode) mode = 1;

            if ( !Opened )
            {
                StringBuilder errbuf = new StringBuilder( Pcap.PCAP_ERRBUF_SIZE ); //will hold errors

                PcapHandle = SafeNativeMethods.pcap_open_live
                    (   Name,           // name of the device
                        Pcap.MAX_PACKET_SIZE,   // portion of the packet to capture. 
                                            // MAX_PACKET_SIZE (65536) grants that the whole packet will be captured on all the MACs.
                        mode,               // promiscuous mode
                        (short)read_timeout,// read timeout                                             
                        errbuf );           // error buffer

                if ( PcapHandle == IntPtr.Zero)
                {
                    string err = "Unable to open the adapter ("+Name+"). "+errbuf.ToString();
                    throw new Exception( err );
                }
            }
        }

        /// <summary>
        /// Starts the capturing process
        /// </summary>
        public virtual void StartCapture()
        {
            if (!Started)
            {
                if (!Opened)
                    throw new Exception("Can't start capture, the pcap device is not opened.");

                Thread thSniff = new Thread(new ThreadStart(this.CaptureThread));
                thSniff.Start();
            }
        }

        /// <summary>
        /// Captures packets on this network device. This method will block
        /// until capturing is finished.
        /// </summary>
        /// <param name="packetCount">The number of packets to be captured. 
        /// Value of '-1' means infinite.</param>
        public virtual void Capture(int packetCount)
        {
            m_pcapPacketCount = packetCount;
            CaptureThread();
            m_pcapPacketCount = Pcap.INFINITE;;
        }

        public virtual void CaptureThread()
        {
            SafeNativeMethods.pcap_handler Callback = new SafeNativeMethods.pcap_handler(PacketHandler);

            m_pcapStarted = true;

            int res = loop(m_pcapPacketCount, Callback, new IntPtr());

            switch (res)    // Check pcap loop status results and notify upstream.
            {
                case Pcap.LOOP_USER_TERMINATED:     // User requsted loop termination with StopCapture()
                    SendCaptureStoppedEvent(false);
                    break;
                case Pcap.LOOP_COUNT_EXHAUSTED:     // m_pcapPacketCount exceeded (successful exit)
                    SendCaptureStoppedEvent(false);
                    break;
                case Pcap.LOOP_EXIT_WITH_ERROR:     // An error occoured whilst capturing.
                    SendCaptureStoppedEvent(true);
                    break;

                default:    // This can only be triggered by a bug in libpcap.
                    throw new Exception("Unknown pcap_loop exit status.");
            }

            m_pcapStarted = false;
        }

        /// <summary>
        /// Stops the capture process
        /// </summary>
        public virtual void StopCapture()
        {
            if (Started)
            {
                SafeNativeMethods.pcap_breakloop(PcapHandle);
            }
        }

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
                foreach(Pcap.PacketArrivalEvent pa in OnPacketArrival.GetInvocationList())
                {
                    OnPacketArrival -= pa;
                }
            }
            if ( OnPcapStatistics != null)
            {
                foreach(Pcap.PcapStatisticsEvent pse in OnPcapStatistics.GetInvocationList())
                {
                    OnPcapStatistics -= pse;
                }
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
        /// <returns>A reference to a packet object</returns
        public virtual int GetNextPacket(out Packet p)
        {
            //Pointer to a packet info struct
            IntPtr header = IntPtr.Zero;
            //Pointer to a packet struct
            IntPtr data = IntPtr.Zero;
            int res = 0;

            // using an invalid PcapHandle can result in an unmanaged segfault
            // so check for that here
            if(!Opened)
            {
                throw new PcapException("Device must be opened via Open() prior to use");
            }

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
        /// The capture procedure.
        /// </summary>
        private int loop(int count, SafeNativeMethods.pcap_handler callback, IntPtr user)
        {
            return SafeNativeMethods.pcap_loop(PcapHandle, count, callback, user);
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
            Packet p = new Packet();
            PcapUnmanagedStructures.pcap_pkthdr pkt_header =
                (PcapUnmanagedStructures.pcap_pkthdr)Marshal.PtrToStructure(header,
                                                                             typeof(PcapUnmanagedStructures.pcap_pkthdr));
            byte[] pkt_data = new byte[pkt_header.caplen];
            Marshal.Copy(data, pkt_data, 0, (int)pkt_header.caplen);

            p = Packets.PacketFactory.dataToPacket(PcapDataLink, pkt_data,
                                                   new Packets.Util.Timeval((ulong)pkt_header.ts.tv_sec,
                                                                            (ulong)pkt_header.ts.tv_usec));
            p.pcapHeader = new PcapHeader(pkt_header);

            return p;
        }

        private void SendPacketArrivalEvent(Packet p)
        {
            //If mode is MODE_CAP:
            if(Mode==PcapMode.Capture)
            {
                if(OnPacketArrival != null )
                {
                    //Invoke the packet arrival event                                           
                    OnPacketArrival(this, new PcapCaptureEventArgs(p));
                }
            }
            //else mode is MODE_STAT
            else if(Mode==PcapMode.Statistics)
            {
                if(OnPcapStatistics != null)
                {
                    //Invoke the pcap statistics event
                    OnPcapStatistics(this, new PcapStatisticsEventArgs(p));
                }
            }
        }

        private void SendCaptureStoppedEvent(bool error)
        {
            if(OnCaptureStopped!=null)
            {
                //Notify upper applications
                OnCaptureStopped(this, error);
            }
        }

        /// <summary>
        /// Compile a kernel level filtering expression, and associate the filter 
        /// with this device. For more info on filter expression syntax, see:
        /// http://www.winpcap.org/docs/docs31/html/group__language.html
        /// </summary>
        /// <param name="filterExpression">The filter expression to 
        /// compile</param>
        public virtual void SetFilter(string filterExpression)
        {
            int res; IntPtr err_ptr; string err="";

            //pointer to a bpf_program struct 
            IntPtr program = IntPtr.Zero;
            //Alocate an unmanaged buffer
            program = Marshal.AllocHGlobal( Marshal.SizeOf(typeof(PcapUnmanagedStructures.bpf_program)));
            //compile the expressions
            res = SafeNativeMethods.pcap_compile(PcapHandle, program, filterExpression,1, (uint)m_mask);
            //watch for errors
            if(res<0)
            {
                try
                {
                    err_ptr = SafeNativeMethods.pcap_geterr( PcapHandle );
                    err = Marshal.PtrToStringAnsi( err_ptr );
                }
                catch{}
                err = "Can't compile filter: "+err;
                throw new PcapException(err);
            }
            //associate the filter with this device
            res = SafeNativeMethods.pcap_setfilter( PcapHandle, program );
            //watch for errors
            if(res<0)
            {
                try
                {
                    err_ptr = SafeNativeMethods.pcap_geterr(PcapHandle);
                    err = Marshal.PtrToStringAnsi(err_ptr);
                }
                catch{}
                err = "Can't set filter.\n"+err;
                throw new PcapException(err);
            }

            // free any pcap internally allocated memory from pcap_compile()
            SafeNativeMethods.pcap_freecode(program);

            // free allocated buffers
            Marshal.FreeHGlobal(program);
        }

        /// <summary>
        /// Opens a file for packet writings
        /// </summary>
        /// <param name="fileName"></param>
        public void DumpOpen(string fileName)
        {
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
        /// <param name="fileName"></param>
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
        /// <param name="fileName"></param>
        public void DumpFlush()
        {
            if(DumpOpened)
                SafeNativeMethods.pcap_dump_flush(m_pcapDumpHandle);
        }

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        /// <param name="p">The packet to write</param>
        public void Dump(byte[] p, PcapHeader h)
        {
            if(!Opened)
                throw new InvalidOperationException("Cannot dump packet, device is not opened");
            if(!DumpOpened)
                throw new InvalidOperationException("Cannot dump packet, dump file is not opened");

            //Marshal packet
            IntPtr pktPtr;
            pktPtr = Marshal.AllocHGlobal(p.Length);
            Marshal.Copy(p, 0, pktPtr, p.Length);

            //Marshal header
            IntPtr hdrPtr;
            hdrPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PcapUnmanagedStructures.pcap_pkthdr)));
            Marshal.StructureToPtr(h.m_pcap_pkthdr, hdrPtr, true);

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

        /// <summary>
        /// Gets a value indicating wether pcap dump file is already associated with this device
        /// </summary>
        public bool DumpOpened
        {
            get{return m_pcapDumpHandle!=IntPtr.Zero;}
        }

        /// <summary>
        /// Sends a raw packet throgh this device
        /// </summary>
        /// <param name="p">The packet to send</param>
        public void SendPacket(Packet p)
        {
            SendPacket(p.Bytes);
        }


        /// <summary>
        /// Sends a raw packet throgh this device
        /// </summary>
        /// <param name="p">The packet to send</param>
        /// <param name="size">The number of bytes to send</param>
        public void SendPacket(Packet p, int size)
        {
            SendPacket(p.Bytes, size);
        }

        /// <summary>
        /// Sends a raw packet throgh this device
        /// </summary>
        /// <param name="p">The packet bytes to send</param>
        public void SendPacket(byte[] p)
        {
            SendPacket(p, p.Length);
        }

        /// <summary>
        /// Sends a raw packet throgh this device
        /// </summary>
        /// <param name="p">The packet bytes to send</param>
        /// <param name="size">The number of bytes to send</param>
        public void SendPacket(byte[] p, int size)
        {
            if ( Opened )
            {
                if (size > p.Length)
                {
                    throw new ArgumentException("Invalid packetSize value: "+size+
                    "\nArgument size is larger than the total size of the packet.");
                }

                if (p.Length > Pcap.MAX_PACKET_SIZE) 
                {
                    throw new ArgumentException("Packet length can't be larger than "+Pcap.MAX_PACKET_SIZE);
                }

                IntPtr p_packet = IntPtr.Zero;          
                p_packet = Marshal.AllocHGlobal( size );
                Marshal.Copy(p, 0, p_packet, size);     

                int res = SafeNativeMethods.pcap_sendpacket(PcapHandle, p_packet, size);
                Marshal.FreeHGlobal(p_packet);
                if(res < 0)
                {
                    throw new PcapException("Can't send packet: " + LastError);
                }
            }
            else
            {
                throw new PcapException("Can't send packet, the device is closed");
            }
        }

        /// <summary>
        /// Sends all packets in a 'PcapSendQueue' out this pcap device
        /// </summary>
        /// <param name="q">The 'PcapSendQueue' hodling the packets</param>
        public int SendQueue( PcapSendQueue q, bool sync )
        {
            return q.Transmit( this, sync );
        }

        /// <summary>
        /// The last pcap error associated with this pcap device
        /// </summary>
        public string LastError
        {
            get
            {
                IntPtr err_ptr = SafeNativeMethods.pcap_geterr( PcapHandle );
                return Marshal.PtrToStringAnsi( err_ptr );
            }
        }

        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("interface: {0}\n", m_pcapIf.ToString());
            return sb.ToString();
        }
    }
}
