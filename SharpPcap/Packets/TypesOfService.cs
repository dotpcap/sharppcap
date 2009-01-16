// $Id: TypesOfService.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace SharpPcap.Packets
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
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:18 $ </lastModifiedAt>
	public struct TypesOfService_Fields{
		public readonly static int MINIMIZE_DELAY = 0x10;
		public readonly static int MAXIMIZE_THROUGHPUT = 0x08;
		public readonly static int MAXIMIZE_RELIABILITY = 0x04;
		public readonly static int MINIMIZE_MONETARY_COST = 0x02;
		public readonly static int UNUSED = 0x01;
	}
	public interface TypesOfService
	{
		//UPGRADE_NOTE: Members of interface 'TypesOfService' were extracted into structure 'TypesOfService_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
		
	}
}