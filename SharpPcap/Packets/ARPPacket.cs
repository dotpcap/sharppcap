// $Id: ARPPacket.cs,v 1.1.1.1 2007-07-03 10:15:17 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.AnsiEscapeSequences' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using AnsiEscapeSequences = Tamir.IPLib.Packets.Util.AnsiEscapeSequences;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.ArrayHelper' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ArrayHelper = Tamir.IPLib.Packets.Util.ArrayHelper;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.Timeval' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using Timeval = Tamir.IPLib.Packets.Util.Timeval;
using Tamir.IPLib.Packets.Util;

namespace Tamir.IPLib.Packets
{


	/// <summary> An ARP protocol packet.
	/// <p>
	/// Extends an ethernet packet, adding ARP header information and an ARP 
	/// data payload. 
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:17 $ </lastModifiedAt>
	[Serializable]
	public class ARPPacket : EthernetPacket, ARPFields
	{
		virtual public int ARPHwType
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ethOffset + ARPFields_Fields.ARP_HW_TYPE_POS, ARPFields_Fields.ARP_ADDR_TYPE_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ethOffset + ARPFields_Fields.ARP_HW_TYPE_POS, ARPFields_Fields.ARP_ADDR_TYPE_LEN);
			}

		}
		virtual public int ARPProtocolType
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ethOffset + ARPFields_Fields.ARP_PR_TYPE_POS, ARPFields_Fields.ARP_ADDR_TYPE_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ethOffset + ARPFields_Fields.ARP_PR_TYPE_POS, ARPFields_Fields.ARP_ADDR_TYPE_LEN);
			}

		}
		virtual public int ARPHwLength
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ethOffset + ARPFields_Fields.ARP_HW_LEN_POS, ARPFields_Fields.ARP_ADDR_SIZE_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ethOffset + ARPFields_Fields.ARP_HW_LEN_POS, ARPFields_Fields.ARP_ADDR_SIZE_LEN);
			}

		}
		virtual public int ARPProtocolLength
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ethOffset + ARPFields_Fields.ARP_PR_LEN_POS, ARPFields_Fields.ARP_ADDR_SIZE_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ethOffset + ARPFields_Fields.ARP_PR_LEN_POS, ARPFields_Fields.ARP_ADDR_SIZE_LEN);
			}

		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Fetch the operation code.
		/// Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
		/// </summary>
		/// <summary> Sets the operation code.
		/// Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
		/// </summary>
		virtual public int ARPOperation
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ethOffset + ARPFields_Fields.ARP_OP_POS, ARPFields_Fields.ARP_OP_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ethOffset + ARPFields_Fields.ARP_OP_POS, ARPFields_Fields.ARP_OP_LEN);
			}

		}
		/// <summary> Fetch the hardware source address.</summary>
		virtual public long ARPSenderHwAddressAsLong
		{
			get
			{
				return ArrayHelper.extractLong(_bytes, _ethOffset + ARPFields_Fields.ARP_S_HW_ADDR_POS, 6);
			}

		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Fetch the proto sender address.</summary>
		/// <summary> Sets the proto sender address.</summary>
		virtual public System.String ARPSenderProtoAddress
		{
			get
			{
				return IPAddress.extract(_ethOffset + ARPFields_Fields.ARP_S_PR_ADDR_POS, _bytes);
			}

			set
			{
				IPAddress.insert(_bytes, value, _ethOffset + ARPFields_Fields.ARP_S_PR_ADDR_POS);
			}

		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Fetch the proto sender address.</summary>
		/// <summary> Sets the proto sender address.</summary>
		virtual public System.String ARPTargetProtoAddress
		{
			get
			{
				return IPAddress.extract(_ethOffset + ARPFields_Fields.ARP_T_PR_ADDR_POS, _bytes);
			}

			set
			{
				IPAddress.insert(_bytes, value, _ethOffset + ARPFields_Fields.ARP_T_PR_ADDR_POS);
			}

		}
		/// <summary> Fetch the arp header, excluding arp data payload.</summary>
		virtual public byte[] ARPHeader
		{
			get
			{
				return PacketEncoding.extractHeader(_ethOffset, ARPFields_Fields.ARP_HEADER_LEN, _bytes);
			}

		}
		/// <summary> Fetch data portion of the arp header.</summary>
		virtual public byte[] ARPData
		{
			get
			{
				return PacketEncoding.extractData(_ethOffset, ARPFields_Fields.ARP_HEADER_LEN, _bytes);
			}

		}
		/// <summary> Fetch the arp header, excluding arp data payload.</summary>
		override public byte[] Header
		{
			get
			{
				return ARPHeader;
			}

		}
		/// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
		override public System.String Color
		{
			get
			{
				return AnsiEscapeSequences_Fields.PURPLE;
			}

		}
		/// <summary> Create a new ARP packet.</summary>
		public ARPPacket(int lLen, byte[] bytes)
			: base(lLen, bytes)
		{
		}

		/// <summary> Create a new ARP packet.</summary>
		public ARPPacket(int lLen, byte[] bytes, Timeval tv)
			: base(lLen, bytes, tv)
		{
		}

		/// <summary> Gets/Sets the hardware source address.</summary>
		public virtual System.String ARPSenderHwAddress
		{
			get
			{
				return MACAddress.extract(_ethOffset + ARPFields_Fields.ARP_S_HW_ADDR_POS, _bytes);
			}
			set
			{
				MACAddress.insert(value, _ethOffset + ARPFields_Fields.ARP_S_HW_ADDR_POS, _bytes);
			}
		}

		/// <summary> Sets the hardware source address.</summary>
		public virtual void setARPSenderHwAddress(long addr)
		{
			ArrayHelper.insertLong(_bytes, addr, _ethOffset + ARPFields_Fields.ARP_S_HW_ADDR_POS, 6);
		}

		/// <summary> Gets/Sets the hardware destination address.</summary>
		public virtual String ARPTargetHwAddress
		{
			get
			{
				return MACAddress.extract(_ethOffset + ARPFields_Fields.ARP_T_HW_ADDR_POS, _bytes);
			}
			set
			{
				MACAddress.insert(value, _ethOffset + ARPFields_Fields.ARP_T_HW_ADDR_POS, _bytes);
			}
		}

		/// <summary> Sets the hardware destination address.</summary>
		public virtual void setARPTargetHwAddress(long addr)
		{
			ArrayHelper.insertLong(_bytes, addr, _ethOffset + ARPFields_Fields.ARP_T_HW_ADDR_POS, 6);
		}

		/// <summary> Fetch data portion of the arp header.</summary>
		public override byte[] Data
		{
			get
			{
				return ARPData;
			}
		}

		/// <summary> Convert this ARP packet to a readable string.</summary>
		public override System.String ToString()
		{
			return ToColoredString(false);
		}

		/// <summary> Generate string with contents describing this ARP packet.</summary>
		/// <param name="colored">whether or not the string should contain ansi
		/// color escape sequences.
		/// </param>
		public override System.String ToColoredString(bool colored)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			buffer.Append('[');
			if (colored)
				buffer.Append(Color);
			buffer.Append("ARPPacket");
			if (colored)
				buffer.Append(AnsiEscapeSequences_Fields.RESET);
			buffer.Append(": ");
			buffer.Append(ARPOperation == ARPFields_Fields.ARP_OP_REQ_CODE ? "request" : "reply");
			buffer.Append(' ');
			buffer.Append(ARPSenderHwAddress + " -> " + ARPTargetHwAddress);
			buffer.Append(", ");
			buffer.Append(ARPSenderProtoAddress + " -> " + ARPTargetProtoAddress);
			//buffer.append(" l=" + header.length + "," + data.length);
			buffer.Append(']');

			return buffer.ToString();
		}
	}
}