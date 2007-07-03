// $Id: EthernetProtocols.cs,v 1.1.1.1 2007-07-03 10:15:17 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace Tamir.IPLib.Packets
{
	
	
	/// <summary> Code constants for well-defined ethernet protocols.
	/// <p>
	/// Taken from linux/if_ether.h and tcpdump/ethertype.h
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:17 $ </lastModifiedAt>
	public struct EthernetProtocols_Fields{
		/// <summary> IP protocol.</summary>
		public const int IP = 0x0800;
		/// <summary> Address resolution protocol.</summary>
		public const int ARP = 0x0806;
		/// <summary> Reverse address resolution protocol.</summary>
		public const int RARP = 0x8035;
		/// <summary> Ethernet Loopback packet </summary>
		public const int LOOP = 0x0060;
		/// <summary> Ethernet Echo packet		</summary>
		public const int ECHO = 0x0200;
		/// <summary> Xerox PUP packet</summary>
		public const int PUP = 0x0400;
		/// <summary> CCITT X.25			</summary>
		public const int X25 = 0x0805;
		/// <summary> G8BPQ AX.25 Ethernet Packet	[ NOT AN OFFICIALLY REGISTERED ID ] </summary>
		public const int BPQ = 0x08FF;
		/// <summary> DEC Assigned proto</summary>
		public const int DEC = 0x6000;
		/// <summary> DEC DNA Dump/Load</summary>
		public const int DNA_DL = 0x6001;
		/// <summary> DEC DNA Remote Console</summary>
		public const int DNA_RC = 0x6002;
		/// <summary> DEC DNA Routing</summary>
		public const int DNA_RT = 0x6003;
		/// <summary> DEC LAT</summary>
		public const int LAT = 0x6004;
		/// <summary> DEC Diagnostics</summary>
		public const int DIAG = 0x6005;
		/// <summary> DEC Customer use</summary>
		public const int CUST = 0x6006;
		/// <summary> DEC Systems Comms Arch</summary>
		public const int SCA = 0x6007;
		/// <summary> Appletalk DDP </summary>
		public const int ATALK = 0x809B;
		/// <summary> Appletalk AARP</summary>
		public const int AARP = 0x80F3;
		/// <summary> IPX over DIX</summary>
		public const int IPX = 0x8137;
		/// <summary> IPv6 over bluebook</summary>
		public const int IPV6 = 0x86DD;
		/// <summary> Dummy type for 802.3 frames  </summary>
		public const int N802_3 = 0x0001;
		/// <summary> Dummy protocol id for AX.25  </summary>
		public const int AX25 = 0x0002;
		/// <summary> Every packet.</summary>
		public const int ALL = 0x0003;
		/// <summary> 802.2 frames</summary>
		public const int N802_2 = 0x0004;
		/// <summary> Internal only</summary>
		public const int SNAP = 0x0005;
		/// <summary> DEC DDCMP: Internal only</summary>
		public const int DDCMP = 0x0006;
		/// <summary> Dummy type for WAN PPP frames</summary>
		public const int WAN_PPP = 0x0007;
		/// <summary> Dummy type for PPP MP frames </summary>
		public const int PPP_MP = 0x0008;
		/// <summary> Localtalk pseudo type </summary>
		public const int LOCALTALK = 0x0009;
		/// <summary> Dummy type for Atalk over PPP</summary>
		public const int PPPTALK = 0x0010;
		/// <summary> 802.2 frames</summary>
		public const int TR_802_2 = 0x0011;
		/// <summary> Mobitex (kaz@cafe.net)</summary>
		public const int MOBITEX = 0x0015;
		/// <summary> Card specific control frames</summary>
		public const int CONTROL = 0x0016;
		/// <summary> Linux/IR</summary>
		public const int IRDA = 0x0017;
		// others not yet documented..
		
		public const int NS = 0x0600;
		public const int SPRITE = 0x0500;
		public const int TRAIL = 0x1000;
		public const int LANBRIDGE = 0x8038;
		public const int DECDNS = 0x803c;
		public const int DECDTS = 0x803e;
		public const int VEXP = 0x805b;
		public const int VPROD = 0x805c;
		public const int N8021Q = 0x8100;
		public const int PPP = 0x880b;
		public const int PPPOED = 0x8863;
		public const int PPPOES = 0x8864;
		public const int LOOPBACK = 0x9000;
		// spanning tree bridge protocol
		public const int STBPDU = 0x0026;
		// intel adapter fault tolerance heartbeats
		public const int INFTH = 0x886d;
		/// <summary> Ethernet protocol mask.</summary>
		public const int MASK = 0xffff;
	}
	public interface EthernetProtocols
	{
		//UPGRADE_NOTE: Members of interface 'EthernetProtocols' were extracted into structure 'EthernetProtocols_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
		
		
		// Non DIX types. Won't clash for 1500 types.
		
		
		// these aren't valid ETHERNET codes, but show up in the type field.
		
		
	}
}