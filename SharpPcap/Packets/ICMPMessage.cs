/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> ICMP message utility class.
    /// 
    /// </summary>
    public class ICMPMessage : ICMPMessages
    {
        /// <summary> Fetch an ICMP message.</summary>
        /// <param name="code">the code associated with the message.
        /// </param>
        /// <returns> a message describing the significance of the ICMP code.
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
        
        /// <summary> 'Human-readable' ICMP messages.</summary>
        //UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
        private static System.Collections.Hashtable messages = new System.Collections.Hashtable();
        
        static ICMPMessage()
        {
            {
                messages[(System.Int32) ICMPMessages_Fields.ECHO_REPLY] = "echo reply";
                messages[(System.Int32) ICMPMessages_Fields.ECHO] = "echo request";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_NET] = "net unreachable";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_HOST] = "host unreachable";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_PROTOCOL] = "bad protocol";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_PORT] = "port unreachable";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_NEEDFRAG] = "ip_df drop";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_SRCFAIL] = "source route failed";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_NET_UNKNOWN] = "unknown network";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_HOST_UNKNOWN] = "unknown host";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_ISOLATED] = "source host isolated";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_NET_PROHIB] = "net access prohibited";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_HOST_PROHIB] = "host access prohibited";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_TOSNET] = "tos for net invalid";
                messages[(System.Int32) ICMPMessages_Fields.UNREACH_TOSHOST] = "tos for host invalid";
                messages[(System.Int32) ICMPMessages_Fields.SOURCE_QUENCH] = "packet lost";
                messages[(System.Int32) ICMPMessages_Fields.REDIRECT_NET] = "redirect to network";
                messages[(System.Int32) ICMPMessages_Fields.REDIRECT_HOST] = "redirect to host";
                messages[(System.Int32) ICMPMessages_Fields.REDIRECT_TOSNET] = "tos redirect to network";
                messages[(System.Int32) ICMPMessages_Fields.REDIRECT_TOSHOST] = "tos redirect to host";
                messages[(System.Int32) ICMPMessages_Fields.ROUTER_ADVERT] = "router advert";
                messages[(System.Int32) ICMPMessages_Fields.ROUTER_SOLICIT] = "router solicit";
                messages[(System.Int32) ICMPMessages_Fields.TIME_EXCEED_INTRANS] = "transit time exceeded";
                messages[(System.Int32) ICMPMessages_Fields.TIME_EXCEED_REASS] = "reass time exceeded";
                messages[(System.Int32) ICMPMessages_Fields.PARAM_PROB] = "bad ip header";
                messages[(System.Int32) ICMPMessages_Fields.TSTAMP] = "timestamp request";
                messages[(System.Int32) ICMPMessages_Fields.TSTAMP_REPLY] = "timestamp reply";
                messages[(System.Int32) ICMPMessages_Fields.IREQ] = "information request";
                messages[(System.Int32) ICMPMessages_Fields.IREQ_REPLY] = "information reply";
                messages[(System.Int32) ICMPMessages_Fields.MASK_REQ] = "address mask request";
                messages[(System.Int32) ICMPMessages_Fields.MASK_REPLY] = "address mask reply";
            }
        }
    }
}
