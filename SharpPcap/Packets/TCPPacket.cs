// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************
using System;
using System.Net;
using AnsiEscapeSequences_Fields = SharpPcap.Packets.Util.AnsiEscapeSequences_Fields;
using ArrayHelper = SharpPcap.Packets.Util.ArrayHelper;
using Timeval = SharpPcap.Packets.Util.Timeval;

namespace SharpPcap.Packets
{
    /// <summary> A TCP packet.
    /// <p>
    /// Extends an IP packet, adding a TCP header and TCP data payload.
    /// </p>
    /// </summary>
    [Serializable]
    public class TCPPacket : IPPacket
    {
        public const int HeaderMinimumLength = 20; // 20 bytes is the smallest tcp header

        /// <summary> Fetch the port number on the source host.</summary>
        virtual public int SourcePort
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + TCPFields_Fields.TCP_SP_POS, TCPFields_Fields.TCP_PORT_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipPayloadOffset + TCPFields_Fields.TCP_SP_POS, TCPFields_Fields.TCP_PORT_LEN);
            }
        }

        /// <summary> Fetches the port number on the destination host.</summary>
        virtual public int DestinationPort
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + TCPFields_Fields.TCP_DP_POS, TCPFields_Fields.TCP_PORT_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipPayloadOffset + TCPFields_Fields.TCP_DP_POS, TCPFields_Fields.TCP_PORT_LEN);
            }
        }

        /// <summary> Fetch the packet sequence number.</summary>
        virtual public long SequenceNumber
        {
            get
            {
                return ArrayHelper.extractLong(Bytes, _ipPayloadOffset + TCPFields_Fields.TCP_SEQ_POS, TCPFields_Fields.TCP_SEQ_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipPayloadOffset + TCPFields_Fields.TCP_SEQ_POS, TCPFields_Fields.TCP_SEQ_LEN);
            }
        }

        /// <summary>    Fetch the packet acknowledgment number.</summary>
        virtual public long AcknowledgmentNumber
        {
            get
            {
                return ArrayHelper.extractLong(Bytes, _ipPayloadOffset + TCPFields_Fields.TCP_ACK_POS, TCPFields_Fields.TCP_ACK_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipPayloadOffset + TCPFields_Fields.TCP_ACK_POS, TCPFields_Fields.TCP_ACK_LEN);
            }
        }

        /// <summary> Fetch the TCP header length in bytes.</summary>
        virtual public int TCPHeaderLength
        {
            get
            {
                return ((ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + TCPFields_Fields.TCP_FLAG_POS, TCPFields_Fields.TCP_FLAG_LEN) >> 12) & 0xf) * 4;
            }

            set
            {
                value = value / 4;
                Bytes[_ipPayloadOffset + TCPFields_Fields.TCP_FLAG_POS] &= (byte)(0x0f);
                Bytes[_ipPayloadOffset + TCPFields_Fields.TCP_FLAG_POS] |= (byte)(((value << 4) & 0xf0));
            }
        }

        /// <summary> Fetches the packet TCP header length.</summary>
        override public int HeaderLength
        {
            get
            {
                return TCPHeaderLength;
            }
        }

        /// <summary> Fetches the length of the payload data.</summary>
        virtual public int PayloadDataLength
        {
            get
            {
                return (IPPayloadLength - TCPHeaderLength);
            }
        }

        /// <summary> Fetch the window size.</summary>
        virtual public int WindowSize
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + TCPFields_Fields.TCP_WIN_POS, TCPFields_Fields.TCP_WIN_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipPayloadOffset + TCPFields_Fields.TCP_WIN_POS, TCPFields_Fields.TCP_WIN_LEN);
            }
        }

        /// <summary> Fetch the header checksum.</summary>
        /// <summary> Set the checksum of the TCP header</summary>
        /// <param name="cs">the checksum value
        /// </param>
        virtual public int TCPChecksum
        {
            get
            {
                return GetTransportLayerChecksum(_ipPayloadOffset + TCPFields_Fields.TCP_CSUM_POS);
            }

            set
            {
                base.SetTransportLayerChecksum(value, TCPFields_Fields.TCP_CSUM_POS);
            }

        }
        /// <summary> Check if the TCP packet is valid, checksum-wise.</summary>
        public override bool ValidChecksum
        {
            get
            {
                return ValidIPChecksum && ValidTCPChecksum;
            }

        }

        virtual public bool ValidTCPChecksum
        {
            get
            {
                return base.IsValidTransportLayerChecksum(true);
            }
        }

        /// <returns> The TCP packet length in bytes.  This is the size of the
        /// IP packet minus the size of the IP header.
        /// </returns>
        virtual public int TCPPacketByteLength
        {
            get
            {
                return IPPayloadLength;
            }
        }

        private int AllFlags
        {
            get
            {
                if (!_allFlagsSet)
                {
                    _allFlags = ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + TCPFields_Fields.TCP_FLAG_POS, TCPFields_Fields.TCP_FLAG_LEN);
                    //tamir: added
                    _allFlagsSet = true;
                }
                return _allFlags;
            }

            set
            {

                ArrayHelper.insertLong(Bytes, value, _ipPayloadOffset + TCPFields_Fields.TCP_FLAG_POS, TCPFields_Fields.TCP_FLAG_LEN);
                _allFlagsSet = false;
            }

        }
        /// <summary> Check the URG flag, flag indicates if the urgent pointer is valid.</summary>
        virtual public bool Urg
        {
            get
            {
                if (!_isUrgSet)
                {
                    _isUrg = (AllFlags & TCPFields_Fields.TCP_URG_MASK) != 0;
                    _isUrgSet = true;
                }
                return _isUrg;
            }

            set
            {
                setFlag(value, TCPFields_Fields.TCP_URG_MASK);
                _isUrgSet = false;
            }
        }

        /// <summary> Check the ACK flag, flag indicates if the ack number is valid.</summary>
        virtual public bool Ack
        {
            get
            {
                if (!_isAckSet)
                {
                    _isAck = (AllFlags & TCPFields_Fields.TCP_ACK_MASK) != 0;
                    _isAckSet = true;
                }
                return _isAck;
            }

            set
            {
                setFlag(value, TCPFields_Fields.TCP_ACK_MASK);
                _isAck = value;
                _isAckSet = true;
            }
        }

        /// <summary> Check the PSH flag, flag indicates the receiver should pass the
        /// data to the application as soon as possible.
        /// </summary>
        virtual public bool Psh
        {
            get
            {
                if (!_isPshSet)
                {
                    _isPsh = (AllFlags & TCPFields_Fields.TCP_PSH_MASK) != 0;
                    _isPshSet = true;
                }
                return _isPsh;
            }

            set
            {
                setFlag(value, TCPFields_Fields.TCP_PSH_MASK);
                _isPsh = value;
                _isPshSet = true;
            }
        }

        /// <summary> Check the RST flag, flag indicates the session should be reset between
        /// the sender and the receiver.
        /// </summary>
        virtual public bool Rst
        {
            get
            {
                if (!_isRstSet)
                {
                    _isRst = (AllFlags & TCPFields_Fields.TCP_RST_MASK) != 0;
                    _isRstSet = true;
                }
                return _isRst;
            }

            set
            {
                setFlag(value, TCPFields_Fields.TCP_RST_MASK);
                _isRst = value;
                _isRstSet = true;
            }
        }

        /// <summary> Check the SYN flag, flag indicates the sequence numbers should
        /// be synchronized between the sender and receiver to initiate
        /// a connection.
        /// </summary>
        virtual public bool Syn
        {
            get
            {
                if (!_isSynSet)
                {
                    _isSyn = (AllFlags & TCPFields_Fields.TCP_SYN_MASK) != 0;
                    _isSynSet = true;
                }
                return _isSyn;
            }

            set
            {
                setFlag(value, TCPFields_Fields.TCP_SYN_MASK);
                _isSyn = value;
                _isSynSet = true;
            }
        }

        /// <summary> Check the FIN flag, flag indicates the sender is finished sending.</summary>
        virtual public bool Fin
        {
            get
            {
                if (!_isFinSet)
                {
                    _isFin = (AllFlags & TCPFields_Fields.TCP_FIN_MASK) != 0;
                    _isFinSet = true;
                }
                return _isFin;
            }

            set
            {
                setFlag(value, TCPFields_Fields.TCP_FIN_MASK);
                _isFin = value;
                _isFinSet = true;
            }
        }

        virtual public bool ECN
        {
            get
            {
                return (AllFlags & TCPFields_Fields.TCP_ECN_MASK) != 0;
            }

            set
            {
                setFlag(value, TCPFields_Fields.TCP_ECN_MASK);
            }
        }

        virtual public bool CWR
        {
            get
            {
                return (AllFlags & TCPFields_Fields.TCP_CWR_MASK) != 0;
            }

            set
            {
                setFlag(value, TCPFields_Fields.TCP_CWR_MASK);
            }
        }

        /// <summary> Fetch the TCP header a byte array.</summary>
        virtual public byte[] TCPHeader
        {
            get
            {
                if (_tcpHeaderBytes == null)
                {
                    _tcpHeaderBytes = PacketEncoding.extractHeader(_ipPayloadOffset, TCPHeaderLength, Bytes);
                }
                return _tcpHeaderBytes;
            }
        }

        /// <summary> Fetch the TCP header as a byte array.</summary>
        override public byte[] Header
        {
            get
            {
                return TCPHeader;
            }
        }

        /// <summary> Fetch the TCP data as a byte array.</summary>
        virtual public byte[] TCPData
        {
            get
            {
                if (_tcpDataBytes == null)
                {
                    _tcpDataBytes = new byte[PayloadDataLength];
                    Array.Copy(Bytes, _ipPayloadOffset + TCPHeaderLength, _tcpDataBytes, 0, PayloadDataLength);
                }
                return _tcpDataBytes;
            }
            set
            {
                SetData(value);
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences_Fields.YELLOW;
            }
        }

        /// <summary> </summary>
        private const long serialVersionUID = 1L;

        /// <summary>
        /// Create a new TCP packet from values
        /// </summary>
        public TCPPacket(IPPacket ipPacket,
                         int sourcePort,
                         int destinationPort,
                         byte[] tcpPayload)
            : base(EthernetFields_Fields.ETH_HEADER_LEN, ipPacket.Bytes, ipPacket.IPVersion)
        {
            int tcpHeaderLength = TCPFields_Fields.TCP_HEADER_LEN;
            int tcpPayloadLength = (tcpPayload != null) ? tcpPayload.Length : 0;

            int totalBytesRequired = 0;
            totalBytesRequired += ipPacket.EthernetHeaderLength;
            totalBytesRequired += ipPacket.IPHeaderLength;
            totalBytesRequired += tcpHeaderLength;
            totalBytesRequired += tcpPayloadLength;

            byte[] newBytes = new byte[totalBytesRequired];

            // copy the contents of the ip packet in, excluding the ip payload
            // since this TCPPacket IS the new payload
            Array.Copy(ipPacket.Bytes, newBytes,
                       ipPacket.EthernetHeaderLength + ipPacket.IPHeaderLength);

            // update the buffer that this packet is overlayed upon
            this.Bytes = newBytes;

            // set the port values
            this.SourcePort = sourcePort;
            this.DestinationPort = destinationPort;

            // set the TCPHeaderLength
            this.TCPHeaderLength = tcpHeaderLength;

            // set the ippacket protocol to tcp
            this.IPProtocol = SharpPcap.Packets.IPProtocol.IPProtocolType.TCP;

            // copy the data payload in, if we were given one
            if(tcpPayload != null)
            {
                TCPData = tcpPayload;
            }
        }

        /// <summary> Create a new TCP packet.</summary>
        public TCPPacket(int byteOffsetToEthernetPayload, byte[] bytes)
            : this(byteOffsetToEthernetPayload, bytes, false)
        {
        }

        /// <summary> Create a new TCP packet.</summary>
        public TCPPacket(int byteOffsetToEthernetPayload, byte[] bytes, bool isEmpty)
            : base(byteOffsetToEthernetPayload, bytes)
        {
        }

        /// <summary> Create a new TCP packet.</summary>
        public TCPPacket(int byteOffsetToEthernetPayload, byte[] bytes, Timeval tv)
            : this(byteOffsetToEthernetPayload, bytes)
        {
            this._timeval = tv;
        }

        /// <summary> Fetch the header checksum.</summary>
        public int Checksum
        {
            get
            {
                return TCPChecksum;
            }
            set
            {
                TCPChecksum=value;
            }
        }

        /// <summary> Computes the TCP checksum, optionally updating the TCP checksum header.
        /// 
        /// </summary>
        /// <param name="update">Specifies whether or not to update the TCP checksum header
        /// after computing the checksum. A value of true indicates the
        /// header should be updated, a value of false indicates it should
        /// not be updated.
        /// </param>
        /// <returns> The computed TCP checksum.
        /// </returns>
        public int ComputeTCPChecksum(bool update)
        {
            return base.ComputeTransportLayerChecksum(TCPFields_Fields.TCP_CSUM_POS, update, true);
        }

        /// <summary> Same as <code>computeTCPChecksum(true);</code>
        /// 
        /// </summary>
        /// <returns> The computed TCP checksum value.
        /// </returns>
        public int ComputeTCPChecksum()
        {
            return ComputeTCPChecksum(true);
        }

        private int _urgentPointer;
        private bool _urgentPointerSet = false;

        /// <summary> Fetch the urgent pointer.</summary>
        public virtual int getUrgentPointer()
        {
            if (!_urgentPointerSet)
            {
                _urgentPointer = ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + TCPFields_Fields.TCP_URG_POS, TCPFields_Fields.TCP_URG_LEN);
                _urgentPointerSet = true;
            }
            return _urgentPointer;
        }

        /// <summary> Sets the urgent pointer.
        /// 
        /// </summary>
        /// <param name="pointer">The urgent pointer value.
        /// </param>
        public void setUrgentPointer(int pointer)
        {
            ArrayHelper.insertLong(Bytes, pointer, _ipPayloadOffset + TCPFields_Fields.TCP_URG_POS, TCPFields_Fields.TCP_URG_LEN);
            _urgentPointerSet = false;
        }

        // next value holds all the flags
        private int _allFlags;
        private bool _allFlagsSet = false;

        private void setFlag(bool on, int MASK)
        {
            if (on)
                AllFlags = AllFlags | MASK;
            else
                AllFlags = AllFlags & ~MASK;
        }

        private bool _isUrg;
        private bool _isUrgSet = false;

        private bool _isAck;
        private bool _isAckSet = false;

        private bool _isPsh;
        private bool _isPshSet = false;

        private bool _isRst;
        private bool _isRstSet = false;

        private bool _isSyn;
        private bool _isSynSet = false;

        private bool _isFin;
        private bool _isFinSet = false;

        private byte[] _tcpHeaderBytes = null;

        // cached copy of the payload of the tcp packet
        private byte[] _tcpDataBytes = null;

        /// <summary> Fetch the TCP data as a byte array.</summary>                          
        public override byte[] Data                                                          
        {                                                                                    
            get
            {
                return TCPData;
            }
        }

        /// <summary> Sets the data section of this tcp packet</summary>
        /// <param name="data">the data bytes
        /// </param>
        private void SetData(byte[] data)
        {
            //reset cached tcp data
            _tcpDataBytes = null;

            // the new packet is the length of the headers + the size of the TCPPacket data payload
            int headerLength = TCPHeaderLength + IPHeaderLength + EthernetHeaderLength;
            int newPacketLength = headerLength + data.Length;

            byte[] newPacketBytes = new byte[newPacketLength];

            // copy the headers into the new packet
            Array.Copy(Bytes, newPacketBytes, headerLength);

            // copy the data into the new packet, immediately after the headers
            Array.Copy(data, 0, newPacketBytes, headerLength, data.Length);

            // make the old headers and new data bytes the new packet bytes
            this.Bytes = newPacketBytes;

            // NOTE: TCPHeaderLength remains the same, we only updated the data portion
            // of the tcp packet

            //update ip total length
            IPPayloadLength = TCPHeaderLength + data.Length;

            //update also offset and pcap header
            OnOffsetChanged();
        }

        public enum OptionTypes
        {
            EndOfList = 0x0,
            Nop = 0x1,
            MaximumSegmentSize = 0x2,
            WindowScale = 0x3,
            SelectiveAckSupported = 0x4,
            Unknown5 = 0x5,
            Unknown6 = 0x6,
            Unknown7 = 0x7,
            Timestamp = 0x8 // http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_Timestamps
        }

        public byte[] Options
        {
            get
            {
                if(Urg)
                {
                    throw new System.NotImplementedException("Urg == true not implemented yet");
                }

                int optionsOffset = TCPFields_Fields.TCP_URG_POS + TCPFields_Fields.TCP_URG_LEN;
                int optionsLength = TCPHeaderLength - optionsOffset;

                byte[] optionBytes = new byte[optionsLength];
                Array.Copy(Bytes, _ipPayloadOffset + optionsOffset, optionBytes, 0, optionsLength);

                return optionBytes;
            }
        }

        /// <summary> Convert this TCP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this TCP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("TCPPacket");
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
            if (Urg)
                buffer.Append(" urg[0x" + System.Convert.ToString(getUrgentPointer(), 16) + "]");
            if (Ack)
                buffer.Append(" ack[0x" + System.Convert.ToString(AcknowledgmentNumber, 16) + "]");
            if (Psh)
                buffer.Append(" psh");
            if (Rst)
                buffer.Append(" rst");
            if (Syn)
                buffer.Append(" syn[0x" + System.Convert.ToString(SequenceNumber, 16) + "," +
                              SequenceNumber + "]");
            if (Fin)
                buffer.Append(" fin");
            buffer.Append(" l=" + TCPHeaderLength + "," + PayloadDataLength);
            buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary> Convert this TCP packet to a verbose.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("TCPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences_Fields.RESET);
            buffer.Append(": ");
            buffer.Append("sport=" + SourcePort + ", ");
            buffer.Append("dport=" + DestinationPort + ", ");
            buffer.Append("seqn=0x" + System.Convert.ToString(SequenceNumber, 16) + ", ");
            buffer.Append("ackn=0x" + System.Convert.ToString(AcknowledgmentNumber, 16) + ", ");
            buffer.Append("hlen=" + HeaderLength + ", ");
            buffer.Append("urg=" + Urg + ", ");
            buffer.Append("ack=" + Ack + ", ");
            buffer.Append("psh=" + Psh + ", ");
            buffer.Append("rst=" + Rst + ", ");
            buffer.Append("syn=" + Syn + ", ");
            buffer.Append("fin=" + Fin + ", ");
            buffer.Append("wsize=" + WindowSize + ", ");       
            buffer.Append("sum=0x" + System.Convert.ToString(Checksum, 16));
            if (this.ValidTCPChecksum)
                buffer.Append(" (correct), ");
            else
                buffer.Append(" (incorrect, should be " + ComputeTCPChecksum(false) + "), ");
            buffer.Append("uptr=0x" + System.Convert.ToString(getUrgentPointer(), 16));
            buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredVerboseString(colored));

            return buffer.ToString();
        }
        public static TCPPacket RandomPacket()
        {
            return RandomPacket(54);
        }

        public static TCPPacket RandomPacket(int size)
        {
            return RandomPacket(size, IPVersions.IPv4);
        }

        private static int MinimumIPv4Bytes = LinkLayer.getLinkLayerLength(LinkLayers_Fields.EN10MB) + IPv4Packet.HeaderMinimumLength + TCPPacket.HeaderMinimumLength;
        private static int MinimumIPv6Bytes = LinkLayer.getLinkLayerLength(LinkLayers_Fields.EN10MB) + IPv6Packet.HeaderMinimumLength + TCPPacket.HeaderMinimumLength;

        public static TCPPacket RandomPacket(IPVersions ipver)
        {
            return RandomPacket(ipver==IPVersions.IPv6 ? MinimumIPv6Bytes : MinimumIPv4Bytes, ipver);
        }

        public override bool IsValid(out string errorString)
        {
            return base.IsValid(out errorString);
        }

        public static TCPPacket RandomPacket(int size, IPVersions ipver)
        {
            if(size < MinimumIPv4Bytes)
                throw new Exception("Size should be at least " + MinimumIPv4Bytes + " (Eth + IP + TCP)");
            if((ipver == IPVersions.IPv6) && (size < MinimumIPv6Bytes))
                throw new Exception("Size should be at least " + MinimumIPv6Bytes + " (Eth + IPv6 + TCP)");

            byte[] bytes = new byte[size];
            SharpPcap.Util.Rand.Instance.GetBytes(bytes);
            TCPPacket tcp = new TCPPacket(LinkLayers_Fields.EN10MB, bytes, true);
            MakeValid(tcp, ipver);
            return tcp;
        }

        public static void MakeValid(TCPPacket tcp, IPVersions ipver)
        {
            tcp.IPVersion = ipver;
            tcp.IPProtocol = Packets.IPProtocol.IPProtocolType.TCP;
            tcp.TCPHeaderLength = TCPFields_Fields.TCP_HEADER_LEN;          //Set the correct TCP header length

            if (ipver == IPVersions.IPv4)
            {
                // the total length of the ip packet is the size of all of the bytes in the packet
                // represented by tcp.Bytes, minus the link layer bytes
                // NOTE: this includes the ip header bytes, which is how the IPv4 total bytes
                // works
                tcp.IPTotalLength = tcp.Bytes.Length - LinkLayers_Fields.EN10MB; //Set the correct IP length
                tcp.IPHeaderLength = IPv4Fields_Fields.IP_HEADER_LEN;
            }
            else if (ipver == IPVersions.IPv6)
            {
                tcp.IPPayloadLength = tcp.Bytes.Length - EthernetFields_Fields.ETH_HEADER_LEN - IPv6Fields_Fields.IPv6_HEADER_LEN;
            }
            else
            {
                throw new System.InvalidOperationException("unknown ipver of " + ipver);
            }

            //Calculate checksums
            tcp.ComputeIPChecksum();
            tcp.ComputeTCPChecksum();
        }
    }
}
