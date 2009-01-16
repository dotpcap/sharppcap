// $Id: EthernetPacket.cs,v 1.2 2007-07-08 13:27:27 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
//UPGRADE_TODO: The type 'SharpPcap.Packets.Util.AnsiEscapeSequences' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using AnsiEscapeSequences = SharpPcap.Packets.Util.AnsiEscapeSequences;
//UPGRADE_TODO: The type 'SharpPcap.Packets.Util.ArrayHelper' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ArrayHelper = SharpPcap.Packets.Util.ArrayHelper;
//UPGRADE_TODO: The type 'SharpPcap.Packets.Util.Timeval' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using Timeval = SharpPcap.Packets.Util.Timeval;
using SharpPcap.Packets.Util;
namespace SharpPcap.Packets
{


	/// <summary> An ethernet packet.
	/// <p>
	/// Contains link-level header and data payload encapsulated by an ethernet
	/// packet.
	/// <p>
	/// There are currently two subclasses. IP and ARP protocols are supported.
	/// IPPacket extends with ip header and data information.
	/// ARPPacket extends with hardware and protocol addresses.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.2 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-08 13:27:27 $ </lastModifiedAt>
	[Serializable]
	public class EthernetPacket : Packet, EthernetFields
	{
		/// <summary> Fetch the ethernet header length in bytes.</summary>
		virtual public int EthernetHeaderLength
		{
			get
			{
				return _ethernetHeaderLength;
			}

		}
		/// <summary> Fetch the packet ethernet header length.</summary>
		virtual public int HeaderLength
		{
			get
			{
				return EthernetHeaderLength;
			}

		}
		/// <summary> Fetch the ethernet header as a byte array.</summary>
		virtual public byte[] EthernetHeader
		{
			get
			{
				return PacketEncoding.extractHeader(0, EthernetHeaderLength, _bytes);
			}

		}
		/// <summary> Fetch the ethernet header as a byte array.</summary>
		override public byte[] Header
		{
			get
			{
				return EthernetHeader;
			}

		}
		/// <summary> Fetch the ethernet data as a byte array.</summary>
		virtual public byte[] EthernetData
		{
			get
			{
				return PacketEncoding.extractData(0, EthernetHeaderLength, _bytes);
			}

		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Fetch the ethernet protocol.</summary>
		/// <summary> Sets the ethernet protocol.</summary>
		virtual public int EthernetProtocol
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, EthernetFields_Fields.ETH_CODE_POS, EthernetFields_Fields.ETH_CODE_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, EthernetFields_Fields.ETH_CODE_POS, EthernetFields_Fields.ETH_CODE_LEN);
			}

		}

		/// <summary>
		///  should be overriden by upper classes
		/// </summary>
		public virtual void OnOffsetChanged()
		{
			if(PcapHeader!=null)
				PcapHeader = new PcapHeader(PcapHeader.Seconds, PcapHeader.MicroSeconds, _bytes.Length, _bytes.Length);
		}

		/// <summary> Fetch the timeval containing the time the packet arrived on the 
		/// device where it was captured.
		/// </summary>
		override public Timeval Timeval
		{
			get
			{
				return _timeval;
			}

		}
		/// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
		override public System.String Color
		{
			get
			{
				return AnsiEscapeSequences_Fields.DARK_GRAY;
			}

		}
		override public byte[] Bytes
		{
			get
			{
				return _bytes;
			}

		}
		// store the data here, all subclasses can offset into this
		protected internal byte[] _bytes;

		// offset from beginning of byte array where the data payload 
		// (i.e. IP packet) starts. The size of the ethernet frame header.
		protected internal int _ethOffset;

		// time that the packet was captured off the wire
		protected internal Timeval _timeval;


		/// <summary> Construct a new ethernet packet.
		/// <p>
		/// For the purpose of jpcap, when the type of ethernet packet is 
		/// recognized as a protocol for which a class exists network library, 
		/// then a more specific class like IPPacket or ARPPacket is instantiated.
		/// The subclass can always be cast into a more generic form.
		/// </summary>
		public EthernetPacket(int lLen, byte[] bytes)
		{
			_bytes = bytes;
			_ethernetHeaderLength = lLen;
			_ethOffset = lLen;
		}

		/// <summary> Construct a new ethernet packet, including the capture time.</summary>
		public EthernetPacket(int lLen, byte[] bytes, Timeval tv)
			: this(lLen, bytes)
		{
			this._timeval = tv;
		}

		// set in constructor
		private int _ethernetHeaderLength;

		/// <summary> Fetch the ethernet data as a byte array.</summary>
		public override byte[] Data
		{
			get
			{
				return EthernetData;
			}
		}
		/// <summary> Fetch the MAC address of the host where the packet originated from.</summary>
		public virtual System.String SourceHwAddress
		{
			get
			{
				return MACAddress.extract(EthernetFields_Fields.ETH_SRC_POS, _bytes);
			}
			set
			{
				MACAddress.insert(value, _bytes, EthernetFields_Fields.ETH_SRC_POS);
			}
		}

		/// <summary> Set the MAC address of the host where the packet originated from.</summary>
		public virtual long SourceHwAddressAsLong
		{
			get
			{
				string mac = MACAddress.extract(EthernetFields_Fields.ETH_SRC_POS, _bytes);
				return Int64.Parse(mac.Replace(":", ""), System.Globalization.NumberStyles.HexNumber);
			}
			set
			{
				ArrayHelper.insertLong(_bytes, value, EthernetFields_Fields.ETH_SRC_POS, MACAddress.WIDTH);
			}
		}

		/// <summary> Fetch the MAC address of the host where the packet originated from.</summary>
		public virtual System.String DestinationHwAddress
		{
			get
			{
				return MACAddress.extract(EthernetFields_Fields.ETH_DST_POS, _bytes);
			}
			set
			{
				MACAddress.insert(value, _bytes, EthernetFields_Fields.ETH_DST_POS);
			}
		}

		/// <summary> Set the MAC address of the host where the packet originated from.</summary>
		public virtual long DestinationHwAddressAsLong
		{
			get
			{
				string mac = MACAddress.extract(EthernetFields_Fields.ETH_DST_POS, _bytes);
				return Int64.Parse(mac.Replace(":", ""), System.Globalization.NumberStyles.HexNumber);
			}
			set
			{
				ArrayHelper.insertLong(_bytes, value, EthernetFields_Fields.ETH_DST_POS, MACAddress.WIDTH);
			}
		}

		/// <summary> Convert this ethernet packet to a readable string.</summary>
		public override System.String ToString()
		{
			return ToColoredString(false);
		}

		/// <summary> Generate string with contents describing this ethernet packet.</summary>
		/// <param name="colored">whether or not the string should contain ansi
		/// color escape sequences.
		/// </param>
		public override System.String ToColoredString(bool colored)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			buffer.Append('[');
			if (colored)
				buffer.Append(Color);
			buffer.Append("EthernetPacket");
			if (colored)
				buffer.Append(AnsiEscapeSequences_Fields.RESET);
			buffer.Append(": ");
			buffer.Append(SourceHwAddress + " -> " + DestinationHwAddress);
			buffer.Append(" proto=0x" + System.Convert.ToString(EthernetProtocol, 16));
			buffer.Append(" l=" + EthernetHeaderLength); // + "," + data.length);
			buffer.Append(']');

			return buffer.ToString();
		}
	}
}