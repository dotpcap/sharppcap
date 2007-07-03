// $Id: IPFields.cs,v 1.1.1.1 2007-07-03 10:15:17 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace Tamir.IPLib.Packets
{
	
	
	/// <summary> IP protocol field encoding information.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:17 $ </lastModifiedAt>
	public struct IPFields_Fields{
		/// <summary> Width of the IP version and header length field in bytes.</summary>
		public readonly static int IP_VER_LEN = 1;
		/// <summary> Width of the TOS field in bytes.</summary>
		public readonly static int IP_TOS_LEN = 1;
		/// <summary> Width of the header length field in bytes.</summary>
		public readonly static int IP_LEN_LEN = 2;
		/// <summary> Width of the ID field in bytes.</summary>
		public readonly static int IP_ID_LEN = 2;
		/// <summary> Width of the fragmentation bits and offset field in bytes.</summary>
		public readonly static int IP_FRAG_LEN = 2;
		/// <summary> Width of the TTL field in bytes.</summary>
		public readonly static int IP_TTL_LEN = 1;
		/// <summary> Width of the IP protocol code in bytes.</summary>
		public readonly static int IP_CODE_LEN = 1;
		/// <summary> Width of the IP checksum in bytes.</summary>
		public readonly static int IP_CSUM_LEN = 2;
		/// <summary> Position of the version code and header length within the IP header.</summary>
		public readonly static int IP_VER_POS = 0;
		/// <summary> Position of the type of service code within the IP header.</summary>
		public readonly static int IP_TOS_POS;
		/// <summary> Position of the length within the IP header.</summary>
		public readonly static int IP_LEN_POS;
		/// <summary> Position of the packet ID within the IP header.</summary>
		public readonly static int IP_ID_POS;
		/// <summary> Position of the flag bits and fragment offset within the IP header.</summary>
		public readonly static int IP_FRAG_POS;
		/// <summary> Position of the ttl within the IP header.</summary>
		public readonly static int IP_TTL_POS;
		/// <summary> Position of the IP protocol code within the IP header.</summary>
		public readonly static int IP_CODE_POS;
		/// <summary> Position of the checksum within the IP header.</summary>
		public readonly static int IP_CSUM_POS;
		/// <summary> Position of the source IP address within the IP header.</summary>
		public readonly static int IP_SRC_POS;
		/// <summary> Position of the destination IP address within a packet.</summary>
		public readonly static int IP_DST_POS;
		/// <summary> Length in bytes of an IP header, excluding options.</summary>
		public readonly static int IP_HEADER_LEN; // == 20
		static IPFields_Fields()
		{
			IP_TOS_POS = IPFields_Fields.IP_VER_POS + IPFields_Fields.IP_VER_LEN;
			IP_LEN_POS = IPFields_Fields.IP_TOS_POS + IPFields_Fields.IP_TOS_LEN;
			IP_ID_POS = IPFields_Fields.IP_LEN_POS + IPFields_Fields.IP_LEN_LEN;
			IP_FRAG_POS = IPFields_Fields.IP_ID_POS + IPFields_Fields.IP_ID_LEN;
			IP_TTL_POS = IPFields_Fields.IP_FRAG_POS + IPFields_Fields.IP_FRAG_LEN;
			IP_CODE_POS = IPFields_Fields.IP_TTL_POS + IPFields_Fields.IP_TTL_LEN;
			IP_CSUM_POS = IPFields_Fields.IP_CODE_POS + IPFields_Fields.IP_CODE_LEN;
			IP_SRC_POS = IPFields_Fields.IP_CSUM_POS + IPFields_Fields.IP_CSUM_LEN;
			IP_DST_POS = IPFields_Fields.IP_SRC_POS + IPAddress.WIDTH;
			IP_HEADER_LEN = IPFields_Fields.IP_DST_POS + IPAddress.WIDTH;
		}
	}
	public interface IPFields
	{
		//UPGRADE_NOTE: Members of interface 'IPFields' were extracted into structure 'IPFields_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
		// field lengths
		
		
		// field positions
		
		
		// complete header length 
		
	}
}