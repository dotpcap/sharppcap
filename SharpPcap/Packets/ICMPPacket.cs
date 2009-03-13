/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
using AnsiEscapeSequences_Fields = SharpPcap.Packets.Util.AnsiEscapeSequences_Fields;
using ArrayHelper = SharpPcap.Packets.Util.ArrayHelper;
using Timeval = SharpPcap.Packets.Util.Timeval;

namespace SharpPcap.Packets
{
    /// <summary> An ICMP packet.
    /// <p>
    /// Extends an IP packet, adding an ICMP header and ICMP data payload.
    /// 
    /// </summary>
    [Serializable]
    public class ICMPPacket : IPPacket, ICMPFields
    {
        /// <summary> Fetch the ICMP header a byte array.</summary>
        virtual public byte[] ICMPHeader
        {
            get
            {
                return PacketEncoding.extractHeader(_ipOffset, ICMPFields_Fields.ICMP_HEADER_LEN, Bytes);
            }
        }

        /// <summary> Fetch the ICMP header as a byte array.</summary>
        override public byte[] Header
        {
            get
            {
                return ICMPHeader;
            }
        }

        /// <summary> Fetch the ICMP data as a byte array.</summary>
        virtual public byte[] ICMPData
        {
            get
            {
                int dataLen = Bytes.Length - _ipOffset - ICMPFields_Fields.ICMP_HEADER_LEN;
                return PacketEncoding.extractData(_ipOffset, ICMPFields_Fields.ICMP_HEADER_LEN, Bytes, dataLen);
            }
        }

        /// <summary> Fetch the ICMP message type code. Formerly .getMessageType().</summary>
        virtual public int MessageMajorCode
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipOffset + ICMPFields_Fields.ICMP_CODE_POS, ICMPFields_Fields.ICMP_CODE_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipOffset + ICMPFields_Fields.ICMP_CODE_POS, ICMPFields_Fields.ICMP_CODE_LEN);
            }
        }

        /// <deprecated> use getMessageMajorCode().
        /// </deprecated>
        virtual public int MessageType
        {
            get
            {
                return MessageMajorCode;
            }

            set
            {
                MessageMajorCode = value;
            }
        }

        /// <summary> Fetch the ICMP message type, including subcode. Return value can be 
        /// used with ICMPMessage.getDescription().
        /// </summary>
        /// <returns> a 2-byte value containing the message type in the high byte
        /// and the message type subcode in the low byte.
        /// </returns>
        virtual public int MessageCode
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipOffset + ICMPFields_Fields.ICMP_SUBC_POS, ICMPFields_Fields.ICMP_SUBC_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipOffset + ICMPFields_Fields.ICMP_SUBC_POS, ICMPFields_Fields.ICMP_SUBC_LEN);
            }
        }

        /// <summary> Fetch the ICMP message subcode.</summary>
        virtual public int MessageMinorCode
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipOffset + ICMPFields_Fields.ICMP_CODE_POS + 1, ICMPFields_Fields.ICMP_CODE_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipOffset + ICMPFields_Fields.ICMP_CODE_POS + 1, ICMPFields_Fields.ICMP_CODE_LEN);
            }

        }
        //UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
        /// <summary> Fetch the ICMP header checksum.</summary>
        /// <summary> Sets the ICMP header checksum.</summary>
        virtual public int ICMPChecksum
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipOffset + ICMPFields_Fields.ICMP_CSUM_POS, ICMPFields_Fields.ICMP_CSUM_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipOffset + ICMPFields_Fields.ICMP_CSUM_POS, ICMPFields_Fields.ICMP_CSUM_LEN);
            }
        }

        virtual public bool ValidICMPChecksum
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences_Fields.LIGHT_BLUE;
            }
        }

        public ICMPPacket(int lLen, byte[] bytes)
            : base(lLen, bytes)
        {
        }

        public ICMPPacket(int lLen, byte[] bytes, Timeval tv)
            : this(lLen, bytes)
        {
            this._timeval = tv;
        }

        /// <summary> Fetch the ICMP data as a byte array.</summary>
        public override byte[] Data
        {
            get
            {
                return ICMPData;
            }
        }

        /// <summary> Fetch the ICMP header checksum.</summary>
        public int Checksum
        {
            get
            {
                return ICMPChecksum;
            }
            set
            {
                ICMPChecksum = value;
            }
        }

        /// <summary> Computes the ICMP checksum, optionally updating the ICMP checksum header.
        /// 
        /// </summary>
        /// <param name="update">Specifies whether or not to update the ICMP checksum
        /// header after computing the checksum.  A value of true indicates
        /// the header should be updated, a value of false indicates it
        /// should not be updated.
        /// </param>
        /// <returns> The computed ICMP checksum.
        /// </returns>
        public int ComputeICMPChecksum(bool update)
        {
            //return base.ComputeTransportLayerChecksum(ICMPFields_Fields.ICMP_CSUM_POS, update, false);
            throw new System.NotImplementedException();
        }

        /// <summary> Same as <code>computeICMPChecksum(true);</code>
        /// 
        /// </summary>
        /// <returns> The computed ICMP checksum value.
        /// </returns>
        public int ComputeICMPChecksum()
        {
            return ComputeICMPChecksum(true);
        }

        /// <summary> Convert this ICMP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this ICMP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("ICMPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences_Fields.RESET);
            buffer.Append(": ");
            buffer.Append(ICMPMessage.getDescription(MessageCode));
            buffer.Append(", ");
            buffer.Append(SourceAddress + " -> " + DestinationAddress);
            buffer.Append(" l=" + ICMPFields_Fields.ICMP_HEADER_LEN + "," + (Bytes.Length - _ipOffset - ICMPFields_Fields.ICMP_HEADER_LEN));
            buffer.Append(']');

            return buffer.ToString();
        }
    }
}
