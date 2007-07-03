// $Id: Packet.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.Timeval' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using Timeval = Tamir.IPLib.Packets.Util.Timeval;
namespace Tamir.IPLib.Packets
{
	
	
	/// <summary> A network packet.
	/// <p>
	/// This class currently contains no implementation because only ethernet 
	/// is supported. In other words, all instances of packets returned by 
	/// packet factory will always be at least as specific as EthernetPacket.
	/// <p>
	/// On large ethernet networks, I sometimes see packets which don't have 
	/// link-level ethernet headers. If and when I figure out what these are, 
	/// maybe this class will be the root node of a packet hierarchy derived 
	/// from something other than ethernet.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:18 $ </lastModifiedAt>
	[Serializable]
	public class Packet
	{
		/// <summary> Fetch data portion of the packet.</summary>
		virtual public byte[] Header
		{
			get
			{
				return null;
			}
			
		}
		virtual public System.String Color
		{
			get
			{
				return "";
			}
			
		}
		virtual public Timeval Timeval
		{
			get
			{
				return null;
			}
			
		}
		virtual public byte[] Bytes
		{
			get
			{
				return null;
			}
			
		}
		virtual public PcapHeader PcapHeader
		{
			get
			{
				if(this.pcapHeader==null)
					this.pcapHeader=new PcapHeader();
				return this.pcapHeader;
			}
			
			set
			{
				this.pcapHeader = value;
			}
			
		}
		public virtual System.String ToColoredString(bool colored)
		{
			return ToString();
		}
		
		/// <summary> Fetch data portion of the packet.</summary>
		public virtual byte[] Data
		{
			get
			{
				return null;
			}
		}
		
		internal PcapHeader pcapHeader;
	}
}