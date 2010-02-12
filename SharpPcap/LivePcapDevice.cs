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
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 * Copyright 2008-2010 Phillip Lemon <lucidcomms@gmail.com>
 */

using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using SharpPcap.Packets;

namespace SharpPcap
{
    /// <summary>
    /// Capture live packets from a network device
    /// </summary>
    public class LivePcapDevice : PcapDevice
    {
        private int          m_mask  = 0; //for filter expression

        /// <summary>
        /// Constructs a new PcapDevice based on a 'pcapIf' struct
        /// </summary>
        /// <param name="pcapIf">A 'pcapIf' struct representing
        /// the pcap device</param>
        internal LivePcapDevice( PcapInterface pcapIf )
        {
            m_pcapIf = pcapIf;
        }

        /// <summary>
        /// Default contructor for subclasses
        /// </summary>
        protected LivePcapDevice()
        {
        }

        /// <summary>
        /// PcapDevice finalizer.  Ensure PcapDevices are stopped and closed before exit.
        /// </summary>
        ~LivePcapDevice()
        {
            this.Close();
        }

        /// <summary>
        /// Gets the pcap name of this network device
        /// </summary>
        public override string Name
        {
            get { return m_pcapIf.Name; }
        }

        public virtual ReadOnlyCollection<PcapAddress> Addresses
        {
            get { return new ReadOnlyCollection<PcapAddress>(m_pcapIf.Addresses); }
        }

        /// <summary>
        /// Gets the pcap description of this device
        /// </summary>
        public override string Description
        {
            get { return m_pcapIf.Description; }
        }

        public virtual uint Flags
        {
            get { return m_pcapIf.Flags; }
        }

        public virtual bool Loopback
        {
            get { return (Flags & Pcap.PCAP_IF_LOOPBACK)==1; }
        }

        /// <summary>
        /// Open the device with default values of: promiscuous_mode = false, read_timeout = 1000
        /// To start capturing call the 'StartCapture' function
        /// </summary>
        public override void Open()
        {
            this.Open(DeviceMode.Normal);
        }

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="mode">
        /// A <see cref="DeviceMode"/>
        /// </param>
        public virtual void Open(DeviceMode mode)
        {
            const int readTimeoutMilliseconds = 1000;
            this.Open(mode, readTimeoutMilliseconds);
        }

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="mode">
        /// A <see cref="DeviceMode"/>
        /// </param>
        /// <param name="read_timeout">
        /// A <see cref="System.Int32"/>
        /// </param>
        public virtual void Open(DeviceMode mode, int read_timeout)
        {
            if ( !Opened )
            {
                StringBuilder errbuf = new StringBuilder( Pcap.PCAP_ERRBUF_SIZE ); //will hold errors

                PcapHandle = SafeNativeMethods.pcap_open_live
                    (   Name,                   // name of the device
                        Pcap.MAX_PACKET_SIZE,   // portion of the packet to capture. 
                                                // MAX_PACKET_SIZE (65536) grants that the whole packet will be captured on all the MACs.
                        (short)mode,            // promiscuous mode
                        (short)read_timeout,    // read timeout
                        errbuf );               // error buffer

                if ( PcapHandle == IntPtr.Zero)
                {
                    string err = "Unable to open the adapter ("+Name+"). "+errbuf.ToString();
                    throw new PcapException( err );
                }
            }
        }

        private const int disableBlocking = 0;
        private const int enableBlocking = 1;

        /// <summary>
        /// Set/Get Non-Blocking Mode. returns allways false for savefiles.
        /// </summary>
        public bool NonBlockingMode
        {
            get
            {
                var errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors
                int ret = SafeNativeMethods.pcap_getnonblock(PcapHandle, errbuf);

                // Errorbuf is only filled when ret = -1
                if (ret == -1)
                {
                    string err = "Unable to set get blocking" + errbuf.ToString();
                    throw new PcapException(err);
                }

                if(ret == enableBlocking)
                    return true;
                return false;
            }
            set 
            {
                var errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors

                int block = disableBlocking;
                if (value)
                    block = enableBlocking;

                int ret = SafeNativeMethods.pcap_setnonblock(PcapHandle, block, errbuf);

                // Errorbuf is only filled when ret = -1
                if (ret == -1)
                {
                    string err = "Unable to set non blocking" + errbuf.ToString();
                    throw new PcapException(err);
                }
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
            IntPtr fakePcap = SafeNativeMethods.pcap_open_dead((int)LinkLayers.Ethernet10Mb, Pcap.MAX_PACKET_SIZE);

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

        /// <summary>
        /// Compile a kernel level filtering expression, and associate the filter 
        /// with this device. For more info on filter expression syntax, see:
        /// http://www.winpcap.org/docs/docs31/html/group__language.html
        /// </summary>
        /// <param name="filterExpression">The filter expression to compile</param>
        public virtual void SetFilter(string filterExpression)
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
            ThrowIfNotOpen("Can't send packet, the device is closed");

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

        /// <summary>
        /// Sends all packets in a 'PcapSendQueue' out this pcap device
        /// </summary>
        /// <param name="q">
        /// A <see cref="SendQueue"/>
        /// </param>
        /// <param name="transmitMode">
        /// A <see cref="SendQueueTransmitModes"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public int SendQueue( SendQueue q, SendQueueTransmitModes transmitMode )
        {
            return q.Transmit( this, transmitMode);
        }

        /// <summary>
        /// Retrieves pcap statistics
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public override PcapStatistics Statistics()
        {
            // can only call PcapStatistics on an open device
            ThrowIfNotOpen("device not open");

            return new PcapStatistics(this.m_pcapAdapterHandle);
        }

        /// <value>
        /// Set the kernel value buffer size in bytes
        /// WinPcap extension
        /// </value>
        public int KernelBufferSize
        {
            set
            {
                ThrowIfNotWinPcap();
                ThrowIfNotOpen("Can't set kernel buffer size, the device is not opened");

                int retval = SafeNativeMethods.pcap_setbuff(this.m_pcapAdapterHandle,
                                                            value);
                if(retval != 0)
                {
                    throw new System.InvalidOperationException("pcap_setbuff() failed");
                }
            }   
        }
    }
}
