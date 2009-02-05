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
using SharpPcap.Packets.Util;
using System.ComponentModel;

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
        public IPv4Packet ipv4;
        public IPv6Packet ipv6;

        private void SetIPOffsetFromVersion()
        {
            if(IPVersion == 4)
            {
                _ipOffset = _ethOffset + ipv4.IPHeaderLength;
            } else if(IPVersion == 6)
            {
                _ipOffset = _ethOffset + IPv6Fields_Fields.IPv6_HEADER_LEN;
            } else
            {
                throw new System.NotImplementedException("IPVersion of " + IPVersion + " is unrecognized");
            }
        }

		/// <summary>
		///  should be overriden by upper classes
		/// </summary>
		public override void OnOffsetChanged()
		{
			base.OnOffsetChanged();

            SetIPOffsetFromVersion();
		}

		/// <summary> Get the IP version code.</summary>
		virtual public int IPVersion
		{
			get
			{
				return (ArrayHelper.extractInteger(_bytes, _ethOffset + IPv4Fields_Fields.IP_VER_POS, IPv4Fields_Fields.IP_VER_LEN) >> 4) & 0xf;
			}

			set
			{
				_bytes[_ethOffset + IPv4Fields_Fields.IP_VER_POS] &= (byte)(0x0f);
				_bytes[_ethOffset + IPv4Fields_Fields.IP_VER_POS] |= (byte)(((value << 4) & 0xf0));
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
		public IPPacket(int lLen, byte[] bytes)
			: base(lLen, bytes)
		{
            if(IPVersion == 4)
            {
                ipv4 = new IPv4Packet(lLen, bytes);
            } else if(IPVersion == 6)
            {
                ipv6 = new IPv6Packet(lLen, bytes);
            } else
            {
                throw new System.NotImplementedException("IPVersion of " + IPVersion + " is unrecognized");
            }

            SetIPOffsetFromVersion();
		}

		/// <summary> Create a new IP packet.</summary>
		public IPPacket(int lLen, byte[] bytes, Timeval tv)
			: this(lLen, bytes)
		{
			this._timeval = tv;
		}

        /// <summary> Returns the payload length of the packet</summary>
        public int IPPayloadLength()
        {
            if(ipv4 != null)
                return ipv4.IPPayloadLength;
            else if(ipv6 != null)
                return ipv6.IPPayloadLength;
            else
                throw new System.InvalidOperationException();
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

            if(ipv4 != null)
                buffer.Append(ipv4.ToColoredString(colored));
            else if(ipv6 != null)
                buffer.Append(ipv6.ToColoredString(colored));

			return buffer.ToString();
		}

		/// <summary> Convert this IP packet to a more verbose string.</summary>
		public virtual System.String ToColoredVerboseString(bool colored)
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
                if(IPVersion == 4)
                {
                    ipv4.SourceAddress = value;
                } else if(IPVersion == 6)
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
	}
}