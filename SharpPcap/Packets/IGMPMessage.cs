/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> IGMP message utility class.
    /// 
    /// </summary>
    public class IGMPMessage : IGMPMessages
    {
        /// <summary> Fetch an IGMP message.</summary>
        /// <param name="code">the code associated with the message.
        /// </param>
        /// <returns> a message describing the significance of the IGMP code.
        /// </returns>
        public static System.String getDescription(int code)
        {
            System.Int32 c = (System.Int32) code;
            if (messages.ContainsKey(c))
            {
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                return (System.String) messages[c];
            }
            else
                return "unknown";
        }
        
        /// <summary> 'Human-readable' IGMP messages.</summary>
        //UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
        private static System.Collections.Hashtable messages = new System.Collections.Hashtable();
        
        static IGMPMessage()
        {
            {
                messages[(System.Int32) IGMPMessages_Fields.LEAVE] = "leave group";
                messages[(System.Int32) IGMPMessages_Fields.V1_REPORT] = "v1 membership report";
                messages[(System.Int32) IGMPMessages_Fields.V2_REPORT] = "v2 membership report";
                messages[(System.Int32) IGMPMessages_Fields.QUERY] = "membership query";
            }
        }
    }
}
