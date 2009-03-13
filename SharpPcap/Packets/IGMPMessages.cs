/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> Code constants for IGMP message types.
    /// 
    /// From RFC #2236.
    /// 
    /// </summary>
    public struct IGMPMessages_Fields{
        /// <summary> membership query.</summary>
        public readonly static int QUERY = 0x11;
        /// <summary> v1 membership report.</summary>
        public readonly static int V1_REPORT = 0x12;
        /// <summary> v2 membership report.</summary>
        public readonly static int V2_REPORT = 0x16;
        /// <summary> Leave group.</summary>
        public readonly static int LEAVE = 0x17;
    }
    public interface IGMPMessages
    {
        //UPGRADE_NOTE: Members of interface 'IGMPMessages' were extracted into structure 'IGMPMessages_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
        
    }
}
