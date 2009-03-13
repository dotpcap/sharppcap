/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> IGMP protocol field encoding information.
    /// 
    /// </summary>
    public struct IGMPFields_Fields{
        /// <summary> Length of the IGMP message type code in bytes.</summary>
        public readonly static int IGMP_CODE_LEN = 1;
        /// <summary> Length of the IGMP max response code in bytes.</summary>
        public readonly static int IGMP_MRSP_LEN = 1;
        /// <summary> Length of the IGMP header checksum in bytes.</summary>
        public readonly static int IGMP_CSUM_LEN = 2;
        /// <summary> Length of group address in bytes.</summary>
        public readonly static int IGMP_GADDR_LEN;
        /// <summary> Position of the IGMP message type.</summary>
        public readonly static int IGMP_CODE_POS = 0;
        /// <summary> Position of the IGMP max response code.</summary>
        public readonly static int IGMP_MRSP_POS;
        /// <summary> Position of the IGMP header checksum.</summary>
        public readonly static int IGMP_CSUM_POS;
        /// <summary> Position of the IGMP group address.</summary>
        public readonly static int IGMP_GADDR_POS;
        /// <summary> Length in bytes of an IGMP header.</summary>
        public readonly static int IGMP_HEADER_LEN; // 8
        static IGMPFields_Fields()
        {
            IGMP_GADDR_LEN = IPv4Fields_Fields.IP_ADDRESS_WIDTH;
            IGMP_MRSP_POS = IGMPFields_Fields.IGMP_CODE_POS + IGMPFields_Fields.IGMP_CODE_LEN;
            IGMP_CSUM_POS = IGMPFields_Fields.IGMP_MRSP_POS + IGMPFields_Fields.IGMP_MRSP_LEN;
            IGMP_GADDR_POS = IGMPFields_Fields.IGMP_CSUM_POS + IGMPFields_Fields.IGMP_CSUM_LEN;
            IGMP_HEADER_LEN = IGMPFields_Fields.IGMP_GADDR_POS + IGMPFields_Fields.IGMP_GADDR_LEN;
        }
    }
    public interface IGMPFields
    {
        //UPGRADE_NOTE: Members of interface 'IGMPFields' were extracted into structure 'IGMPFields_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
        // field lengths
        
        
        // field positions
        
        
        // complete header length 
        
    }
}
