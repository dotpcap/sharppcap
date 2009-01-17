/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace SharpPcap.Packets
{
	/// <summary> Code constants for well-defined IP protocols.
	/// <p>
	/// Taken from netinet/in.h
	/// 
	/// </summary>
	public struct IPProtocols_Fields{
		/// <summary> Dummy protocol for TCP. </summary>
		public const int IP = 0;
		/// <summary> IPv6 Hop-by-Hop options. </summary>
		public const int HOPOPTS = 0;
		/// <summary> Internet Control Message Protocol. </summary>
		public const int ICMP = 1;
		/// <summary> Internet Group Management Protocol.</summary>
		public const int IGMP = 2;
		/// <summary> IPIP tunnels (older KA9Q tunnels use 94). </summary>
		public const int IPIP = 4;
		/// <summary> Transmission Control Protocol. </summary>
		public const int TCP = 6;
		/// <summary> Exterior Gateway Protocol. </summary>
		public const int EGP = 8;
		/// <summary> PUP protocol. </summary>
		public const int PUP = 12;
		/// <summary> User Datagram Protocol. </summary>
		public const int UDP = 17;
		/// <summary> XNS IDP protocol. </summary>
		public const int IDP = 22;
		/// <summary> SO Transport Protocol Class 4. </summary>
		public const int TP = 29;
		/// <summary> IPv6 header. </summary>
		public const int IPV6 = 41;
		/// <summary> IPv6 routing header. </summary>
		public const int ROUTING = 43;
		/// <summary> IPv6 fragmentation header. </summary>
		public const int FRAGMENT = 44;
		/// <summary> Reservation Protocol. </summary>
		public const int RSVP = 46;
		/// <summary> General Routing Encapsulation. </summary>
		public const int GRE = 47;
		/// <summary> encapsulating security payload. </summary>
		public const int ESP = 50;
		/// <summary> authentication header. </summary>
		public const int AH = 51;
		/// <summary> ICMPv6. </summary>
		public const int ICMPV6 = 58;
		/// <summary> IPv6 no next header. </summary>
		public const int NONE = 59;
		/// <summary> IPv6 destination options. </summary>
		public const int DSTOPTS = 60;
		/// <summary> Multicast Transport Protocol. </summary>
		public const int MTP = 92;
		/// <summary> Encapsulation Header. </summary>
		public const int ENCAP = 98;
		/// <summary> Protocol Independent Multicast. </summary>
		public const int PIM = 103;
		/// <summary> Compression Header Protocol. </summary>
		public const int COMP = 108;
		/// <summary> Raw IP packets. </summary>
		public const int RAW = 255;
		/// <summary> Unrecognized IP protocol.
		/// WARNING: this only works because the int storage for the protocol
		/// code has more bits than the field in the IP header where it is stored.
		/// </summary>
		public const int INVALID = - 1;
		/// <summary> IP protocol mask.</summary>
		public const int MASK = 0xff;
	}
	public interface IPProtocols
	{
		//UPGRADE_NOTE: Members of interface 'IPProtocols' were extracted into structure 'IPProtocols_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
		
	}
}
