// $Id: PacketFactory.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.ArrayHelper' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ArrayHelper = Tamir.IPLib.Packets.Util.ArrayHelper;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.Timeval' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using Timeval = Tamir.IPLib.Packets.Util.Timeval;
namespace Tamir.IPLib.Packets
{


	/// <summary> This factory constructs high-level packet objects from
	/// captured data streams.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:18 $ </lastModifiedAt>
	public class PacketFactory
	{
		/// <summary> Convert captured packet data into an object.</summary>
		public static Packet dataToPacket(int linkType, byte[] bytes)
		{
			int ethProtocol;

			// record the length of the headers associated with this link layer type.
			// this length is the offset to the header embedded in the packet.
			lLen = LinkLayer.getLinkLayerLength(linkType);

			// extract the protocol code for the type of header embedded in the 
			// link-layer of the packet
			int offset = LinkLayer.getProtoOffset(linkType);
			if (offset == -1)
				// if there is no embedded protocol, assume IP?
				ethProtocol = EthernetProtocols_Fields.IP;
			else
				ethProtocol = ArrayHelper.extractInteger(bytes, offset, EthernetFields_Fields.ETH_CODE_LEN);

			// try to recognize the ethernet type..
			switch (ethProtocol)
			{

				// arp
				case EthernetProtocols_Fields.ARP:
					return new ARPPacket(lLen, bytes);

				case EthernetProtocols_Fields.IP:
					// ethernet level code is recognized as IP, figure out what kind..
					int ipProtocol = IPProtocol.extractProtocol(lLen, bytes);
					switch (ipProtocol)
					{

						// icmp
						case IPProtocols_Fields.ICMP: return new ICMPPacket(lLen, bytes);
						// igmp

						case IPProtocols_Fields.IGMP: return new IGMPPacket(lLen, bytes);
						// tcp

						case IPProtocols_Fields.TCP: return new TCPPacket(lLen, bytes);
						// udp

						case IPProtocols_Fields.UDP: return new UDPPacket(lLen, bytes);
						// unidentified ip..

						default: return new IPPacket(lLen, bytes);

					}
					// ethernet level code not recognized, default to anonymous packet..
					//goto default;

				default: return new EthernetPacket(lLen, bytes);

			}
		}

		/// <summary> Convert captured packet data into an object.</summary>
		public static Packet dataToPacket(int linkType, byte[] bytes, Timeval tv)
		{
			int ethProtocol;

			// record the length of the headers associated with this link layer type.
			// this length is the offset to the header embedded in the packet.
			lLen = LinkLayer.getLinkLayerLength(linkType);

			// extract the protocol code for the type of header embedded in the 
			// link-layer of the packet
			int offset = LinkLayer.getProtoOffset(linkType);
			if (offset == -1)
				// if there is no embedded protocol, assume IP?
				ethProtocol = EthernetProtocols_Fields.IP;
			else
				ethProtocol = ArrayHelper.extractInteger(bytes, offset, EthernetFields_Fields.ETH_CODE_LEN);

			// try to recognize the ethernet type..
			switch (ethProtocol)
			{

				// arp
				case EthernetProtocols_Fields.ARP:
					return new ARPPacket(lLen, bytes, tv);

				case EthernetProtocols_Fields.IP:
					// ethernet level code is recognized as IP, figure out what kind..
					int ipProtocol = IPProtocol.extractProtocol(lLen, bytes);
					switch (ipProtocol)
					{

						// icmp
						case IPProtocols_Fields.ICMP: return new ICMPPacket(lLen, bytes, tv);
						// igmp

						case IPProtocols_Fields.IGMP: return new IGMPPacket(lLen, bytes, tv);
						// tcp

						case IPProtocols_Fields.TCP: return new TCPPacket(lLen, bytes, tv);
						// udp

						case IPProtocols_Fields.UDP: return new UDPPacket(lLen, bytes, tv);
						// unidentified ip..

						default: return new IPPacket(lLen, bytes, tv);

					}
					// ethernet level code not recognized, default to anonymous packet..
					//goto default;

				default: return new EthernetPacket(lLen, bytes, tv);

			}
		}


		/// <summary> Length in bytes of the link-level headers that this factory is 
		/// decoding packets for.
		/// </summary>
		private static int lLen;
	}
}