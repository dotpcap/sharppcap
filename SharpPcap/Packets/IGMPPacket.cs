// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************
using System;
using AnsiEscapeSequences_Fields = SharpPcap.Packets.Util.AnsiEscapeSequences_Fields;
using ArrayHelper = SharpPcap.Packets.Util.ArrayHelper;
using Timeval = SharpPcap.Packets.Util.Timeval;

namespace SharpPcap.Packets
{
    /// <summary> An IGMP packet.
    /// <p>
    /// Extends an IP packet, adding an IGMP header and IGMP data payload.
    /// </p>
    /// </summary>
    [Serializable]
    public class IGMPPacket : IPPacket, IGMPFields
    {
        /// <summary> Fetch the IGMP header a byte array.</summary>
        virtual public byte[] IGMPHeader
        {
            get
            {
                if (_igmpHeaderBytes == null)
                {
                    _igmpHeaderBytes = PacketEncoding.extractHeader(_ethPayloadOffset, IGMPFields_Fields.IGMP_HEADER_LEN, Bytes);
                }
                return _igmpHeaderBytes;
            }

        }
        /// <summary> Fetch the IGMP header as a byte array.</summary>
        override public byte[] Header
        {
            get
            {
                return IGMPHeader;
            }
        }

        /// <summary> Fetch the IGMP data as a byte array.</summary>
        virtual public byte[] IGMPData
        {
            get
            {
                if (_igmpDataBytes == null)
                {
                    // set data length based on info in headers (note: tcpdump
                    //  can return extra junk bytes which bubble up to here
                    int dataLen = Bytes.Length - _ethPayloadOffset - IGMPFields_Fields.IGMP_HEADER_LEN;

                    _igmpDataBytes = PacketEncoding.extractData(_ethPayloadOffset, IGMPFields_Fields.IGMP_HEADER_LEN, Bytes, dataLen);
                }
                return _igmpDataBytes;
            }

        }
        /// <summary> Fetch the IGMP message type, including subcode. Return value can be 
        /// used with IGMPMessage.getDescription().
        /// </summary>
        /// <returns> a 2-byte value containing the message type in the high byte
        /// and the message type subcode in the low byte.
        /// </returns>
        virtual public int MessageType
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + IGMPFields_Fields.IGMP_CODE_POS, IGMPFields_Fields.IGMP_CODE_LEN);
            }

        }
        /// <summary> Fetch the IGMP max response time.</summary>
        virtual public int MaxResponseTime
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + IGMPFields_Fields.IGMP_MRSP_POS, IGMPFields_Fields.IGMP_MRSP_LEN);
            }
        }

        /// <summary> Fetch the IGMP header checksum.</summary>
        virtual public int IGMPChecksum
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + IGMPFields_Fields.IGMP_CSUM_POS, IGMPFields_Fields.IGMP_CSUM_LEN);
            }

        }
        /// <summary> Fetch the IGMP group address.</summary>
        virtual public System.Net.IPAddress GroupAddress
        {
            get
            {
                return IPPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             _ipPayloadOffset + IGMPFields_Fields.IGMP_GADDR_POS,
                                             Bytes);
            }

        }
        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences_Fields.BROWN;
            }

        }
        public IGMPPacket(int lLen, byte[] bytes)
            : base(lLen, bytes)
        {
        }

        public IGMPPacket(int lLen, byte[] bytes, Timeval tv)
            : this(lLen, bytes)
        {
            this._timeval = tv;
        }

        private byte[] _igmpHeaderBytes = null;

        private byte[] _igmpDataBytes = null;

        /// <summary> Fetch the IGMP data as a byte array.</summary>
        public override byte[] Data
        {
            get
            {
                return IGMPData;
            }
        }

        /// <summary> Fetch the IGMP header checksum.</summary>
        public int Checksum
        {
            get
            {
                return IGMPChecksum;
            }
        }


        /// <summary> Convert this IGMP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this IGMP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("IGMPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences_Fields.RESET);
            buffer.Append(": ");
            buffer.Append(IGMPMessage.getDescription(MessageType));
            buffer.Append(", ");
            buffer.Append(GroupAddress + ": ");
            buffer.Append(SourceAddress + " -> " + DestinationAddress);
            buffer.Append(" l=" + IGMPFields_Fields.IGMP_HEADER_LEN + "," + (Bytes.Length - _ipPayloadOffset - IGMPFields_Fields.IGMP_HEADER_LEN));
            buffer.Append(']');

            return buffer.ToString();
        }
    }
}