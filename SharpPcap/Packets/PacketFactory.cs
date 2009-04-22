/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// Copyright 2009 Chris Morgan <cmorgan@alum.wpi.edu>
/// </summary>
using ArrayHelper = SharpPcap.Packets.Util.ArrayHelper;
using Timeval = SharpPcap.Packets.Util.Timeval;
namespace SharpPcap.Packets
{
    /// <summary> This factory constructs high-level packet objects from
    /// captured data streams.
    /// </summary>
    public class PacketFactory
    {
        /// <summary> Convert captured packet data into an object.</summary>
        public static Packet dataToPacket(int linkType, byte[] bytes)
        {
            return dataToPacket(linkType, bytes, new Timeval(0, 0));
        }

        /// <summary> Convert captured packet data into an object.</summary>
        public static Packet dataToPacket(int linkType, byte[] bytes, Timeval tv)
        {
            int ethProtocol;

            // retrieve the length of the headers associated with this link layer type.
            // this length is the offset to the header embedded in the packet.
            int lLen = LinkLayer.getLinkLayerLength(linkType);

            // extract the protocol code for the type of header embedded in the 
            // link-layer of the packet
            int offset = LinkLayer.getProtoOffset(linkType);
            if (offset == -1)
            {
                // if there is no embedded protocol, assume IP?
                ethProtocol = EthernetPacket.EtherType.IP;
            } else
            {
                ethProtocol = ArrayHelper.extractInteger(bytes, offset, EthernetFields_Fields.ETH_CODE_LEN);
            }

            // try to recognize the ethernet type..
            switch (ethProtocol)
            {
                // arp
                case EthernetPacket.EtherType.ARP:
                    return new ARPPacket(lLen, bytes, tv);

                case EthernetPacket.EtherType.IPV6:
                case EthernetPacket.EtherType.IP:
                    // ethernet level code is recognized as IP, figure out what kind..
                    int ipProtocol = IPProtocol.extractProtocol(lLen, bytes);
                    switch (ipProtocol)
                    {
                        case (int)IPProtocol.IPProtocolType.ICMP: return new ICMPPacket(lLen, bytes, tv);
                        case (int)IPProtocol.IPProtocolType.IGMP: return new IGMPPacket(lLen, bytes, tv);
                        case (int)IPProtocol.IPProtocolType.TCP:  return new TCPPacket(lLen, bytes, tv);
                        case (int)IPProtocol.IPProtocolType.UDP:  return new UDPPacket(lLen, bytes, tv);

                        // unidentified ip..
                        default:
                            return new IPPacket(lLen, bytes, tv);
                    }

                // ethernet level code not recognized, default to anonymous packet..
                default:
                    return new EthernetPacket(lLen, bytes, tv);
            }
        }
    }
}