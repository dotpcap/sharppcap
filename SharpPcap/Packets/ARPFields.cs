/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> IP protocol field encoding information.
    /// 
    /// </summary>
    /// FIXME: These fields are partially broken because they assume the offset for
    /// several fields and the offset is actually based on the accumulated offset
    /// into the structure determined by the fields that indicate sizes
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
            // NOTE: We use IPv4Fields_Fields.IP_ADDRESS_WIDTH because arp packets are
            //       only used in IPv4 networks. Neighbor discovery is used with IPv6
            //FIXME: we really should use the sizes given by the length fields to determine
            // the position offsets here instead of assuming the hw address is an ethernet mac address
            ARP_PR_TYPE_POS = ARPFields_Fields.ARP_HW_TYPE_POS + ARPFields_Fields.ARP_ADDR_TYPE_LEN;
            ARP_HW_LEN_POS = ARPFields_Fields.ARP_PR_TYPE_POS + ARPFields_Fields.ARP_ADDR_TYPE_LEN;
            ARP_PR_LEN_POS = ARPFields_Fields.ARP_HW_LEN_POS + ARPFields_Fields.ARP_ADDR_SIZE_LEN;
            ARP_OP_POS = ARPFields_Fields.ARP_PR_LEN_POS + ARPFields_Fields.ARP_ADDR_SIZE_LEN;
            ARP_S_HW_ADDR_POS = ARPFields_Fields.ARP_OP_POS + ARPFields_Fields.ARP_OP_LEN;
            ARP_S_PR_ADDR_POS = ARPFields_Fields.ARP_S_HW_ADDR_POS + EthernetFields_Fields.MAC_ADDRESS_LENGTH;
            ARP_T_HW_ADDR_POS = ARPFields_Fields.ARP_S_PR_ADDR_POS + IPv4Fields_Fields.IP_ADDRESS_WIDTH;
            ARP_T_PR_ADDR_POS = ARPFields_Fields.ARP_T_HW_ADDR_POS + EthernetFields_Fields.MAC_ADDRESS_LENGTH;
            ARP_HEADER_LEN = ARPFields_Fields.ARP_T_PR_ADDR_POS + IPv4Fields_Fields.IP_ADDRESS_WIDTH;
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
