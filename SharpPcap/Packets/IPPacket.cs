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
    /// <summary> An IP protocol packet.
    /// <p>
    /// Extends an ethernet packet, adding IP header information and an IP 
    /// data payload. 
    ///
    /// </summary>
    [Serializable]
    public class IPPacket : EthernetPacket
    {
        /// <summary> Code constants for internet protocol versions.
        /// 
        /// </summary>
        public enum IPVersions
        {
            /// <summary> Internet protocol version 4.</summary>
            IPv4 = 4,
            /// <summary> Internet protocol version 6.</summary>
            IPv6 = 6
        }

        public IPv4Packet ipv4;
        public IPv6Packet ipv6;

        // offset from beginning of byte array where IP header ends (i.e.,
        //  size of ethernet frame header and IP header
        protected internal int _ipOffset;

        /// <summary> Create a new IP packet. </summary>
        public IPPacket(int lLen, byte[] bytes, IPVersions version)
            : base(lLen, bytes)
        {
            IPVersion = version;
        }

        /// <summary> Create a new IP packet. </summary>
        public IPPacket(int lLen, byte[] bytes)
            : base(lLen, bytes)
        {
            InitIPPacket(IPVersion);
        }

        /// <summary> Create a new IP packet.</summary>
        public IPPacket(int lLen, byte[] bytes, Timeval tv)
            : this(lLen, bytes)
        {
            this._timeval = tv;
        }

        /// <summary> Create a new IP packet.</summary>
        public IPPacket(int lLen, byte[] bytes, Timeval tv, IPVersions version)
            : this(lLen, bytes, version)
        {
            this._timeval = tv;
        }

        private void InitIPPacket(IPVersions version)
        {
            ipv4 = null;
            ipv6 = null;

            if (version == IPVersions.IPv4)
            {
                ipv4 = new IPv4Packet(EthernetHeaderLength, Bytes);
                _ipOffset = _ethOffset + IPv4Fields_Fields.IP_HEADER_LEN;
            }
            else if (version == IPVersions.IPv6)
            {
                ipv6 = new IPv6Packet(EthernetHeaderLength, Bytes);
                _ipOffset = _ethOffset + IPv6Fields_Fields.IPv6_HEADER_LEN;
            }
            else
            {
                //lame default
                _ipOffset = _ethOffset;
            }
        }

        public override byte[] Bytes
        {
            get
            {
                return base.Bytes;
            }
            protected set
            {
                base.Bytes = value;
                InitIPPacket(IPVersion);
            }
        }

        /// <summary>
        ///  should be overriden by upper classes
        /// </summary>
        public override void OnOffsetChanged()
        {
            base.OnOffsetChanged();

            InitIPPacket(IPVersion);
        }

        /// <summary> Get the IP version code.</summary>
        virtual public IPVersions IPVersion
        {
            get
            {
                return (IPVersions)((ArrayHelper.extractInteger(Bytes,
                                                                _ethOffset + IPv4Fields_Fields.IP_VER_POS,
                                                                IPv4Fields_Fields.IP_VER_LEN) >> 4) & 0xf);
            }

            set
            {
                Bytes[_ethOffset + IPv4Fields_Fields.IP_VER_POS] &= (byte)(0x0f);
                Bytes[_ethOffset + IPv4Fields_Fields.IP_VER_POS] |= (byte)((((int)value << 4) & 0xf0));

                //version had changed, reinit packet
                InitIPPacket(IPVersion);
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences_Fields.WHITE;
            }
        }

        /// <summary> Returns the payload length of the packet</summary>
        public int IPPayloadLength
        {
            get
            {
                if(ipv4 != null)
                    return ipv4.IPPayloadLength;
                else if(ipv6 != null)
                    return ipv6.IPPayloadLength;
                else
                    throw new System.InvalidOperationException();
            }
            set
            {
                if (ipv4 != null)
                    ipv4.IPTotalLength = value + IPv4Fields_Fields.IP_HEADER_LEN;
                else if (ipv6 != null)
                    ipv6.IPPayloadLength = value;
                else
                    throw new System.InvalidOperationException();
            }
        }

        /// <summary> Convert this IP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this IP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            if(ipv4 != null)
                buffer.Append(ipv4.ToColoredString(colored));
            else if (ipv6 != null)
                buffer.Append(ipv6.ToColoredString(colored));
            else
            {
                //unknown version
                buffer.Append('[');
                if (colored)
                    buffer.Append(Color);
                buffer.Append("IPPacket (Unknown Version)");
                if (colored)
                    buffer.Append(AnsiEscapeSequences_Fields.RESET);
            }

            return buffer.ToString();
        }

        /// <summary> Convert this IP packet to a more verbose string.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("IPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences_Fields.RESET);

            if(ipv4 != null)
                buffer.Append(ipv4.ToColoredVerboseString(colored));
            else if(ipv6 != null)
                buffer.Append(ipv6.ToColoredVerboseString(colored));

            return buffer.ToString();
        }

        public static System.Net.IPAddress GetIPAddress(System.Net.Sockets.AddressFamily ipType, int fieldOffset, byte[] bytes)
        {
            byte[] address;
            if(ipType == System.Net.Sockets.AddressFamily.InterNetwork) // ipv4
            {
                address = new byte[4];
            } else if(ipType == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                address = new byte[16];
            } else
            {
                throw new System.InvalidOperationException("ipType " + ipType + " unknown");
            }

            System.Array.Copy(bytes, fieldOffset,
                              address, 0, address.Length);

            return new System.Net.IPAddress(address);
        }



        // some convience mapping methods since there are fields that match exactly between
        // ipv4 and ipv6
        /// <summary> Fetch the IP address of the host where the packet originated from.</summary>
        virtual public System.Net.IPAddress SourceAddress
        {
            get
            {
                if(ipv4 != null)
                {
                    return ipv4.SourceAddress;
                } else if(ipv6 != null)
                {
                    return ipv6.SourceAddress;
                } else
                {
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");                    
                }
            }

            set
            {
                if(IPVersion == IPVersions.IPv4)
                {
                    ipv4.SourceAddress = value;
                } else if(IPVersion == IPVersions.IPv6)
                {
                    ipv6.SourceAddress = value;
                } else
                {
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");                    
                }
            }
        }

        /// <summary> Fetch the IP address of the host where the packet is destined.</summary>
        virtual public System.Net.IPAddress DestinationAddress
        {
            get
            {
                if(ipv4 != null)
                {
                    return ipv4.DestinationAddress;
                } else if(ipv6 != null)
                {
                    return ipv6.DestinationAddress;
                } else
                {
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");                    
                }
            }

            set
            {
                if(ipv4 != null)
                {
                    ipv4.DestinationAddress = value;
                } else if(ipv6 != null)
                {
                    ipv6.DestinationAddress = value;
                } else
                {
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");                    
                }
            }
        }

        // HopLimit(IPv6) and TimeToLive(IPv4) have the same meaning
        public int HopLimit
        {
            get
            {
                return TimeToLive;
            }

            set
            {
                TimeToLive = value;
            }
        }

        public int TimeToLive
        {
            get
            {
                if(ipv4 != null)
                    return ipv4.TimeToLive;
                else if(ipv6 != null)
                    return ipv6.HopLimit;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");                    
            }

            set
            {
                if(ipv4 != null)
                    ipv4.TimeToLive = value;
                else if(ipv6 != null)
                    ipv6.HopLimit = value;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");                    
            }
        }

        // NextHeader(IPv6) and IPProtocol(IPv4) have the same meaning
        public IPProtocol.IPProtocolType NextHeader
        {
            get
            {
                return IPProtocol;
            }

            set
            {
                IPProtocol = value;
            }
        }

        public IPProtocol.IPProtocolType IPProtocol
        {
            get
            {
                if(ipv4 != null)
                    return ipv4.IPProtocol;
                else if(ipv6 != null)
                    return ipv6.NextHeader;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
            }

            set
            {
                if(ipv4 != null)
                    ipv4.IPProtocol = value;
                else if(ipv6 != null)
                    ipv6.NextHeader = value;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
            }
        }

        /// <summary> Fetch the header checksum.</summary>
        public virtual int GetTransportLayerChecksum(int pos)
        {
            return ArrayHelper.extractInteger(Bytes, pos, 2);
        }

        protected internal virtual void SetTransportLayerChecksum(int cs, int csPos)
        {
            SetChecksum(cs, _ipOffset + csPos);
        }

        public virtual bool IsValidTransportLayerChecksum(bool pseudoIPHeader)
        {
            byte[] upperLayer = IPData;
            if (pseudoIPHeader)
                upperLayer = AttachPseudoIPHeader(upperLayer);
            int onesSum = ChecksumUtils.OnesSum(upperLayer);
            return (onesSum == 0xffff);
        }

        /// <summary> Sets the IP header checksum.</summary>
        protected internal virtual void SetChecksum(int cs, int checkSumOffset)
        {
            ArrayHelper.insertLong(Bytes, cs, checkSumOffset, 2);
        }

        /// <summary>
        /// Returns the IP data.
        /// </summary>
        virtual public byte[] IPData
        {
            get
            {
                if (ipv4 != null)
                    return ipv4.IPData;
                else if (ipv6 != null)
                    return ipv6.IPData;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
            }
        }

        protected internal virtual byte[] AttachPseudoIPHeader(byte[] origHeader)
        {
            if (ipv4 != null)
                return ipv4.AttachPseudoIPHeader(origHeader);
            else if (ipv6 != null)
                return ipv6.AttachPseudoIPHeader(origHeader);
            else
                throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
        }

        public int ComputeTransportLayerChecksum(int checksumOffset, bool update, bool pseudoIPHeader)
        {
            // copy the tcp section with data
            byte[] dataToChecksum = IPData;
            // reset the checksum field (checksum is calculated when this field is
            // zeroed)
            ArrayHelper.insertLong(dataToChecksum, 0, checksumOffset, 2);
            if (pseudoIPHeader)
                dataToChecksum = AttachPseudoIPHeader(dataToChecksum);
            // compute the one's complement sum of the tcp header
            int cs = ChecksumUtils.OnesComplementSum(dataToChecksum);
            if (update)
            {
                SetTransportLayerChecksum(cs, checksumOffset);
            }

            return cs;
        }

        /// <summary> Same as <code>computeIPChecksum(true);</code>
        /// 
        /// </summary>
        /// <returns> The computed IP checksum value.
        /// </returns>
        public int ComputeIPChecksum()
        {
            return ComputeIPChecksum(true);
        }

        /// <summary> Computes the IP checksum, optionally updating the IP checksum header.
        /// 
        /// </summary>
        /// <param name="update">Specifies whether or not to update the IP checksum
        /// header after computing the checksum.  A value of true indicates
        /// the header should be updated, a value of false indicates it
        /// should not be updated.
        /// </param>
        /// <returns> The computed IP checksum.
        /// </returns>
        public int ComputeIPChecksum(bool update)
        {
            if (ipv4 != null)
                return ipv4.ComputeIPChecksum(update);
            else if (ipv6 != null)
                return 0; // ipv6 packets don't contain a checksum
            else
                throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
        }

        /// <summary> Check if the IP packet is valid, checksum-wise.</summary>
        virtual public bool ValidChecksum
        {
            get
            {
                return ValidIPChecksum;
            }

        }

        /// <summary> Check if the IP packet is valid, checksum-wise.</summary>
        virtual public bool ValidIPChecksum
        {
            get
            {
                if (ipv4 != null)
                    return ipv4.ValidIPChecksum;
                else if (ipv6 != null)
                    return true;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
            }

        }

        /// <summary> Fetch the header a byte array.</summary>
        virtual public byte[] IPHeader
        {
            get
            {
                if(ipv4 != null)
                    return ipv4.IPHeader;
                else if(ipv6 != null)
                    return ipv6.IPv6Header;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
            }
        }

        /// <summary> Fetch the UDP header as a byte array.</summary>
        override public byte[] Header
        {
            get
            {
                return IPHeader;
            }
        }

        /// <summary> Fetch the IP header length in bytes. </summary>
        /// <summary> Sets the IP header length field.  At most, this can be a 
        /// four-bit value.  The high order bits beyond the fourth bit
        /// will be ignored.
        /// 
        /// </summary>
        /// <param name="length">The length of the IP header in 32-bit words.
        /// </param>
        virtual public int IPHeaderLength
        {
            get
            {
                if (ipv4 != null)
                    return ipv4.IPHeaderLength;
                else if (ipv6 != null)
                    return IPv6Fields_Fields.IPv6_HEADER_LEN;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
            }

            set
            {
                if (ipv4 != null)
                    ipv4.IPHeaderLength = value;
                else if (ipv6 != null)
                    throw new System.InvalidOperationException("can't set IPHeaderLength on ipv6 packet");
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
            }

        }

        /// <summary> Fetches the packet header length.</summary>
        override public int HeaderLength
        {
            get
            {
                return IPHeaderLength;
            }
        }

        /// <summary> Fetch the IP length in bytes.</summary>
        public virtual int IPTotalLength
        {
            get
            {
                if (ipv4 != null)
                    return ipv4.IPTotalLength;
                else if (ipv6 != null)
                    return IPv6Fields_Fields.IPv6_HEADER_LEN + ipv6.IPPayloadLength;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
            }

            set
            {
                if (ipv4 != null)
                    ipv4.IPTotalLength = value;
                else if (ipv6 != null)
                    ipv6.IPPayloadLength = value - IPv6Fields_Fields.IPv6_HEADER_LEN;
                else
                    throw new System.InvalidOperationException("ipv4 and ipv6 are both null");
            }
        }
    }
}