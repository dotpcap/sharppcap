/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> Link-layer type codes.
    /// <p>
    /// Taken from libpcap/bpf/net/bpf.h and pcap/net/bpf.h.
    /// <p>
    /// The link-layer type is used to determine what data-structure the 
    /// IP protocol bits will be encapsulated inside of.
    /// <p>
    /// On a 10/100mbps network, packets are encapsulated inside of ethernet.
    /// 14-byte ethernet headers which contain MAC addresses and an ethernet type 
    /// field.
    /// <p>
    /// On ethernet over ppp, the link-layer type is raw, and packets 
    /// are not encapsulated in any ethernet header. 
    /// <p>
    /// 
    /// </summary>
    public struct LinkLayers_Fields{
        /// <summary> no link-layer encapsulation </summary>
        public const int NULL = 0;
        /// <summary> Ethernet (10Mb) </summary>
        public const int EN10MB = 1;
        /// <summary> Experimental Ethernet (3Mb) </summary>
        public const int EN3MB = 2;
        /// <summary> Amateur Radio AX.25 </summary>
        public const int AX25 = 3;
        /// <summary> Proteon ProNET Token Ring </summary>
        public const int PRONET = 4;
        /// <summary> Chaos </summary>
        public const int CHAOS = 5;
        /// <summary> IEEE 802 Networks </summary>
        public const int IEEE802 = 6;
        /// <summary> ARCNET </summary>
        public const int ARCNET = 7;
        /// <summary> Serial Line IP </summary>
        public const int SLIP = 8;
        /// <summary> Point-to-point Protocol </summary>
        public const int PPP = 9;
        /// <summary> FDDI </summary>
        public const int FDDI = 10;
        /// <summary> LLC/SNAP encapsulated atm </summary>
        public const int ATM_RFC1483 = 11;
        /// <summary> raw IP </summary>
        public const int RAW = 12;
        /// <summary> BSD Slip.</summary>
        public const int SLIP_BSDOS = 15;
        /// <summary> BSD PPP.</summary>
        public const int PPP_BSDOS = 16;
        /// <summary> IP over ATM.</summary>
        public const int ATM_CLIP = 19;
        /// <summary> PPP over HDLC.</summary>
        public const int PPP_SERIAL = 50;
        /// <summary> Cisco HDLC.</summary>
        public const int CHDLC = 104;
        /// <summary> IEEE 802.11 wireless.</summary>
        public const int IEEE802_11 = 105;
        /// <summary> OpenBSD loopback.</summary>
        public const int LOOP = 108;
        /// <summary> Linux cooked sockets.</summary>
        public const int LINUX_SLL = 113;
        /// <summary> unknown link-layer type</summary>
        public const int UNKNOWN = - 1;
    }
    public interface LinkLayers
    {
        //UPGRADE_NOTE: Members of interface 'LinkLayers' were extracted into structure 'LinkLayers_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
        
    }
}
