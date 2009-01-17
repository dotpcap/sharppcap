/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace SharpPcap.Packets
{
	/// <summary> IPProtocol utility class.
	/// 
	/// </summary>
	public class IPProtocol : IPProtocols
	{
		/// <summary> Fetch a protocol description.</summary>
		/// <param name="code">the code associated with the message.
		/// </param>
		/// <returns> a message describing the significance of the IP protocol.
		/// </returns>
		public static System.String getDescription(int code)
		{
			System.Int32 c = (System.Int32) code;
			if (messages.ContainsKey(c))
			{
				//UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
				return (System.String) messages[c];
			}
			else
				return "unknown";
		}
		
		/// <summary> 'Human-readable' IP protocol descriptions.</summary>
		//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
		private static System.Collections.Hashtable messages = new System.Collections.Hashtable();
		
		/// <summary> Extract the protocol code from packet data. The packet data 
		/// must contain an IP datagram.
		/// The protocol code specifies what kind of information is contained in the 
		/// data block of the ip datagram.
		/// 
		/// </summary>
		/// <param name="lLen">the length of the link-level header.
		/// </param>
		/// <param name="packetBytes">packet bytes, including the link-layer header.
		/// </param>
		/// <returns> the IP protocol code. i.e. 0x06 signifies TCP protocol.
		/// </returns>
		public static int extractProtocol(int lLen, byte[] packetBytes)
		{
			return packetBytes[lLen + IPFields_Fields.IP_CODE_POS];
		}
		
		static IPProtocol()
		{
			{
				messages[(System.Int32) IPProtocols_Fields.IP] = "Dummy protocol for TCP";
				messages[(System.Int32) IPProtocols_Fields.HOPOPTS] = "IPv6 Hop-by-Hop options";
				messages[(System.Int32) IPProtocols_Fields.ICMP] = "Internet Control Message Protocol";
				messages[(System.Int32) IPProtocols_Fields.IGMP] = "Internet Group Management Protocol";
				messages[(System.Int32) IPProtocols_Fields.IPIP] = "IPIP tunnels";
				messages[(System.Int32) IPProtocols_Fields.TCP] = "Transmission Control Protocol";
				messages[(System.Int32) IPProtocols_Fields.EGP] = "Exterior Gateway Protocol";
				messages[(System.Int32) IPProtocols_Fields.PUP] = "PUP protocol";
				messages[(System.Int32) IPProtocols_Fields.UDP] = "User Datagram Protocol";
				messages[(System.Int32) IPProtocols_Fields.IDP] = "XNS IDP protocol";
				messages[(System.Int32) IPProtocols_Fields.TP] = "SO Transport Protocol Class 4";
				messages[(System.Int32) IPProtocols_Fields.IPV6] = "IPv6 header";
				messages[(System.Int32) IPProtocols_Fields.ROUTING] = "IPv6 routing header";
				messages[(System.Int32) IPProtocols_Fields.FRAGMENT] = "IPv6 fragmentation header";
				messages[(System.Int32) IPProtocols_Fields.RSVP] = "Reservation Protocol";
				messages[(System.Int32) IPProtocols_Fields.GRE] = "General Routing Encapsulation";
				messages[(System.Int32) IPProtocols_Fields.ESP] = "encapsulating security payload";
				messages[(System.Int32) IPProtocols_Fields.AH] = "authentication header";
				messages[(System.Int32) IPProtocols_Fields.ICMPV6] = "ICMPv6";
				messages[(System.Int32) IPProtocols_Fields.NONE] = "IPv6 no next header";
				messages[(System.Int32) IPProtocols_Fields.DSTOPTS] = "IPv6 destination options";
				messages[(System.Int32) IPProtocols_Fields.MTP] = "Multicast Transport Protocol";
				messages[(System.Int32) IPProtocols_Fields.ENCAP] = "Encapsulation Header";
				messages[(System.Int32) IPProtocols_Fields.PIM] = "Protocol Independent Multicast";
				messages[(System.Int32) IPProtocols_Fields.COMP] = "Compression Header Protocol";
				messages[(System.Int32) IPProtocols_Fields.RAW] = "Raw IP Packet";
				messages[(System.Int32) IPProtocols_Fields.INVALID] = "INVALID IP";
			}
		}
	}
}
