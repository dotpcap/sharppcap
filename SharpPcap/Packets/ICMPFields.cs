/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> ICMP protocol field encoding information.
    /// 
    /// </summary>
    public struct ICMPFields_Fields{
        /// <summary> Length of the ICMP message type code in bytes.</summary>
        public readonly static int ICMP_CODE_LEN = 1;
        /// <summary> Length of the ICMP subcode in bytes.</summary>
        public readonly static int ICMP_SUBC_LEN = 1;
        /// <summary> Length of the ICMP header checksum in bytes.</summary>
        public readonly static int ICMP_CSUM_LEN = 2;
        /// <summary> Position of the ICMP message type.</summary>
        public readonly static int ICMP_CODE_POS = 0;
        /// <summary> Position of the ICMP message subcode.</summary>
        public readonly static int ICMP_SUBC_POS;
        /// <summary> Position of the ICMP header checksum.</summary>
        public readonly static int ICMP_CSUM_POS;
        /// <summary> Length in bytes of an ICMP header.</summary>
        public readonly static int ICMP_HEADER_LEN; // == 4
        static ICMPFields_Fields()
        {
            ICMP_SUBC_POS = ICMPFields_Fields.ICMP_CODE_POS + ICMPFields_Fields.ICMP_CODE_LEN;
            ICMP_CSUM_POS = ICMPFields_Fields.ICMP_SUBC_POS + ICMPFields_Fields.ICMP_CODE_LEN;
            ICMP_HEADER_LEN = ICMPFields_Fields.ICMP_CSUM_POS + ICMPFields_Fields.ICMP_CSUM_LEN;
        }
    }
    public interface ICMPFields
    {
        //UPGRADE_NOTE: Members of interface 'ICMPFields' were extracted into structure 'ICMPFields_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
        // field lengths
        
        
        // field positions
        
        
        // complete header length 
        
    }
}
