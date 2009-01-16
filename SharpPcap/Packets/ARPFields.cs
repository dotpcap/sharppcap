// $Id: ARPFields.cs,v 1.1.1.1 2007-07-03 10:15:17 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace SharpPcap.Packets
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
	public struct ARPFields_Fields{
		/// <summary> Type code for ethernet addresses.</summary>
		public readonly static int ARP_ETH_ADDR_CODE = 0x0001;
		/// <summary> Type code for MAC addresses.</summary>
		public readonly static int ARP_IP_ADDR_CODE = 0x0800;
		/// <summary> Code for ARP request.</summary>
		public readonly static int ARP_OP_REQ_CODE = 0x1;
		/// <summary> Code for ARP reply.</summary>
		public readonly static int ARP_OP_REP_CODE = 0x2;
		/// <summary> Operation type length in bytes.</summary>
		public readonly static int ARP_OP_LEN = 2;
		/// <summary> Address type length in bytes.</summary>
		public readonly static int ARP_ADDR_TYPE_LEN = 2;
		/// <summary> Address type length in bytes.</summary>
		public readonly static int ARP_ADDR_SIZE_LEN = 1;
		/// <summary> Position of the hardware address type.</summary>
		public readonly static int ARP_HW_TYPE_POS = 0;
		/// <summary> Position of the protocol address type.</summary>
		public readonly static int ARP_PR_TYPE_POS;
		/// <summary> Position of the hardware address length.</summary>
		public readonly static int ARP_HW_LEN_POS;
		/// <summary> Position of the protocol address length.</summary>
		public readonly static int ARP_PR_LEN_POS;
		/// <summary> Position of the operation type.</summary>
		public readonly static int ARP_OP_POS;
		/// <summary> Position of the sender hardware address.</summary>
		public readonly static int ARP_S_HW_ADDR_POS;
		/// <summary> Position of the sender protocol address.</summary>
		public readonly static int ARP_S_PR_ADDR_POS;
		/// <summary> Position of the target hardware address.</summary>
		public readonly static int ARP_T_HW_ADDR_POS;
		/// <summary> Position of the target protocol address.</summary>
		public readonly static int ARP_T_PR_ADDR_POS;
		/// <summary> Total length in bytes of an ARP header.</summary>
		public readonly static int ARP_HEADER_LEN; // == 28
		static ARPFields_Fields()
		{
			ARP_PR_TYPE_POS = ARPFields_Fields.ARP_HW_TYPE_POS + ARPFields_Fields.ARP_ADDR_TYPE_LEN;
			ARP_HW_LEN_POS = ARPFields_Fields.ARP_PR_TYPE_POS + ARPFields_Fields.ARP_ADDR_TYPE_LEN;
			ARP_PR_LEN_POS = ARPFields_Fields.ARP_HW_LEN_POS + ARPFields_Fields.ARP_ADDR_SIZE_LEN;
			ARP_OP_POS = ARPFields_Fields.ARP_PR_LEN_POS + ARPFields_Fields.ARP_ADDR_SIZE_LEN;
			ARP_S_HW_ADDR_POS = ARPFields_Fields.ARP_OP_POS + ARPFields_Fields.ARP_OP_LEN;
			ARP_S_PR_ADDR_POS = ARPFields_Fields.ARP_S_HW_ADDR_POS + MACAddress.WIDTH;
			ARP_T_HW_ADDR_POS = ARPFields_Fields.ARP_S_PR_ADDR_POS + IPAddress.WIDTH;
			ARP_T_PR_ADDR_POS = ARPFields_Fields.ARP_T_HW_ADDR_POS + MACAddress.WIDTH;
			ARP_HEADER_LEN = ARPFields_Fields.ARP_T_PR_ADDR_POS + IPAddress.WIDTH;
		}
	}
	public interface ARPFields
	{
		//UPGRADE_NOTE: Members of interface 'ARPFields' were extracted into structure 'ARPFields_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
		// ARP codes
		
		
		// field lengths 
		
		
		// field positions 
		
		
		// complete header length
		
	}
}