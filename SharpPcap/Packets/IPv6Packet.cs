/// <summary>
///   David Bond 1/18/2009
///   Mokon@Mokon.Net
///   www.Mokon.Net
/// License
///   Distributed under the Mozilla Public License
///   http://www.mozilla.org/NPL/MPL-1.1.txt
/// </summary>
using System;
using System.IO;
using AnsiEscapeSequences_Fields = SharpPcap.Packets.Util.AnsiEscapeSequences_Fields;
using ArrayHelper = SharpPcap.Packets.Util.ArrayHelper;
using Timeval = SharpPcap.Packets.Util.Timeval;

namespace SharpPcap.Packets
{
    /// <summary>
    /// This is a implementation of the IPv6 Layer as an object for use with the SharpPcap library.
    /// 
    /// References
    /// ----------
    /// http://tools.ietf.org/html/rfc2460
    /// http://en.wikipedia.org/wiki/IPv6
    /// </summary>
    public class IPv6Packet : EthernetPacket
    {
        public static int ipVersion = 6;

        /// <summary>
        /// Constructor with a byte array and the size of the link layer.
        /// </summary>
        /// <param name="lLen">The link layer size</param>
        /// <param name="bytes">A byte array.</param>
        public IPv6Packet( int lLen, byte[] bytes )
                    : base( lLen, bytes )
        {
        }

        /// <summary>
        /// Constructor with a byte array, a time value, and the size of the link layer.
        /// </summary>
        /// <param name="lLen">The link layer size.</param>
        /// <param name="bytes">A byte array.</param>
        /// <param name="tv">A time value.</param>
        public IPv6Packet( int lLen, byte[] bytes, Timeval tv )
                            : this( lLen, bytes )
        {
            this._timeval = tv;
        }

        /// <summary>
        /// TODO No Idea what this is for.
        /// </summary>
        public override void OnOffsetChanged( )
        {
            base.OnOffsetChanged( );
          _ipv6Offset = _ethOffset + IPv6Fields_Fields.IPv6_HEADER_LEN;
        }

        /// <summary>
        /// The start of the ipv6 packet.
        /// </summary>
        protected internal int _ipv6Offset;

        /// <summary>
        /// The version field of the IPv6 Packet.
        /// </summary>
        public int IPv6Version
        {
            get
            {
                return (ArrayHelper.extractInteger(Bytes, _ethOffset + IPv6Fields_Fields.LINE_ONE_POS,
                         IPv6Fields_Fields.LINE_ONE_LEN ) >> 28 ) & 0x0F;
            }

            set
            {
                ulong org = ((ulong)ArrayHelper.extractLong(Bytes, _ethOffset + IPv6Fields_Fields.LINE_ONE_POS,
                                IPv6Fields_Fields.LINE_ONE_LEN ) & 0x0FFFFFFF ) | ( ( ( (ulong)value ) << 28 ) & 0xF0000000 );
                ArrayHelper.insertLong(Bytes, (long)org, _ethOffset + IPv6Fields_Fields.LINE_ONE_POS, 4);
            }
        }

        /// <summary>
        /// The version field of the IPv6 Packet. Delgates to IPv6Version so maybe overridden.
        /// </summary>
        public virtual int Version
        {
            get
            {
                return IPv6Version;
            }

            set
            {
                IPv6Version = value;
            }
        }

        /// <summary>
        /// The traffic class field of the IPv6 Packet.
        /// </summary>
        public virtual int TrafficClass
        {
            get
            {
                return (ArrayHelper.extractInteger(Bytes, _ethOffset + IPv6Fields_Fields.LINE_ONE_POS,
                         IPv6Fields_Fields.LINE_ONE_LEN ) >> 20 ) & 0xFF;
            }

            set
            {
                ulong org = ((ulong)ArrayHelper.extractLong(Bytes, _ethOffset + IPv6Fields_Fields.LINE_ONE_POS,
                              IPv6Fields_Fields.LINE_ONE_LEN ) & 0xF00FFFFF ) | ( ( ( (ulong)value ) << 20 ) & 0x0FF00000 );
                ArrayHelper.insertLong(Bytes, (long)org, _ethOffset + IPv6Fields_Fields.LINE_ONE_POS, 4);
            }
        }

        /// <summary>
        /// The flow label field of the IPv6 Packet.
        /// </summary>
        public virtual int FlowLabel
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ethOffset + IPv6Fields_Fields.LINE_ONE_POS,
                                                   IPv6Fields_Fields.LINE_ONE_LEN ) & 0xFFFFF;
            }

            set
            {
                ulong org = ((ulong)ArrayHelper.extractLong(Bytes, _ethOffset + IPv6Fields_Fields.LINE_ONE_POS,
                             IPv6Fields_Fields.LINE_ONE_LEN ) & 0xFFF00000 ) | ( ( (ulong)value ) & 0x000FFFFF );
                ArrayHelper.insertLong(Bytes, (long)org, _ethOffset + IPv6Fields_Fields.LINE_ONE_POS, 4);
            }
        }

        /// <summary>
        /// The payload lengeth field of the IPv6 Packet
        /// NOTE: Differs from the IPv4 'Total length' field that includes the length of the header as
        ///       payload length is ONLY the size of the payload.
        /// </summary>
        public virtual int IPPayloadLength
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ethOffset + IPv6Fields_Fields.PAYLOAD_LENGTH_POS,
                                                  IPv6Fields_Fields.PAYLOAD_LENGTH_LEN );
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ethOffset + IPv6Fields_Fields.PAYLOAD_LENGTH_POS,
                                       IPv6Fields_Fields.PAYLOAD_LENGTH_LEN );
            }
        }

        /// <summary>
        /// The next header field of the IPv6 Packet.
        /// 
        /// Replaces IPv4's 'protocol' field, has compatible values
        /// </summary>
        public virtual IPProtocol.IPProtocolType NextHeader
        {
            get
            {
                return (IPProtocol.IPProtocolType)ArrayHelper.extractInteger(Bytes,
                                                                             _ethOffset + IPv6Fields_Fields.NEXT_HEADER_POS,
                                                                             IPv6Fields_Fields.NEXT_HEADER_LEN );
            }

            set
            {
                Bytes[_ethOffset + IPv6Fields_Fields.NEXT_HEADER_POS] = (byte)value;
            }
        }

        /// <summary>
        /// The hop limit field of the IPv6 Packet.
        /// NOTE: Replaces the 'time to live' field of IPv4
        /// 
        /// 8-bit value
        /// </summary>
        public virtual int HopLimit
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ethOffset + IPv6Fields_Fields.HOP_LIMIT_POS,
                                                  IPv6Fields_Fields.HOP_LIMIT_LEN );
            }

            set
            {
                Bytes[_ethOffset + IPv6Fields_Fields.HOP_LIMIT_POS] = (byte)value;
            }
        }

        /// <summary>
        /// The source address field of the IPv6 Packet.
        /// </summary>
        public virtual System.Net.IPAddress SourceAddress
        {
            get
            {
                return IPPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetworkV6,
                                             _ethOffset + IPv6Fields_Fields.SRC_ADDRESS_POS,
                                             Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0, Bytes, _ethOffset + IPv6Fields_Fields.SRC_ADDRESS_POS, address.Length);
            }
        }

        /// <summary>
        /// The destination address field of the IPv6 Packet.
        /// </summary>
        public virtual System.Net.IPAddress DestinationAddress
        {
            get
            {
                return IPPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetworkV6,
                                             _ethOffset + IPv6Fields_Fields.DST_ADDRESS_POS,
                                             Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0, Bytes, _ethOffset + IPv6Fields_Fields.DST_ADDRESS_POS, address.Length);
            }
        }

        /// <summary>
        /// Returns the bytes of the IPv6 Header.
        /// </summary>
        public virtual byte[] IPv6Header
        {
            get
            {
                return PacketEncoding.extractHeader(_ethOffset, IPv6Fields_Fields.IPv6_HEADER_LEN, Bytes);
            }
        }

        /// <summary>
        /// Returns the bytes of the IPv6 Header.
        /// </summary>
        override public byte[] Header
        {
            get
            {
                return IPv6Header;
            }
        }

        /// <summary>
        /// Returns the IP data.
        /// </summary>
        virtual public byte[] IPData
        {
            get
            {
                return PacketEncoding.extractData(_ethOffset, IPv6Fields_Fields.IPv6_HEADER_LEN, Bytes,
                                                  IPPayloadLength );
            }
        }

        /// <summary>
        /// Returns the IP data.
        /// </summary>
        public override byte[] Data
        {
            get
            {
                return IPData;
            }
        }

        // Prepend to the given byte[] origHeader the portion of the IPv6 header used for
        // generating an tcp checksum
        //
        // http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_checksum_using_IPv6
        // http://tools.ietf.org/html/rfc2460#page-27
        protected internal virtual byte[] AttachPseudoIPHeader(byte[] origHeader)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            // 0-16: ip src addr
            bw.Write(Bytes, _ethOffset + IPv6Fields_Fields.SRC_ADDRESS_POS, IPv6Fields_Fields.SRC_ADDRESS_LEN);

            // 17-32: ip dst addr
            bw.Write(Bytes, _ethOffset + IPv6Fields_Fields.DST_ADDRESS_POS, IPv6Fields_Fields.DST_ADDRESS_LEN);

            // 33-36: TCP length
            bw.Write( (UInt32) SharpPcap.Util.IPUtil.Hton( origHeader.Length ) );

            // 37-39: 3 bytes of zeros
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write((byte)0);

            // 40: Next header
            bw.Write((byte)NextHeader);

            // prefix the pseudoHeader to the header+data
            byte[] header = ms.ToArray();
            int headerSize = header.Length + origHeader.Length; 
            bool odd = origHeader.Length % 2 != 0;
            if (odd)
                headerSize++;

            byte[] finalData = new byte[headerSize];

            // copy the pseudo header in
            Array.Copy(header, 0, finalData, 0, header.Length);

            // copy the origHeader in
            Array.Copy(origHeader, 0, finalData, header.Length, origHeader.Length);

            //if not even length, pad with a zero
            if (odd)
                finalData[finalData.Length - 1] = 0;

            return finalData;
        }

        /// <summary>
        /// Converts the packet to a string.
        /// </summary>
        /// <returns></returns>
        public override String ToString( )
        {
            return base.ToString( ) + "\r\nIPv6 Packet [\r\n"
                   + "\tIPv6 Source Address: " + SourceAddress.ToString() + ", \r\n"
                   + "\tIPv6 Destination Address: " + DestinationAddress.ToString() + "\r\n"
                   + "]";
            // TODO Implement Better ToString
        }

        /// <summary> Generate string with contents describing this IP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("IPv6Packet");
            if (colored)
                buffer.Append(AnsiEscapeSequences_Fields.RESET);
            buffer.Append(": ");
            buffer.Append(SourceAddress + " -> " + DestinationAddress);
            buffer.Append(" next header=" + NextHeader);
            buffer.Append(" l=" + this.IPPayloadLength);
            buffer.Append(" sum=" + this.IPPayloadLength);
            buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary> Convert this IP packet to a more verbose string.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Converts the packet to a color string. TODO add a method for colored to string.
        /// </summary>
        override public String Color
        {
            get
            {
                return AnsiEscapeSequences_Fields.WHITE;
            }
        }
    }
}
