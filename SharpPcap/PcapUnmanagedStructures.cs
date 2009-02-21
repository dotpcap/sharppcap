using System;
using System.Runtime.InteropServices;

namespace SharpPcap
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

            // TODO: would be great to be able to have the compiler take care of this for us
            //       but I'm not sure how to

            // pad the size of sockaddr_in out to 16 bytes
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
            private byte[]       pad;
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

        [StructLayout(LayoutKind.Sequential)]    
        public struct timeval
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
        /// Each packet in the dump file is prepended with this generic header.
        /// This gets around the problem of different headers for different
        /// packet interfaces.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct pcap_pkthdr 
        {
            public timeval  ts;             /* time stamp */
            public UInt32   caplen;         /* length of portion present */        public UInt32   len;            /* length this packet (off wire) */
        };

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

        #endregion Unmanaged Structs Implementation
    }
}
