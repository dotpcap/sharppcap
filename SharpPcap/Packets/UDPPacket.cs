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
    /// <summary> A UDP packet.
    /// <p>
    /// Extends an IP packet, adding a UDP header and UDP data payload.
    /// 
    /// </summary>
    [Serializable]
    public class UDPPacket : IPPacket, UDPFields
    {
        /// <summary> Fetch the port number on the source host.</summary>
        virtual public int SourcePort
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipOffset + UDPFields_Fields.UDP_SP_POS, UDPFields_Fields.UDP_PORT_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipOffset + UDPFields_Fields.UDP_SP_POS, UDPFields_Fields.UDP_PORT_LEN);
            }

        }

        /// <summary> Fetch the port number on the target host.</summary>
        virtual public int DestinationPort
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipOffset + UDPFields_Fields.UDP_DP_POS, UDPFields_Fields.UDP_PORT_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipOffset + UDPFields_Fields.UDP_DP_POS, UDPFields_Fields.UDP_PORT_LEN);
            }

        }
        /// <summary> Fetch the total length of the UDP packet, including header and
        /// data payload, in bytes.
        /// </summary>
        virtual public int UDPLength
        {
            get
            {
                // should produce the same value as header.length + data.length
                return Length;
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipOffset + UDPFields_Fields.UDP_LEN_POS, UDPFields_Fields.UDP_LEN_LEN);
            }

        }

        /// <summary> Fetch the header checksum.</summary>
        virtual public int UDPChecksum
        {
            get
            {
                return GetTransportLayerChecksum(_ipOffset + UDPFields_Fields.UDP_CSUM_POS);
            }

            set
            {
                SetTransportLayerChecksum(value, UDPFields_Fields.UDP_CSUM_POS);
            }

        }

        /// <summary> Check if the TCP packet is valid, checksum-wise.</summary>
        public override bool ValidChecksum
        {
            get
            {
                return ValidUDPChecksum;
            }

        }

        virtual public bool ValidUDPChecksum
        {
            get
            {
                return base.IsValidTransportLayerChecksum(true);
            }
        }

        /// <summary> Fetch the UDP header a byte array.</summary>
        virtual public byte[] UDPHeader
        {
            get
            {
                if (_udpHeaderBytes == null)
                {
                    _udpHeaderBytes = PacketEncoding.extractHeader(_ipOffset, UDPFields_Fields.UDP_HEADER_LEN, Bytes);
                }
                return _udpHeaderBytes;
            }

        }

        /// <summary> Fetch the UDP header as a byte array.</summary>
        override public byte[] Header
        {
            get
            {
                return UDPHeader;
            }
        }

        /// <summary> Fetch the UDP header length in bytes.</summary>
        virtual public int UDPHeaderLength
        {
            get
            {
                return UDPFields_Fields.UDP_HEADER_LEN;
            }
        }

        /// <summary> Fetches the packet header length.</summary>
        override public int HeaderLength
        {
            get
            {
                return UDPHeaderLength;
            }
        }

        /// <summary> Fetches the length of the payload data.</summary>
        virtual public int PayloadDataLength
        {
            get
            {
                return (IPPayloadLength - UDPHeaderLength);
            }
        }

        /// <summary> Fetch the UDP data as a byte array.</summary>
        virtual public byte[] UDPData
        {
            get
            {
                if (_udpDataBytes == null)
                {
                    _udpDataBytes = new byte[PayloadDataLength];
                    Array.Copy(Bytes, _ipOffset + UDPHeaderLength, _udpDataBytes, 0, PayloadDataLength);
                }
                return _udpDataBytes;
            }
            set
            {
                SetData(value);
            }

        }

        /// <summary>
        /// Sets the data section of this udp packet
        /// </summary>
        /// <param name="data">the data bytes</param>
        public void SetData(byte[] data)
        {
            if (IPVersion != IPVersions.IPv4)
                throw new System.NotImplementedException("IPVersion of " + IPVersion + " is unrecognized");

            byte[] headers = ArrayHelper.copy(Bytes, 0, UDPFields_Fields.UDP_HEADER_LEN + IPHeaderLength + EthernetHeaderLength);
            byte[] newBytes = ArrayHelper.join(headers, data);
            this.Bytes = newBytes;
            UDPLength = Bytes.Length - IPHeaderLength - EthernetHeaderLength;

            //update ip total length length
            IPTotalLength = IPHeaderLength + UDPFields_Fields.UDP_HEADER_LEN + data.Length;

            //update also offset and pcap header
            OnOffsetChanged();
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences_Fields.LIGHT_GREEN;
            }

        }
        /// <summary> Create a new UDP packet.</summary>
        public UDPPacket(int lLen, byte[] bytes)
            : base(lLen, bytes)
        {
        }

        /// <summary> Create a new UDP packet.</summary>
        public UDPPacket(int lLen, byte[] bytes, Timeval tv)
            : this(lLen, bytes)
        {
            this._timeval = tv;
        }

        /// <summary> Fetch the total length of the UDP packet, including header and
        /// data payload, in bytes.
        /// </summary>
        public int Length
        {
            get
            {
                // should produce the same value as header.length + data.length
                return ArrayHelper.extractInteger(Bytes, _ipOffset + UDPFields_Fields.UDP_LEN_POS, UDPFields_Fields.UDP_LEN_LEN);
            }
        }

        /// <summary> Fetch the header checksum.</summary>
        public int Checksum
        {
            get
            {
                return UDPChecksum;
            }
            set
            {
                UDPChecksum=value;
            }
        }

        /// <summary> Computes the UDP checksum, optionally updating the UDP checksum header.
        /// 
        /// </summary>
        /// <param name="update">Specifies whether or not to update the UDP checksum header
        /// after computing the checksum. A value of true indicates the
        /// header should be updated, a value of false indicates it should
        /// not be updated.
        /// </param>
        /// <returns> The computed UDP checksum.
        /// </returns>
        public int ComputeUDPChecksum(bool update)
        {
            if (IPVersion != IPVersions.IPv4)
                throw new System.NotImplementedException("IPVersion of " + IPVersion + " is unrecognized");

            // copy the udp section with data
            byte[] udp = IPData;
            // reset the checksum field (checksum is calculated when this field is
            // zeroed)
            ArrayHelper.insertLong(udp, 0, UDPFields_Fields.UDP_CSUM_POS, UDPFields_Fields.UDP_CSUM_LEN);
            //pseudo ip header should be attached to the udp+data
            udp = AttachPseudoIPHeader(udp);
            // compute the one's complement sum of the udp header
            int cs = ChecksumUtils.OnesComplementSum(udp);
            if (update)
            {
                UDPChecksum = cs;
            }

            return cs;
        }

        public int ComputeUDPChecksum()
        {
            return ComputeUDPChecksum(true);
        }

        private byte[] _udpHeaderBytes = null;

        private byte[] _udpDataBytes = null;

        /// <summary> Fetch the UDP data as a byte array.</summary>
        public override byte[] Data
        {
            get
            {
                return UDPData;
            }
        }

        /// <summary> Convert this UDP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this UDP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("UDPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences_Fields.RESET);
            buffer.Append(": ");
            buffer.Append(SourceAddress);
            buffer.Append('.');
            buffer.Append(IPPort.getName(SourcePort));
            buffer.Append(" -> ");
            buffer.Append(DestinationAddress);
            buffer.Append('.');
            buffer.Append(IPPort.getName(DestinationPort));
            buffer.Append(" l=" + UDPFields_Fields.UDP_HEADER_LEN + "," + (Length - UDPFields_Fields.UDP_HEADER_LEN));
            buffer.Append(']');

            return buffer.ToString();
        }
    }
}
