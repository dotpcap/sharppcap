// $Id: TCPPacket.cs,v 1.3 2007-07-30 09:26:03 tamirgal Exp $

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
namespace Tamir.IPLib.Packets
{

	/// <summary> A TCP packet.
	/// <p>
	/// Extends an IP packet, adding a TCP header and TCP data payload.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.3 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-30 09:26:03 $ </lastModifiedAt>
	[Serializable]
	public class TCPPacket : IPPacket, TCPFields
	{
		/// <summary> Fetch the port number on the source host.</summary>
		virtual public int SourcePort
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ipOffset + TCPFields_Fields.TCP_SP_POS, TCPFields_Fields.TCP_PORT_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ipOffset + TCPFields_Fields.TCP_SP_POS, TCPFields_Fields.TCP_PORT_LEN);
			}

		}
		/// <summary> Fetches the port number on the destination host.</summary>
		virtual public int DestinationPort
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ipOffset + TCPFields_Fields.TCP_DP_POS, TCPFields_Fields.TCP_PORT_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ipOffset + TCPFields_Fields.TCP_DP_POS, TCPFields_Fields.TCP_PORT_LEN);
			}

		}
		/// <summary> Fetch the packet sequence number.</summary>
		virtual public long SequenceNumber
		{
			get
			{
				return ArrayHelper.extractLong(_bytes, _ipOffset + TCPFields_Fields.TCP_SEQ_POS, TCPFields_Fields.TCP_SEQ_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ipOffset + TCPFields_Fields.TCP_SEQ_POS, TCPFields_Fields.TCP_SEQ_LEN);
			}

		}
		/// <summary>    Fetch the packet acknowledgment number.</summary>
		virtual public long AcknowledgmentNumber
		{
			get
			{
				return ArrayHelper.extractLong(_bytes, _ipOffset + TCPFields_Fields.TCP_ACK_POS, TCPFields_Fields.TCP_ACK_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ipOffset + TCPFields_Fields.TCP_ACK_POS, TCPFields_Fields.TCP_ACK_LEN);
			}

		}
		/// <summary> Fetch the packet acknowledgment number. </summary>
		virtual public long AcknowledgementNumber
		{
			get
			{
				return AcknowledgmentNumber;
			}
			set
			{
				AcknowledgmentNumber = value;
			}

		}
		/// <summary> Fetch the TCP header length in bytes.</summary>
		virtual public int TCPHeaderLength
		{
			get
			{
				return ((ArrayHelper.extractInteger(_bytes, _ipOffset + TCPFields_Fields.TCP_FLAG_POS, TCPFields_Fields.TCP_FLAG_LEN) >> 12) & 0xf) * 4;
			}

			set
			{
				value = value / 4;
				_bytes[_ipOffset + TCPFields_Fields.TCP_FLAG_POS] &= (byte)(0x0f);
				_bytes[_ipOffset + TCPFields_Fields.TCP_FLAG_POS] |= (byte)(((value << 4) & 0xf0));
			}

		}
				/// <summary> Fetch the TCP header length in bytes.</summary>
		virtual public int TcpHeaderLength
		{
			get
			{
				return TCPHeaderLength;
			}
			set
			{
				TCPHeaderLength = value;
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
				return (IPTotalLength - IPHeaderLength - TcpHeaderLength);
			}

		}
		/// <summary> Fetch the window size.</summary>
		virtual public int WindowSize
		{
			get
			{
				return ArrayHelper.extractInteger(_bytes, _ipOffset + TCPFields_Fields.TCP_WIN_POS, TCPFields_Fields.TCP_WIN_LEN);
			}

			set
			{
				ArrayHelper.insertLong(_bytes, value, _ipOffset + TCPFields_Fields.TCP_WIN_POS, TCPFields_Fields.TCP_WIN_LEN);
			}

		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Fetch the header checksum.</summary>
		/// <summary> Set the checksum of the TCP header</summary>
		/// <param name="cs">the checksum value
		/// </param>
		virtual public int TCPChecksum
		{
			get
			{
				return GetTransportLayerChecksum(_ipOffset + TCPFields_Fields.TCP_CSUM_POS);
			}

			set
			{
				base.SetTransportLayerChecksum(value, TCPFields_Fields.TCP_CSUM_POS);
			}

		}
		/// <summary> Check if the TCP packet is valid, checksum-wise.</summary>
		override public bool ValidChecksum
		{
			get
			{
				return ValidTCPChecksum;
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
				return IPTotalLength - IpHeaderLength;
			}

		}
		private int AllFlags
		{
			get
			{
				if (!_allFlagsSet)
				{
					_allFlags = ArrayHelper.extractInteger(_bytes, _ipOffset + TCPFields_Fields.TCP_FLAG_POS, TCPFields_Fields.TCP_FLAG_LEN);
					//tamir: added
					_allFlagsSet = true;
				}
				return _allFlags;
			}

			set
			{

				ArrayHelper.insertLong(_bytes, value, _ipOffset + TCPFields_Fields.TCP_FLAG_POS, TCPFields_Fields.TCP_FLAG_LEN);
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
					_tcpHeaderBytes = PacketEncoding.extractHeader(_ipOffset, TcpHeaderLength, _bytes);
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
					// set data length based on info in headers (note: tcpdump
					//  can return extra junk bytes which bubble up to here
					_tcpDataBytes = PacketEncoding.extractData(_ipOffset, TcpHeaderLength, _bytes, PayloadDataLength);
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

		/// <summary> Create a new TCP packet.</summary>
		public TCPPacket(int lLen, byte[] bytes)
			: this(lLen, bytes, false)
		{
		}

		/// <summary> Create a new TCP packet.</summary>
		public TCPPacket(int lLen, byte[] bytes, bool isEmpty)
			: base(lLen, bytes)
		{
			//tamir
			if (!isEmpty)
			{
				int generatedAux = TcpHeaderLength;
			}
		}

		/// <summary> Create a new TCP packet.</summary>
		public TCPPacket(int lLen, byte[] bytes, Timeval tv)
			: this(lLen, bytes)
		{
			this._timeval = tv;
		}

		/// <summary> Fetch the header checksum.</summary>
		public override int Checksum
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
			return base.ComputeTransportLayerChecksum(TCPFields_Fields.TCP_CSUM_POS, true, true);
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
				_urgentPointer = ArrayHelper.extractInteger(_bytes, _ipOffset + TCPFields_Fields.TCP_URG_POS, TCPFields_Fields.TCP_URG_LEN);
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
			ArrayHelper.insertLong(_bytes, pointer, _ipOffset + TCPFields_Fields.TCP_URG_POS, TCPFields_Fields.TCP_URG_LEN);
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
		public virtual void SetData(byte[] data)
		{
			byte[] headers = ArrayHelper.copy(_bytes, 0, TcpHeaderLength+IpHeaderLength+EthernetHeaderLength);
			byte[] newBytes = ArrayHelper.join(headers, data);
			this._bytes = newBytes;
			TCPHeaderLength = _bytes.Length-data.Length-IpHeaderLength-EthernetHeaderLength;
		
			//update ip total length length
			IPTotalLength = IpHeaderLength + TcpHeaderLength + data.Length;
			//update also offset and pcap header
			OnOffsetChanged();
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
				buffer.Append(" syn[0x" + System.Convert.ToString(SequenceNumber, 16) + "]");
			if (Fin)
				buffer.Append(" fin");
			buffer.Append(" l=" + TCPHeaderLength + "," + PayloadDataLength);
			buffer.Append(']');

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

			return buffer.ToString();
		}

		public static TCPPacket RandomPacket()
		{
			return RandomPacket(54);
		}

		public static TCPPacket RandomPacket(int size)
		{
			if(size<54)
				throw new Exception("Size should be at least 54 (Eth + IP + TCP)");

			byte[] bytes = new byte[size];
			Tamir.IPLib.Util.Rand.Instance.GetBytes(bytes);
			TCPPacket tcp = new TCPPacket(14, bytes, true);
			MakeValid(tcp);
			return tcp;
		}

		public static void MakeValid(TCPPacket tcp)
		{
			tcp.IPVersion = 4;
			tcp.IPTotalLength = tcp.Bytes.Length-14;			//Set the correct IP length
			tcp.IPHeaderLength = IPFields_Fields.IP_HEADER_LEN;
			tcp.TCPHeaderLength = TCPFields_Fields.TCP_HEADER_LEN;			//Set the correct TCP header length
			//Calculate checksums
			tcp.ComputeIPChecksum();
			tcp.ComputeTCPChecksum();
		}
	}
}