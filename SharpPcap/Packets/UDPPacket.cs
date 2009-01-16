// $Id: UDPPacket.cs,v 1.2 2007-07-08 13:27:27 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.AnsiEscapeSequences_Fields' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using AnsiEscapeSequences_Fields = Tamir.IPLib.Packets.Util.AnsiEscapeSequences_Fields;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.ArrayHelper' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ArrayHelper = Tamir.IPLib.Packets.Util.ArrayHelper;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.Timeval' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using Timeval = Tamir.IPLib.Packets.Util.Timeval;
using Tamir.IPLib.Packets.Util;

namespace Tamir.IPLib.Packets
{


	/// <summary> A UDP packet.
	/// <p>
	/// Extends an IP packet, adding a UDP header and UDP data payload.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.2 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-08 13:27:27 $ </lastModifiedAt>
	[Serializable]
	public class UDPPacket : IPPacket, UDPFields
	{
		/// <summary> Fetch the port number on the source host.</summary>
		virtual public int SourcePort
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ipOffset + UDPFields_Fields.UDP_SP_POS, UDPFields_Fields.UDP_PORT_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ipOffset + UDPFields_Fields.UDP_SP_POS, UDPFields_Fields.UDP_PORT_LEN);
			}

		}
		/// <summary> Fetch the port number on the target host.</summary>
		virtual public int DestinationPort
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ipOffset + UDPFields_Fields.UDP_DP_POS, UDPFields_Fields.UDP_PORT_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ipOffset + UDPFields_Fields.UDP_DP_POS, UDPFields_Fields.UDP_PORT_LEN);
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
				ArrayHelper.insertLong(_bytes, value, _ipOffset + UDPFields_Fields.UDP_LEN_POS, UDPFields_Fields.UDP_LEN_LEN);
			}

		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Fetch the header checksum.</summary>
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
		override public bool ValidChecksum
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
					_udpHeaderBytes = PacketEncoding.extractHeader(_ipOffset, UDPFields_Fields.UDP_HEADER_LEN, _bytes);
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
		/// <summary> Fetch the UDP data as a byte array.</summary>
		virtual public byte[] UDPData
		{
			get
			{
				if (_udpDataBytes == null)
				{
					// set data length based on info in headers (note: tcpdump
					//  can return extra junk bytes which bubble up to here
					int tmpLen = _bytes.Length - _ipOffset - UDPFields_Fields.UDP_HEADER_LEN;
					_udpDataBytes = PacketEncoding.extractData(_ipOffset, UDPFields_Fields.UDP_HEADER_LEN, _bytes, tmpLen);
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
			byte[] headers = ArrayHelper.copy(_bytes, 0, UDPFields_Fields.UDP_HEADER_LEN +IpHeaderLength+EthernetHeaderLength);
			byte[] newBytes = ArrayHelper.join(headers, data);
			this._bytes = newBytes;
			UDPLength = _bytes.Length-IpHeaderLength-EthernetHeaderLength;
		
			//update ip total length length
			IPTotalLength = IpHeaderLength + UDPFields_Fields.UDP_HEADER_LEN + data.Length;
		
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
		public override int Length
		{
			get
			{
				// should produce the same value as header.length + data.length
				return ArrayHelper.extractInteger(_bytes, _ipOffset + UDPFields_Fields.UDP_LEN_POS, UDPFields_Fields.UDP_LEN_LEN);
			}
		}

		/// <summary> Fetch the header checksum.</summary>
		public override int Checksum
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
			// copy the udp section with data
			byte[] udp = IPData;
			// reset the checksum field (checksum is calculated when this field is
			// zeroed)
			ArrayHelper.insertLong(udp, 0, UDPFields_Fields.UDP_CSUM_POS, UDPFields_Fields.UDP_CSUM_LEN);
			//pseudo ip header should be attached to the udp+data
			udp = AttachPseudoIPHeader(udp);
			// compute the one's complement sum of the udp header
			int cs = _OnesCompSum(udp);
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