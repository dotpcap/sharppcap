/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> Code constants for ICMP message types.
    /// 
    /// Taken originally from tcpdump/print-icmp.c
    /// 
    /// </summary>
    public struct ICMPMessages_Fields{
        /// <summary> Echo reply.</summary>
        public readonly static int ECHO_REPLY = 0x0000;
        /// <summary> Destination network unreachable.</summary>
        public readonly static int UNREACH_NET = 0x0300;
        /// <summary> Destination host unreachable.</summary>
        public readonly static int UNREACH_HOST = 0x0301;
        /// <summary> Bad protocol.</summary>
        public readonly static int UNREACH_PROTOCOL = 0x0302;
        /// <summary> Bad port.</summary>
        public readonly static int UNREACH_PORT = 0x0303;
        /// <summary> IP_DF caused drop.</summary>
        public readonly static int UNREACH_NEEDFRAG = 0x0304;
        /// <summary> Src route failed.</summary>
        public readonly static int UNREACH_SRCFAIL = 0x0305;
        /// <summary> Unknown network.</summary>
        public readonly static int UNREACH_NET_UNKNOWN = 0x0306;
        /// <summary> Unknown host.</summary>
        public readonly static int UNREACH_HOST_UNKNOWN = 0x0307;
        /// <summary> Src host isolated.</summary>
        public readonly static int UNREACH_ISOLATED = 0x0308;
        /// <summary> Network access prohibited.</summary>
        public readonly static int UNREACH_NET_PROHIB = 0x0309;
        /// <summary> Host access prohibited.</summary>
        public readonly static int UNREACH_HOST_PROHIB = 0x030a;
        /// <summary> Bad TOS for net.</summary>
        public readonly static int UNREACH_TOSNET = 0x030b;
        /// <summary> Bad TOS for host.</summary>
        public readonly static int UNREACH_TOSHOST = 0x030c;
        /// <summary> Packet lost, slow down.</summary>
        public readonly static int SOURCE_QUENCH = 0x0400;
        /// <summary> Shorter route to network.</summary>
        public readonly static int REDIRECT_NET = 0x0500;
        /// <summary> Shorter route to host.</summary>
        public readonly static int REDIRECT_HOST = 0x0501;
        /// <summary> Shorter route for TOS and network.</summary>
        public readonly static int REDIRECT_TOSNET = 0x0502;
        /// <summary> Shorter route for TOS and host.</summary>
        public readonly static int REDIRECT_TOSHOST = 0x0503;
        /// <summary> Echo request.</summary>
        public readonly static int ECHO = 0x0800;
        /// <summary> router advertisement</summary>
        public readonly static int ROUTER_ADVERT = 0x0900;
        /// <summary> router solicitation</summary>
        public readonly static int ROUTER_SOLICIT = 0x0a00;
        /// <summary> time exceeded in transit.</summary>
        public readonly static int TIME_EXCEED_INTRANS = 0x0b00;
        /// <summary> time exceeded in reass.</summary>
        public readonly static int TIME_EXCEED_REASS = 0x0b01;
        /// <summary> ip header bad; option absent.</summary>
        public readonly static int PARAM_PROB = 0x0c01;
        /// <summary> timestamp request </summary>
        public readonly static int TSTAMP = 0x0d00;
        /// <summary> timestamp reply </summary>
        public readonly static int TSTAMP_REPLY = 0x0e00;
        /// <summary> information request </summary>
        public readonly static int IREQ = 0x0f00;
        /// <summary> information reply </summary>
        public readonly static int IREQ_REPLY = 0x1000;
        /// <summary> address mask request </summary>
        public readonly static int MASK_REQ = 0x1100;
        /// <summary> address mask reply </summary>
        public readonly static int MASK_REPLY = 0x1200;
        // marker indicating index of largest ICMP message type code
        public readonly static int LAST_MAJOR_CODE = 0x12;
    }
    public interface ICMPMessages
    {
        //UPGRADE_NOTE: Members of interface 'ICMPMessages' were extracted into structure 'ICMPMessages_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
        
        
    }
}
