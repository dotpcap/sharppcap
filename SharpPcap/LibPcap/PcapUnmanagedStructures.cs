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
 */

using System;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    internal class PcapUnmanagedStructures
    {
        #region Unmanaged Structs Implementation

        /// <summary>
        /// Item in a list of interfaces.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct pcap_if 
        {
            public IntPtr /* pcap_if* */    Next;           
            public string                   Name;           /* name to hand to "pcap_open_live()" */                
            public string                   Description;    /* textual description of interface, or NULL */
            public IntPtr /*pcap_addr * */  Addresses;
            public UInt32                   Flags;          /* PCAP_IF_ interface flags */
        };

        /// <summary>
        /// Representation of an interface address.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct pcap_addr 
        {
            public IntPtr /* pcap_addr* */  Next;
            public IntPtr /* sockaddr * */  Addr;       /* address */
            public IntPtr /* sockaddr * */  Netmask;    /* netmask for that address */
            public IntPtr /* sockaddr * */  Broadaddr;  /* broadcast address for that address */
            public IntPtr /* sockaddr * */  Dstaddr;    /* P2P destination address for that address */
        };

        /// <summary>
        /// Structure used by kernel to store a generic address
        /// Look at the sa_family value to determine which specific structure to use
        /// 'struct sockaddr'
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct sockaddr 
        {
            public UInt16       sa_family;      /* address family */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=14)]
            public byte[]       sa_data;        /* 14 bytes of protocol address */
        };

        /// <summary>
        /// Structure that holds an ipv4 address
        /// </summary>
        public struct in_addr
        {
            public UInt32 s_addr;
        }

        /// <summary>
        /// Structure that holds an ipv4 address
        /// 'struct sockaddr'
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct sockaddr_in
        {
            public UInt16       sa_family;      /* address family */
            public UInt16       sa_port;        /* port */
            public in_addr      sin_addr;       /* address */

            // TODO: I'm not sure that we can define a fixed field in another easier to
            //       understand way

            // pad the size of sockaddr_in out to 16 bytes
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
// Disable warnings around this unused field
#pragma warning disable 0169
            private byte[]       pad;
#pragma warning restore 0169
        };

        /// <summary>
        /// Structure that holds an ipv6 address
        /// NOTE: we cast the 'struct sockaddr*' to this structure based on the sa_family type
        /// 'struct sockaddr_in6'
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct sockaddr_in6
        {
            public UInt16       sin6_family;    /* address family */
            public UInt16       sin6_port;      /* Transport layer port # */
            public UInt32       sin6_flowinfo;  /* IPv6 flow information */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=16)]
            public byte[]       sin6_addr;      /* IPv6 address */
            public UInt32       sin6_scope_id;  /* scope id (new in RFC2553) */
        };

        /// <summary>
        /// Structure to represent a low level address, like a hardware address
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct sockaddr_ll
        {
            public UInt16 sll_family;
            public UInt16 sll_protocol;
            public UInt32 sll_ifindex;
            public UInt16 sll_hatype;
            public byte   sll_pkttype;
            public byte   sll_halen;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
            public byte[] sll_addr;
        };

        #region timeval
        /// <summary>
        /// Windows and Unix differ in their memory models and make it difficult to
        /// support struct timeval in a single library, like this one, across
        /// multiple platforms.
        ///
        /// See http://en.wikipedia.org/wiki/64bit#Specific_data_models
        ///
        /// The issue is that struct timeval { long tv_sec; long tv_usec; }
        /// has different sizes on Linux 32 and 64bit but the same size on
        /// Windows 32 and 64 bit
        ///
        /// Thanks to Jon Pryor for his help in figuring out both the issue with Linux
        /// 32/64bit and the issue between Windows and Unix
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]    
        public struct timeval_unix
        {
            // NOTE: The use of IntPtr here is due to the issue with the timeval structure
            //       The timeval structure contains long values, which differ between 32 bit and
            //       64 bit platforms. One trick, thanks to Jon Pryor for the suggestion, is to
            //       use IntPtr. The size of IntPtr will change depending on the platform the
            //       code runs on, so it should handle the size properly on both 64 bit and 32 bit platforms.
            public IntPtr tv_sec;
            public IntPtr tv_usec;
        };

        /// <summary>
        /// Windows version of struct timeval, the longs are 32bit even on 64-bit versions of Windows
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]    
        public struct timeval_windows
        {
            public Int32 tv_sec;
            public Int32 tv_usec;
        };
        #endregion

        #region pcap_pkthdr
        /// <summary>
        /// Each packet in the dump file is prepended with this generic header.
        /// This gets around the problem of different headers for different
        /// packet interfaces.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct pcap_pkthdr_unix
        {
            public timeval_unix     ts;             /* time stamp */
            public UInt32           caplen;         /* length of portion present */
            public UInt32           len;            /* length this packet (off wire) */
        };

        /// <summary>
        /// Each packet in the dump file is prepended with this generic header.
        /// This gets around the problem of different headers for different
        /// packet interfaces.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct pcap_pkthdr_windows
        {
            public timeval_windows  ts;             /* time stamp */
            public UInt32           caplen;         /* length of portion present */
            public UInt32           len;            /* length this packet (off wire) */
        };
        #endregion

        /// <summary>
        /// Packet data bytes
        /// NOTE: This struct doesn't exist in header files, it is a construct to map to an
        ///        unmanaged byte array
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct PCAP_PKTDATA
        {   
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=SharpPcap.Pcap.MAX_PACKET_SIZE)]                     
            public byte[]       bytes;
        };

        /// <summary>
        /// A BPF pseudo-assembly program for packet filtering
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct bpf_program 
        {
            public uint bf_len;                
            public IntPtr /* bpf_insn **/ bf_insns;  
        };

        /// <summary>
        /// A queue of raw packets that will be sent to the network with pcap_sendqueue_transmit()
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct pcap_send_queue 
        {
            public uint maxlen;   
            public uint len;   
            public IntPtr /* char **/ ptrBuff;  
        };

        /// <summary>
        /// Define the return values from int pcap_stats()
        /// </summary>
        internal enum PcapStatReturnValue : int
        {
            Success = 0,
            Error = -1
        }

        /// <summary>
        /// Unix version of 'struct pcap_stat'
        /// Uses the same trick as timeval_unix
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct pcap_stat_unix
        {
            /// <summary>
            /// Packets received
            /// </summary>
            public IntPtr ps_recv;

            /// <summary>
            /// Packets dropped
            /// </summary>
            public IntPtr ps_drop;

            /// <summary>
            /// Drops by interface (maybe not yet supported)
            /// </summary>
            public IntPtr ps_ifdrop;
        }

        /// <summary>
        /// Windows version of 'struct pcap_stat'
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct pcap_stat_windows
        {
            /// <summary>
            /// Packets received
            /// </summary>
            public uint ps_recv;

            /// <summary>
            /// Packets dropped
            /// </summary>
            public uint ps_drop;

            /// <summary>
            /// Drops by interface (maybe not yet supported)
            /// </summary>
            public uint ps_ifdrop;

            /// <summary>
            /// Packets that reach the application
            /// WIN32 only, based on struct pcap_stat in pcap.h
            /// </summary>
            public uint bs_capt;
        }

        #endregion Unmanaged Structs Implementation
    }
}
