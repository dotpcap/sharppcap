/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> Ethernet protocol field encoding information.
    /// 
    /// </summary>
    public struct EthernetFields_Fields{
        /// <summary> Width of the ethernet type code in bytes.</summary>
        public readonly static int ETH_CODE_LEN = 2;
        /// <summary> Position of the destination MAC address within the ethernet header.</summary>
        public readonly static int ETH_DST_POS = 0;
        /// <summary> Position of the source MAC address within the ethernet header.</summary>
        public readonly static int ETH_SRC_POS;
        /// <summary> Position of the ethernet type field within the ethernet header.</summary>
        public readonly static int ETH_CODE_POS;
        /// <summary> Total length of an ethernet header in bytes.</summary>
        public readonly static int ETH_HEADER_LEN; // == 14
        static EthernetFields_Fields()
        {
            ETH_SRC_POS = EthernetFields_Fields.MAC_ADDRESS_LENGTH;
            ETH_CODE_POS = EthernetFields_Fields.MAC_ADDRESS_LENGTH * 2;
            ETH_HEADER_LEN = EthernetFields_Fields.ETH_CODE_POS + EthernetFields_Fields.ETH_CODE_LEN;
        }

        // size of an ethernet mac address in bytes
        public readonly static int MAC_ADDRESS_LENGTH = 6;
    }
    public interface EthernetFields
    {
        //UPGRADE_NOTE: Members of interface 'EthernetFields' were extracted into structure 'EthernetFields_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
        // field lengths
        
        
        // field positions
        
        
        // complete header length
        
    }
}
