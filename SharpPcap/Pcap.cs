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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpPcap
{
    /// <summary>
    /// Summary description for SharpPcap.
    /// </summary>
    public class Pcap
    {
        /// <summary>A delegate for Packet Arrival events</summary>
        public delegate void PacketArrivalEvent(object sender, Packets.Packet packet);

        /// <summary>
        /// A delegate for delivering network statistics
        /// </summary>
        public delegate void PcapStatisticsEvent(object sender, PcapStatistics statistics);

        /// <summary>
        /// A delegate fornotifying of a capture stopped event
        /// </summary>
        public delegate void PcapCaptureStoppedEvent(object sender, bool error);

        /// <summary>Represents the infinite number for packet captures </summary>
        public const int INFINITE = -1;

        /// <summary>A string value that prefixes avery pcap device name </summary>
        internal const string PCAP_NAME_PREFIX = @"\Device\NPF_";


        /* interface is loopback */
        internal const uint     PCAP_IF_LOOPBACK                = 0x00000001;
        private  const int      MAX_ADAPTER_NAME_LENGTH         = 256;      
        private  const int      MAX_ADAPTER_DESCRIPTION_LENGTH  = 128;
        internal const int      MAX_PACKET_SIZE                 = 65536;
        internal const int      PCAP_ERRBUF_SIZE                = 256;
        internal const int      MODE_CAPT                       =   0;
        internal const int      MODE_STAT                       =   1;
        internal const string   PCAP_SRC_IF_STRING              = "rpcap://";

        // Constants for address families
        // These are set in a Pcap static initializer because the values
        // differ between Windows and Linux
        private static int      AF_INET;
        private static int      AF_PACKET;
        private static int      AF_INET6;


        private void MyPacketHandler(IntPtr param, IntPtr /* pcap_pkthdr* */ header, IntPtr pkt_data)
        {
            DateTime tm;
            
            if (header != IntPtr.Zero)
            {
                PcapUnmanagedStructures.pcap_pkthdr PktInfo =
                    (PcapUnmanagedStructures.pcap_pkthdr)Marshal.PtrToStructure( header,
                                                                                typeof(PcapUnmanagedStructures.pcap_pkthdr) );
                /* convert the timestamp to readable format */
                tm = new DateTime( (long)(PktInfo.ts.tv_usec) );                
            
                Console.WriteLine("{0}, len: {1}", tm.ToShortTimeString(), PktInfo.len);
            }
        }


        public class Sockaddr
        {
            public enum Type
            {
                AF_INET_AF_INET6,
                HARDWARE,
                UNKNOWN
            }
            public Type type;

            public System.Net.IPAddress ipAddress;
            public byte[] hardwareAddress;

            private int _sa_family;
            public int sa_family
            {
                get { return _sa_family; }
            }

            public Sockaddr(IntPtr sockaddrPtr)
            {
                // A sockaddr struct. We use this to determine the address family
                PcapUnmanagedStructures.sockaddr saddr;

                // Marshal memory pointer into a struct
                saddr = (PcapUnmanagedStructures.sockaddr)Marshal.PtrToStructure(sockaddrPtr,
                                                         typeof(PcapUnmanagedStructures.sockaddr));

                // record the sa_family for informational purposes
                _sa_family = saddr.sa_family;

                byte[] addressBytes;
                if(saddr.sa_family == AF_INET)
                {
                    type = Type.AF_INET_AF_INET6; 
                    PcapUnmanagedStructures.sockaddr_in saddr_in = 
                        (PcapUnmanagedStructures.sockaddr_in)Marshal.PtrToStructure(sockaddrPtr,
                                                                                    typeof(PcapUnmanagedStructures.sockaddr_in));
                    ipAddress = new System.Net.IPAddress(saddr_in.sin_addr.s_addr);
                } else if(saddr.sa_family == AF_INET6)
                {
                    type = Type.AF_INET_AF_INET6;
                    addressBytes = new byte[16];
                    PcapUnmanagedStructures.sockaddr_in6 sin6 =
                        (PcapUnmanagedStructures.sockaddr_in6)Marshal.PtrToStructure(sockaddrPtr,
                                                             typeof(PcapUnmanagedStructures.sockaddr_in6));
                    Array.Copy(sin6.sin6_addr, addressBytes, addressBytes.Length);
                    ipAddress = new System.Net.IPAddress(addressBytes);
                } else if(saddr.sa_family == AF_PACKET)
                {
                    type = Type.HARDWARE;

                    PcapUnmanagedStructures.sockaddr_ll saddr_ll =
                        (PcapUnmanagedStructures.sockaddr_ll)Marshal.PtrToStructure(sockaddrPtr,
                                                          typeof(PcapUnmanagedStructures.sockaddr_ll));

                    hardwareAddress = new byte[saddr_ll.sll_halen];
                    for(int x = 0; x < saddr_ll.sll_halen; x++)
                    {
                        hardwareAddress[x] = saddr_ll.sll_addr[x];
                    }
                } else
                {
                    type = Type.UNKNOWN;

                    // place the sockaddr.sa_data into the hardware address just in case
                    // someone wants access to the bytes
                    hardwareAddress = new byte[saddr.sa_data.Length];
                    for(int x = 0; x < saddr.sa_data.Length; x++)
                    {
                        hardwareAddress[x] = saddr.sa_data[x];
                    }
                }
            }

            public override string ToString()
            {
                if(type == Type.AF_INET_AF_INET6)
                {
                    return ipAddress.ToString();
                } else if(type == Type.HARDWARE)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("HW addr: ");
                    for(int x = 0; x < hardwareAddress.Length; x++)
                    {
                        if(x == 0)
                            sb.AppendFormat("{0}", hardwareAddress[x].ToString("x2"));
                        else
                            sb.AppendFormat(":{0}", hardwareAddress[x].ToString("x2"));
                    }

                    return sb.ToString();
                } else if(type == Type.UNKNOWN)
                {
                    return String.Empty;
                }

                return String.Empty;
            }
        }

        // managed version of pcap_addr
        public class PcapAddress
        {
            public Sockaddr Addr;
            public Sockaddr Netmask;
            public Sockaddr Broadaddr;
            public Sockaddr Dstaddr;

            internal PcapAddress(PcapUnmanagedStructures.pcap_addr pcap_addr)
            {
                if(pcap_addr.Addr != IntPtr.Zero)
                    Addr = new Sockaddr( pcap_addr.Addr );
                if(pcap_addr.Netmask != IntPtr.Zero)
                    Netmask = new Sockaddr( pcap_addr.Netmask );
                if(pcap_addr.Broadaddr !=IntPtr.Zero)
                    Broadaddr = new Sockaddr( pcap_addr.Broadaddr );
                if(pcap_addr.Dstaddr != IntPtr.Zero)
                    Dstaddr = new Sockaddr( pcap_addr.Dstaddr );
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                if(Addr != null)
                    sb.AppendFormat("Addr:      {0}\n", Addr.ToString());

                if(Netmask != null)
                    sb.AppendFormat("Netmask:   {0}\n", Netmask.ToString());

                if(Broadaddr != null)
                    sb.AppendFormat("Broadaddr: {0}\n", Broadaddr.ToString());

                if(Dstaddr != null)
                    sb.AppendFormat("Dstaddr:   {0}\n", Dstaddr.ToString());

                return sb.ToString();
            }
        }

        // managed version of pcap_if
        // NOTE: we can't use pcap_if directly because the class contains
        //       a pointer to pcap_if that will be freed when the
        //       device memory is freed, so instead convert the unmanaged structure
        //       to a managed one to avoid this issue
        public class PcapInterface
        {
            public string            Name;        /* name to hand to "pcap_open_live()" */              
            public string            Description; /* textual description of interface */
            public List<PcapAddress> Addresses;
            public uint              Flags;       /* PCAP_IF_ interface flags */

            internal PcapInterface(PcapUnmanagedStructures.pcap_if pcapIf)
            {
                Name = pcapIf.Name;
                Description = pcapIf.Description;
                Flags = pcapIf.Flags;

                // retrieve addresses
                Addresses = new List<PcapAddress>();
                IntPtr address = pcapIf.Addresses;
                while(address != IntPtr.Zero)
                {
                    //A sockaddr struct
                    PcapUnmanagedStructures.pcap_addr addr;

                    //Marshal memory pointer into a struct
                    addr = (PcapUnmanagedStructures.pcap_addr)Marshal.PtrToStructure(address,
                                                                                     typeof(PcapUnmanagedStructures.pcap_addr));

                    Addresses.Add(new PcapAddress(addr));

                    address = addr.Next; // move to the next address
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Name: {0}\n", Name);
                sb.AppendFormat("Description: {0}\n", Description);
                foreach(PcapAddress addr in Addresses)
                {
                    sb.AppendFormat("Addresses:\n{0}\n", addr);
                }
                sb.AppendFormat("Flags: {0}\n", Flags);
                return sb.ToString();
            }
        }

        public static string Version
        {
            get
            {
                try
                {
                    return SafeNativeMethods.pcap_lib_version();
                }
                catch
                {
                    return "pcap version can't be identified, you are either using "+
                        "an older version, or pcap is not installed.";
                }
            }
        }

        private static bool isUnix()
        {
            int p = (int) Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                return true;
            } else {
                return false;
            }
        }

        static Pcap()
        {
            // happens to have the same value on Windows and Linux
            AF_INET = 2;

            // AF_PACKET = 17 on Linux, AF_NETBIOS = 17 on Windows
            // FIXME: need to resolve the discrepency at some point
            AF_PACKET = 17;

            if(isUnix())
            {
                AF_INET6 = 10; // value for linux from socket.h
            } else
            {
                AF_INET6 = 23; // value for windows from winsock.h
            }
        }

        /// <summary>
        /// Returns all pcap network devices available on this machine.
        /// </summary>
        public static List<PcapDevice> GetAllDevices()
        {
            IntPtr ptrDevs = IntPtr.Zero; // pointer to a PCAP_IF struct
            IntPtr next = IntPtr.Zero;    // pointer to a PCAP_IF struct
            StringBuilder errbuf = new StringBuilder( 256 ); //will hold errors
            List<PcapDevice> deviceList = new List<PcapDevice>();

            /* Retrieve the device list */
            int res = SafeNativeMethods.pcap_findalldevs(ref ptrDevs, errbuf);
            if (res == -1)
            {
                string err = "Error in WinPcap.GetAllDevices(): " + errbuf;
                throw new Exception( err );
            }
            else
            {   // Go through device structs and add to list
                next = ptrDevs;
                while (next != IntPtr.Zero)
                {
                    //Marshal memory pointer into a struct
                    PcapUnmanagedStructures.pcap_if pcap_if_unmanaged =
                        (PcapUnmanagedStructures.pcap_if)Marshal.PtrToStructure(next,
                                                        typeof(PcapUnmanagedStructures.pcap_if));
                    PcapInterface pcap_if = new PcapInterface(pcap_if_unmanaged);
                    deviceList.Add(new PcapDevice(pcap_if));
                    next = pcap_if_unmanaged.Next;
                }
            }
            SafeNativeMethods.pcap_freealldevs( ptrDevs );  // free buffers
            return deviceList;
        }

        /// <summary>
        /// Returns a PCAP_IF struct representing a pcap network device
        /// </summary>
        /// <param name="pcapName">The name of a device.<br>
        /// Can be either in pcap device format or windows network
        /// device format</param>
        /// <returns></returns>
        internal static PcapDevice GetPcapDeviceStruct(string pcapName)
        {
            if( !pcapName.StartsWith( PCAP_NAME_PREFIX ) )
            {
                pcapName = PCAP_NAME_PREFIX+pcapName;
            }

            List<PcapDevice> devices = GetAllDevices();
            foreach(PcapDevice d in devices)
            {
                if(d.Name.Equals(pcapName))
                {
                    return d;
                }
            }

            throw new Exception("Device not found: "+pcapName);
        }

        public static PcapOfflineDevice GetPcapOfflineDevice(string pcapFileName)
        {
            return new PcapOfflineDevice( pcapFileName );
        }

        public static PcapDevice GetPcapDevice( string pcapDeviceName )
        {
            return GetPcapDeviceStruct(pcapDeviceName);
        }
    }
}
