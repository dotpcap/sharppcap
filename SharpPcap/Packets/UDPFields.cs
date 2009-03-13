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
    public struct UDPFields_Fields{
        /// <summary> Length of a UDP port in bytes.</summary>
        public readonly static int UDP_PORT_LEN = 2;
        /// <summary> Length of the header length field in bytes.</summary>
        public readonly static int UDP_LEN_LEN = 2;
        /// <summary> Length of the checksum field in bytes.</summary>
        public readonly static int UDP_CSUM_LEN = 2;
        /// <summary> Position of the source port.</summary>
        public readonly static int UDP_SP_POS = 0;
        /// <summary> Position of the destination port.</summary>
        public readonly static int UDP_DP_POS;
        /// <summary> Position of the header length.</summary>
        public readonly static int UDP_LEN_POS;
        /// <summary> Position of the header checksum length.</summary>
        public readonly static int UDP_CSUM_POS;
        /// <summary> Length of a UDP header in bytes.</summary>
        public readonly static int UDP_HEADER_LEN; // == 8
        static UDPFields_Fields()
        {
            UDP_DP_POS = UDPFields_Fields.UDP_PORT_LEN;
            UDP_LEN_POS = UDPFields_Fields.UDP_DP_POS + UDPFields_Fields.UDP_PORT_LEN;
            UDP_CSUM_POS = UDPFields_Fields.UDP_LEN_POS + UDPFields_Fields.UDP_LEN_LEN;
            UDP_HEADER_LEN = UDPFields_Fields.UDP_CSUM_POS + UDPFields_Fields.UDP_CSUM_LEN;
        }
    }
    public interface UDPFields
    {
        //UPGRADE_NOTE: Members of interface 'UDPFields' were extracted into structure 'UDPFields_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
        // field lengths
        
        
        // field positions
        
        
        // complete header length 
        
    }
}
