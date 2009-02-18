using System;
using SharpPcap.Packets.Util;

namespace SharpPcap.Packets
{
	public class IPv4Packet : EthernetPacket
	{
    	/// <summary> Type of service code constants for IP. Type of service describes 
    	/// how a packet should be handled.
    	/// <p>
    	/// TOS is an 8-bit record in an IP header which contains a 3-bit 
    	/// precendence field, 4 TOS bit fields and a 0 bit.
    	/// <p>
    	/// The following constants are bit masks which can be logically and'ed
    	/// with the 8-bit IP TOS field to determine what type of service is set.
    	/// <p>
    	/// Taken from TCP/IP Illustrated V1 by Richard Stevens, p34.
    	/// 
    	/// </summary>
    	public struct TypesOfService_Fields{
    		public readonly static int MINIMIZE_DELAY = 0x10;
    		public readonly static int MAXIMIZE_THROUGHPUT = 0x08;
    		public readonly static int MAXIMIZE_RELIABILITY = 0x04;
    		public readonly static int MINIMIZE_MONETARY_COST = 0x02;
    		public readonly static int UNUSED = 0x01;
    	}

        public static int ipVersion = 4;

		/// <summary>
		///  should be overriden by upper classes
		/// </summary>
		public override void OnOffsetChanged()
		{
			base.OnOffsetChanged();
			_ipOffset = _ethOffset + IPHeaderLength;
		}

		/// <summary> Get the IP version code.</summary>
		public virtual int Version
		{
			get
			{
				return IPVersion;
			}
			set
			{
				IPVersion = value;
			}
		}
		/// <summary> Get the IP version code.</summary>
		virtual public int IPVersion
		{
			get
			{
				return (ArrayHelper.extractInteger(_bytes,
                                                   _ethOffset + IPv4Fields_Fields.IP_VER_POS,
                                                   IPv4Fields_Fields.IP_VER_LEN) >> 4) & 0xf;
			}

			set
			{
				_bytes[_ethOffset + IPv4Fields_Fields.IP_VER_POS] &= (byte)(0x0f);
				_bytes[_ethOffset + IPv4Fields_Fields.IP_VER_POS] |= (byte)(((value << 4) & 0xf0));
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
				return (ArrayHelper.extractInteger(_bytes,
                                                   _ethOffset + IPv4Fields_Fields.IP_VER_POS,
                                                   IPv4Fields_Fields.IP_VER_LEN) & 0xf) * 4;
			}

			set
			{
				value /= 4;
				// Clear low order bits and then set
				_bytes[_ethOffset + IPv4Fields_Fields.IP_VER_POS] &= (byte)(0xf0);
				_bytes[_ethOffset + IPv4Fields_Fields.IP_VER_POS] |= (byte)(value & 0x0f);
				// set offset into _bytes of previous layers
				_ipOffset = _ethOffset + IPHeaderLength;
			}

		}
		/// <summary> Fetch the packet IP header length.</summary>
		override public int HeaderLength
		{
			get
			{
				return IPHeaderLength;
			}

		}
		/// <summary> Fetch the packet IP header length.</summary>
		public int IpHeaderLength
		{
			get
			{
				return IPHeaderLength;
			}

			set
			{
				IPHeaderLength = value;
			}

		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Fetch the unique ID of this IP datagram. The ID normally 
		/// increments by one each time a datagram is sent by a host.
		/// </summary>
		/// <summary> Sets the IP identification header value.
		/// 
		/// </summary>
		/// <param name="id">A 16-bit unsigned integer.
		/// </param>
		virtual public int Id
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes,
                                                  _ethOffset + IPv4Fields_Fields.IP_ID_POS,
                                                  IPv4Fields_Fields.IP_ID_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value,
                                       _ethOffset + IPv4Fields_Fields.IP_ID_POS,
                                       IPv4Fields_Fields.IP_ID_LEN);
			}

		}

		/// <summary> Fetch fragmentation offset.</summary>
		/// <summary> Sets the fragment offset header value.  The offset specifies a
		/// number of octets (i.e., bytes).
		/// 
		/// </summary>
		/// <param name="offset">A 13-bit unsigned integer.
		/// </param>
		virtual public int FragmentOffset
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ethOffset + IPv4Fields_Fields.IP_FRAG_POS, IPv4Fields_Fields.IP_FRAG_LEN) & 0x1fff;
			}

			set
			{
				_bytes[_ethOffset + IPv4Fields_Fields.IP_FRAG_POS] &= (byte)(0xe0);
				_bytes[_ethOffset + IPv4Fields_Fields.IP_FRAG_POS] |= (byte)(((value >> 8) & 0x1f));
				_bytes[_ethOffset + IPv4Fields_Fields.IP_FRAG_POS + 1] = (byte)(value & 0xff);
			}

		}

		/// <summary> Fetch the IP address of the host where the packet originated from.</summary>
		virtual public System.Net.IPAddress SourceAddress
		{
			get
			{
                return IPPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             _ethOffset + IPv4Fields_Fields.IP_SRC_POS, _bytes);
			}

			set
			{
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0, _bytes, _ethOffset + IPv4Fields_Fields.IP_SRC_POS, address.Length);
			}
		}

		/// <summary> Fetch the IP address of the host where the packet is destined.</summary>
		virtual public System.Net.IPAddress DestinationAddress
		{
			get
			{
                return IPPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             _ethOffset + IPv4Fields_Fields.IP_DST_POS,
                                             _bytes);
			}

			set
			{
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0, _bytes, _ethOffset + IPv4Fields_Fields.IP_DST_POS, address.Length);
			}

		}

		/// <summary> Fetch the IP header a byte array.</summary>
		virtual public byte[] IPHeader
		{
			get
			{
				return PacketEncoding.extractHeader(_ethOffset, IPHeaderLength, _bytes);
			}

		}

        /// <summary> Fetch the IP header as a byte array.</summary>
		override public byte[] Header
		{
			get
			{
				return IPHeader;
			}

		}

        /// <summary> Fetch the IP data as a byte array.</summary>
		virtual public byte[] IPData
		{
			get
			{

				// set data length based on info in headers (note: tcpdump
				//  can return extra junk bytes which bubble up to here

				//tamir: changed getLength() to specific getIPTotalLength() to fix
				//confusion in subclasses overloading getLength()
				int tmpLen = IPTotalLength - IPHeaderLength;
				return PacketEncoding.extractData(_ethOffset, IPHeaderLength, _bytes, tmpLen);
			}

		}

        /// <summary> Fetch the header checksum.</summary>
		virtual public int IPChecksum
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ethOffset + IPv4Fields_Fields.IP_CSUM_POS, IPv4Fields_Fields.IP_CSUM_LEN);
			}

			set
			{
				SetChecksum(value, _ethOffset + IPv4Fields_Fields.IP_CSUM_POS);
			}

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
				// first validate other information about the packet. if this stuff
				// is not true, the packet (and therefore the checksum) is invalid
				// - ip_hl >= 5 (ip_hl is the length in 4-byte words)
				if (IPHeaderLength < IPv4Fields_Fields.IP_HEADER_LEN)
				{
					return false;
				}
				else
				{
					return (_OnesSum(_bytes, _ethOffset, IpHeaderLength) == 0xffff);
				}
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
		// offset from beginning of byte array where IP header ends (i.e.,
		//  size of ethernet frame header and IP header
		protected internal int _ipOffset;

		/// <summary> Create a new IP packet. </summary>
		public IPv4Packet(int lLen, byte[] bytes)
			: base(lLen, bytes)
		{
			_ipOffset = _ethOffset + IPHeaderLength;
		}

		/// <summary> Create a new IP packet.</summary>
		public IPv4Packet(int lLen, byte[] bytes, Timeval tv)
			: this(lLen, bytes)
		{
			this._timeval = tv;
		}

		/// <summary> Fetch the type of service./// </summary>
		public virtual int TypeOfService
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes,
                                                  _ethOffset + IPv4Fields_Fields.IP_TOS_POS,
                                                  IPv4Fields_Fields.IP_TOS_LEN);
			}
			set
			{
				_bytes[_ethOffset + IPv4Fields_Fields.IP_TOS_POS] = (byte)(value & 0xff);
			}
		}

		/// <summary> Fetch the IP length in bytes.</summary>
		public virtual int Length
		{
			get
			{
				return IPTotalLength;
			}
			set
			{
				IPTotalLength = value;
			}
		}

        /// <summary> Fetch the IP length in bytes.</summary>
		public virtual int IPTotalLength
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, 
                                                  _ethOffset + IPv4Fields_Fields.IP_LEN_POS,
                                                  IPv4Fields_Fields.IP_LEN_LEN);
			}
			set
			{
				ArrayHelper.insertLong(_bytes, value,
                                       _ethOffset + IPv4Fields_Fields.IP_LEN_POS,
                                       IPv4Fields_Fields.IP_LEN_LEN);
			}
		}

        public virtual int IPPayloadLength
        {
            get
            {
                return IPTotalLength - IPv4Fields_Fields.IP_HEADER_LEN;
            }
        }

		/// <summary> Fetch fragmentation flags.</summary>
		/// </summary>
		/// <param name="flags">A 3-bit unsigned integer.
		/// </param>
		public virtual int FragmentFlags
		{
			get
			{
				// fragment flags are the high 3 bits
				//		int huh = ArrayHelper.extractInteger(_bytes, _ethOffset
				//				+ IP_FRAG_POS, IP_FRAG_LEN);
				return (ArrayHelper.extractInteger(_bytes, _ethOffset + IPv4Fields_Fields.IP_FRAG_POS, IPv4Fields_Fields.IP_FRAG_LEN) >> 13) & 0x7;
			}
			set
			{
				_bytes[_ethOffset + IPv4Fields_Fields.IP_FRAG_POS] &= (byte)(0x1f);
				_bytes[_ethOffset + IPv4Fields_Fields.IP_FRAG_POS] |= (byte)(((value << 5) & 0xe0));
			}
		}

        /// <summary> Fetch the time to live. TTL sets the upper limit on the number of 
		/// routers through which this IP datagram is allowed to pass.
		/// Originally intended to be the number of seconds the packet lives it is now decremented
		/// by one each time a router passes the packet on
		/// 
		/// 8-bit value
		/// </summary>
		public virtual int TimeToLive
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ethOffset + IPv4Fields_Fields.IP_TTL_POS,
                                                  IPv4Fields_Fields.IP_TTL_LEN);
			}
			set
			{
				_bytes[_ethOffset + IPv4Fields_Fields.IP_TTL_POS] = (byte)value;
			}
		}

		/// <summary> Fetch the code indicating the type of protocol embedded in the IP</summary>
		/// <seealso cref="IPProtocols.">
		/// </seealso>
		public virtual IPProtocol.IPProtocolType IPProtocol
		{
			get
			{
				return (IPProtocol.IPProtocolType)ArrayHelper.extractInteger(_bytes,
                                                                             _ethOffset + IPv4Fields_Fields.IP_CODE_POS,
                                                                             IPv4Fields_Fields.IP_CODE_LEN);
			}
			set
			{
				_bytes[_ethOffset + IPv4Fields_Fields.IP_CODE_POS] = (byte)value;
			}
		}

		/// <summary> Fetch the IP data as a byte array.</summary>
		public override byte[] Data
		{
			get
			{
				return IPData;
			}
		}

		/// <summary> Fetch the IP header checksum.</summary>
		public virtual int Checksum
		{
			get
			{
				return IPChecksum;
			}
			set
			{
				IPChecksum=value;
			}
		}

		/// <summary> Sets the IP header checksum.</summary>
		protected internal virtual void SetChecksum(int cs, int checkSumOffset)
		{
			ArrayHelper.insertLong(_bytes, cs, checkSumOffset, 2);
		}

		protected internal virtual void SetTransportLayerChecksum(int cs, int csPos)
		{
			SetChecksum(cs, _ipOffset + csPos);
		}

		/// <summary>
		/// Computes the one's complement sum on a byte array
		/// </summary>
		protected internal virtual int _OnesCompSum(byte[] bytes)
		{
			//just complement the one's sum
			return _OnesCompSum(bytes, 0, bytes.Length);
		}

		/// <summary> 
		/// Computes the one's complement sum on a byte array
		/// </summary>
		protected internal virtual int _OnesCompSum(byte[] bytes, int start, int len)
		{
			//just complement the one's sum
			return (~_OnesSum(bytes, start, len)) & 0xFFFF;
		}

		/// <summary>
		/// Computes the one's sum on a byte array.
		/// Based TCP/IP Illustrated Vol. 2(1995) by Gary R. Wright and W. Richard
		/// Stevens. Page 236. And on http://www.cs.utk.edu/~cs594np/unp/checksum.html
		/// </summary>
		protected internal virtual int _OnesSum(byte[] bytes)
		{
			return _OnesSum(bytes, 0, bytes.Length);
		}

		/// <summary>
		/// Computes the one's sum on a byte array.
		/// Based TCP/IP Illustrated Vol. 2(1995) by Gary R. Wright and W. Richard
		/// Stevens. Page 236. And on http://www.cs.utk.edu/~cs594np/unp/checksum.html
		/// </summary>
		protected internal virtual int _OnesSum(byte[] bytes, int start, int len)
		{
			int sum = 0; /* assume 32 bit long, 16 bit short */
			int i = start;
			len = start + len;

			while (i < len - 1)
			{
				sum += ArrayHelper.extractInteger(bytes, i, 2);
				//if ((sum & unchecked((int)0x80000000)) != 0)
				if ((sum & 0x80000000) != 0)
					/* if high order bit set, fold */
					sum = (sum & 0xFFFF) + (sum >> 16);
				i += 2;
			}

			if (i < len)
				/* take care of left over byte */
				sum += ArrayHelper.extractInteger(bytes, start, 2);

			while (sum >> 16 != 0)
				sum = (sum & 0xFFFF) + (sum >> 16);

			return sum & 0xFFFF;
		}

		/*
		* taken from TCP/IP Illustrated Vol. 2(1995) by Gary R. Wright and W.
		* Richard Stevens. Page 236
		*/

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
			//copy the ip header
			byte[] ip = ArrayHelper.copy(_bytes, _ethOffset, IpHeaderLength);
			//reset the checksum field (checksum is calculated when this field is zeroed)
			ArrayHelper.insertLong(ip, 0, IPv4Fields_Fields.IP_CSUM_POS, 2);
			//compute the one's complement sum of the ip header
			int cs = _OnesCompSum(ip, 0, ip.Length);
			if (update)
			{
				IPChecksum = cs;
			}

			return cs;
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
			int cs = _OnesCompSum(dataToChecksum);
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

        // Prepend to the given byte[] origHeader the portion of the IPv6 header used for
        // generating an tcp checksum
        //
        // http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_checksum_using_IPv4
        // http://tools.ietf.org/html/rfc793
		protected internal virtual byte[] AttachPseudoIPHeader(byte[] origHeader)
		{
			bool odd = origHeader.Length % 2 != 0;
            int numberOfBytesFromIPHeaderUsedToGenerateChecksum = 12;
			int headerSize = numberOfBytesFromIPHeaderUsedToGenerateChecksum + origHeader.Length;
			if (odd)
				headerSize++;

			byte[] headerForChecksum = new byte[headerSize];
			// 0-7: ip src+dest addr
			Array.Copy(_bytes, _ethOffset + IPv4Fields_Fields.IP_SRC_POS, headerForChecksum, 0, 8);
			// 8: always zero
			headerForChecksum[8] = 0;
			// 9: ip protocol
			headerForChecksum[9] = (byte)IPProtocol;
			// 10-11: header+data length
			ArrayHelper.insertLong(headerForChecksum, origHeader.Length, 10, 2);

			// prefix the pseudoHeader to the header+data
			Array.Copy(origHeader,
                       0, headerForChecksum,
                       numberOfBytesFromIPHeaderUsedToGenerateChecksum, origHeader.Length);

			//if not even length, pad with a zero
			if (odd)
				headerForChecksum[headerForChecksum.Length - 1] = 0;

			return headerForChecksum;
		}

		public virtual bool IsValidTransportLayerChecksum(bool pseudoIPHeader)
		{
			byte[] upperLayer = IPData;
			if (pseudoIPHeader)
				upperLayer = AttachPseudoIPHeader(upperLayer);
			int onesSum = _OnesSum(upperLayer);
			return (onesSum == 0xffff);
		}

		/// <summary> Fetch the header checksum.</summary>
		public virtual int GetTransportLayerChecksum(int pos)
		{
			return ArrayHelper.extractInteger(_bytes, pos, 2);
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
			buffer.Append('[');
			if (colored)
				buffer.Append(Color);
			buffer.Append("IPPacket");
			if (colored)
				buffer.Append(AnsiEscapeSequences_Fields.RESET);
			buffer.Append(": ");
			buffer.Append(SourceAddress + " -> " + DestinationAddress);
			buffer.Append(" proto=" + IPProtocol);
			buffer.Append(" l=" + IPHeaderLength + "," + Length);
			buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredString(colored));

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
			buffer.Append(": ");
			buffer.Append("version=" + Version + ", ");
			buffer.Append("hlen=" + HeaderLength + ", ");
			buffer.Append("tos=" + TypeOfService + ", ");
			buffer.Append("length=" + Length + ", ");
			buffer.Append("id=" + Id + ", ");
			buffer.Append("flags=0x" + System.Convert.ToString(FragmentFlags, 16) + ", ");
			buffer.Append("offset=" + FragmentOffset + ", ");
			buffer.Append("ttl=" + TimeToLive + ", ");
			buffer.Append("proto=" + IPProtocol + ", ");
			buffer.Append("sum=0x" + System.Convert.ToString(Checksum, 16));
			if (this.ValidChecksum)
				buffer.Append(" (correct), ");
			else
				buffer.Append(" (incorrect, should be " + ComputeIPChecksum(false) + "), ");
			buffer.Append("src=" + SourceAddress + ", ");
			buffer.Append("dest=" + DestinationAddress);
			buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredVerboseString(colored));

			return buffer.ToString();
		}

		/// <summary> This inner class provides access to private methods for unit testing.</summary>
		public class TestProbe
		{
			public TestProbe(IPv4Packet enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}

			private void InitBlock(IPv4Packet enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			private IPv4Packet enclosingInstance;
			virtual public int ComputedReceiverIPChecksum
			{
				get
				{
					return Enclosing_Instance._OnesSum(Enclosing_Instance._bytes,
                                                       Enclosing_Instance._ethOffset,
                                                       Enclosing_Instance.IpHeaderLength);
				}
			}

			virtual public int ComputedSenderIPChecksum()
			{
				return Enclosing_Instance.ComputeIPChecksum(false);
			}

			public IPv4Packet Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
			}
		}		
	}
}
