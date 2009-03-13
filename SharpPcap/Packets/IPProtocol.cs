/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using SharpPcap.Packets.Util;
namespace SharpPcap.Packets
{
    /// <summary> IPProtocol utility class.
    /// 
    /// </summary>
    public class IPProtocol
    {
        public enum IPProtocolType
        {
            /// <summary> Dummy protocol for TCP. </summary>
            IP = 0,
            /// <summary> IPv6 Hop-by-Hop options. </summary>
            HOPOPTS = 0,
            /// <summary> Internet Control Message Protocol. </summary>
            ICMP = 1,
            /// <summary> Internet Group Management Protocol.</summary>
            IGMP = 2,
            /// <summary> IPIP tunnels (older KA9Q tunnels use 94). </summary>
            IPIP = 4,
            /// <summary> Transmission Control Protocol. </summary>
            TCP = 6,
            /// <summary> Exterior Gateway Protocol. </summary>
            EGP = 8,
            /// <summary> PUP protocol. </summary>
            PUP = 12,
            /// <summary> User Datagram Protocol. </summary>
            UDP = 17,
            /// <summary> XNS IDP protocol. </summary>
            IDP = 22,
            /// <summary> SO Transport Protocol Class 4. </summary>
            TP = 29,
            /// <summary> IPv6 header. </summary>
            IPV6 = 41,
            /// <summary> IPv6 routing header. </summary>
            ROUTING = 43,
            /// <summary> IPv6 fragmentation header. </summary>
            FRAGMENT = 44,
            /// <summary> Reservation Protocol. </summary>
            RSVP = 46,
            /// <summary> General Routing Encapsulation. </summary>
            GRE = 47,
            /// <summary> encapsulating security payload. </summary>
            ESP = 50,
            /// <summary> authentication header. </summary>
            AH = 51,
            /// <summary> ICMPv6. </summary>
            ICMPV6 = 58,
            /// <summary> IPv6 no next header. </summary>
            NONE = 59,
            /// <summary> IPv6 destination options. </summary>
            DSTOPTS = 60,
            /// <summary> Multicast Transport Protocol. </summary>
            MTP = 92,
            /// <summary> Encapsulation Header. </summary>
            ENCAP = 98,
            /// <summary> Protocol Independent Multicast. </summary>
            PIM = 103,
            /// <summary> Compression Header Protocol. </summary>
            COMP = 108,
            /// <summary> Raw IP packets. </summary>
            RAW = 255,

            /// <summary> Unrecognized IP protocol.
            /// WARNING: this only works because the int storage for the protocol
            /// code has more bits than the field in the IP header where it is stored.
            /// </summary>
            INVALID = -1,

            /// <summary> IP protocol mask.</summary>
            MASK = 0xff
        }

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
                return (System.String) messages[c];
            } else
            {
                return "unknown";
            }
        }

        /// <summary> 'Human-readable' IP protocol descriptions.</summary>
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
            IPPacket.IPVersions ipVer = ExtractVersion(lLen, packetBytes);
            int protoOffset;

            switch (ipVer)
            {
                case IPPacket.IPVersions.IPv4:
                    protoOffset = IPv4Fields_Fields.IP_CODE_POS;
                    break;
                case IPPacket.IPVersions.IPv6:
                    protoOffset = IPv6Fields_Fields.NEXT_HEADER_POS;
                    break;
                default:
                    return -1;//unknown ip version
            }
            return packetBytes[lLen + protoOffset];
        }

        public static IPPacket.IPVersions ExtractVersion(int lLen, byte[] packetBytes)
        {
            return (IPPacket.IPVersions)((ArrayHelper.extractInteger(packetBytes,
                                                            lLen + IPv4Fields_Fields.IP_VER_POS,
                                                            IPv4Fields_Fields.IP_VER_LEN) >> 4) & 0xf);
        }

        static IPProtocol()
        {
            {
                messages[(System.Int32) IPProtocolType.IP] = "Dummy protocol for TCP";
                messages[(System.Int32) IPProtocolType.HOPOPTS] = "IPv6 Hop-by-Hop options";
                messages[(System.Int32) IPProtocolType.ICMP] = "Internet Control Message Protocol";
                messages[(System.Int32) IPProtocolType.IGMP] = "Internet Group Management Protocol";
                messages[(System.Int32) IPProtocolType.IPIP] = "IPIP tunnels";
                messages[(System.Int32) IPProtocolType.TCP] = "Transmission Control Protocol";
                messages[(System.Int32) IPProtocolType.EGP] = "Exterior Gateway Protocol";
                messages[(System.Int32) IPProtocolType.PUP] = "PUP protocol";
                messages[(System.Int32) IPProtocolType.UDP] = "User Datagram Protocol";
                messages[(System.Int32) IPProtocolType.IDP] = "XNS IDP protocol";
                messages[(System.Int32) IPProtocolType.TP] = "SO Transport Protocol Class 4";
                messages[(System.Int32) IPProtocolType.IPV6] = "IPv6 header";
                messages[(System.Int32) IPProtocolType.ROUTING] = "IPv6 routing header";
                messages[(System.Int32) IPProtocolType.FRAGMENT] = "IPv6 fragmentation header";
                messages[(System.Int32) IPProtocolType.RSVP] = "Reservation Protocol";
                messages[(System.Int32) IPProtocolType.GRE] = "General Routing Encapsulation";
                messages[(System.Int32) IPProtocolType.ESP] = "encapsulating security payload";
                messages[(System.Int32) IPProtocolType.AH] = "authentication header";
                messages[(System.Int32) IPProtocolType.ICMPV6] = "ICMPv6";
                messages[(System.Int32) IPProtocolType.NONE] = "IPv6 no next header";
                messages[(System.Int32) IPProtocolType.DSTOPTS] = "IPv6 destination options";
                messages[(System.Int32) IPProtocolType.MTP] = "Multicast Transport Protocol";
                messages[(System.Int32) IPProtocolType.ENCAP] = "Encapsulation Header";
                messages[(System.Int32) IPProtocolType.PIM] = "Protocol Independent Multicast";
                messages[(System.Int32) IPProtocolType.COMP] = "Compression Header Protocol";
                messages[(System.Int32) IPProtocolType.RAW] = "Raw IP Packet";
//              messages[(System.Int32) IPProtocolType.INVALID] = "INVALID IP";
            }
        }
    }
}
