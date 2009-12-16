// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************
// Copyright 2009 Chris Morgan <cmorgan@alum.wpi.edu>
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
        public static Packet dataToPacket(LinkLayers linkType, byte[] bytes)
        {
            return dataToPacket(linkType, bytes, new Timeval(0, 0));
        }

        /// <summary> Convert captured packet data into an object.</summary>
        public static Packet dataToPacket(LinkLayers linkType, byte[] bytes, Timeval tv)
        {
            EthernetPacketType ethProtocol;

            // retrieve the length of the headers associated with this link layer type.
            // this length is the offset to the header embedded in the packet.
            int byteOffsetToEthernetPayload = LinkLayer.LinkLayerLength(linkType);

            // extract the protocol code for the type of header embedded in the 
            // link-layer of the packet
            int offset = LinkLayer.ProtocolOffset(linkType);
            if (offset == -1)
            {
                // if there is no embedded protocol, assume IpV4
                ethProtocol = EthernetPacketType.IpV4;
            } else
            {
                ethProtocol = (EthernetPacketType)ArrayHelper.extractInteger(bytes, offset, EthernetFields_Fields.ETH_CODE_LEN);
            }

            string errorString;
            Packet parsedPacket = null;
            try
            {
                // try to recognize the ethernet type..
                switch (ethProtocol)
                {
                    // arp
                case EthernetPacketType.Arp:
                       parsedPacket = new ARPPacket(byteOffsetToEthernetPayload, bytes, tv);
                       break;

                case EthernetPacketType.IpV6:
                case EthernetPacketType.IpV4:
                        try
                        {
                            // ethernet level code is recognized as IP, figure out what kind..
                            int ipProtocol = IPProtocol.extractProtocol(byteOffsetToEthernetPayload, bytes);
                            switch (ipProtocol)
                            {
                                case (int)IPProtocol.IPProtocolType.ICMP:
                                    parsedPacket = new ICMPPacket(byteOffsetToEthernetPayload, bytes, tv);
                                    break;
    
                                case (int)IPProtocol.IPProtocolType.IGMP:
                                    parsedPacket = new IGMPPacket(byteOffsetToEthernetPayload, bytes, tv);
                                    break;
    
                                case (int)IPProtocol.IPProtocolType.TCP:
                                    parsedPacket = new TCPPacket(byteOffsetToEthernetPayload, bytes, tv);
                                    break;
    
                                case (int)IPProtocol.IPProtocolType.UDP:
                                    parsedPacket = new UDPPacket(byteOffsetToEthernetPayload, bytes, tv);
                                    break;
    
                                // unidentified ip..
                                default:
                                    parsedPacket = new IPPacket(byteOffsetToEthernetPayload, bytes, tv);
                                    break;
                            }

                            // check that the parsed packet is valid
                            if(!parsedPacket.IsValid(out errorString))
                            {
                                throw new PcapException(errorString);
                            } else
                            {
                                return parsedPacket;
                            }
                        } catch
                        {
                            // error parsing the specific ip packet type, parse as a generic IPPacket
                            parsedPacket = new IPPacket(byteOffsetToEthernetPayload, bytes, tv);

                            // check that the parsed packet is valid
                            if(!parsedPacket.IsValid(out errorString))
                            {
                                throw new PcapException(errorString);
                            } else
                            {
                                return parsedPacket;
                            }
                        }

                    // ethernet level code not recognized, default to anonymous packet..
                    default:
                        parsedPacket = new EthernetPacket(byteOffsetToEthernetPayload, bytes, tv);
                        break;
                }

                return parsedPacket;
            } catch
            {
                // we know we have at least an ethernet packet, so return that
                return new EthernetPacket(byteOffsetToEthernetPayload, bytes, tv);
            }
        }
    }
}