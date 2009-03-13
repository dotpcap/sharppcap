/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> Information about network link layers.
    /// 
    /// </summary>
    public class LinkLayer : LinkLayers
    {
        /// <summary> Fetch the header length associated with various link-layer types.</summary>
        /// <param name="layerType">the link-layer code
        /// </param>
        /// <returns> the length of the header for the specified link-layer
        /// </returns>
        public static int getLinkLayerLength(int layerType)
        {
            switch (layerType)
            {
                
                case LinkLayers_Fields.ARCNET: 
                    return 6;
                
                case LinkLayers_Fields.SLIP: 
                    return 16;
                
                case LinkLayers_Fields.SLIP_BSDOS: 
                    return 24;
                
                case LinkLayers_Fields.NULL: 
                case LinkLayers_Fields.LOOP: 
                    return 4;
                
                case LinkLayers_Fields.PPP: 
                case LinkLayers_Fields.CHDLC: 
                case LinkLayers_Fields.PPP_SERIAL: 
                    return 4;
                
                case LinkLayers_Fields.PPP_BSDOS: 
                    return 24;
                
                case LinkLayers_Fields.FDDI: 
                    return 21;
                
                case LinkLayers_Fields.IEEE802_11: 
                    return 22;
                
                case LinkLayers_Fields.ATM_RFC1483: 
                    return 8;
                
                case LinkLayers_Fields.RAW: 
                    return 0;
                
                case LinkLayers_Fields.ATM_CLIP: 
                    return 8;
                
                case LinkLayers_Fields.LINUX_SLL: 
                    return 16;
                
                case LinkLayers_Fields.EN10MB: 
                default: 
                    return 14;
                }
        }
        
        /// <summary> Fetch the offset into the link-layer header where the protocol code
        /// can be found. Returns -1 if there is no embedded protocol code.
        /// </summary>
        /// <param name="layerType">the link-layer code
        /// </param>
        /// <returns> the offset in bytes
        /// </returns>
        public static int getProtoOffset(int layerType)
        {
            switch (layerType)
            {
                
                case LinkLayers_Fields.ARCNET: 
                    return 2;
                
                case LinkLayers_Fields.SLIP: 
                    return - 1;
                
                case LinkLayers_Fields.SLIP_BSDOS: 
                    return - 1;
                
                case LinkLayers_Fields.NULL: 
                case LinkLayers_Fields.LOOP: 
                    return 0;
                
                case LinkLayers_Fields.PPP: 
                case LinkLayers_Fields.CHDLC: 
                case LinkLayers_Fields.PPP_SERIAL: 
                    return 2;
                
                case LinkLayers_Fields.PPP_BSDOS: 
                    return 5;
                
                case LinkLayers_Fields.FDDI: 
                    return 13;
                
                case LinkLayers_Fields.IEEE802_11: 
                    return 14;
                
                case LinkLayers_Fields.ATM_RFC1483: 
                    return 6;
                
                case LinkLayers_Fields.RAW: 
                    return - 1;
                
                case LinkLayers_Fields.ATM_CLIP: 
                    return 6;
                
                case LinkLayers_Fields.LINUX_SLL: 
                    return 14;
                
                case LinkLayers_Fields.EN10MB: 
                default: 
                    return 12;
                }
        }
        
        /// <summary> Fetch a link-layer type description.</summary>
        /// <param name="code">the code associated with the description.
        /// </param>
        /// <returns> a description of the link-layer type.
        /// </returns>
        public static System.String getDescription(int code)
        {
            System.Int32 c = (System.Int32) code;
            if (descriptions.ContainsKey(c))
            {
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                return (System.String) descriptions[c];
            }
            else
                return "unknown";
        }
        
        /// <summary> 'Human-readable' link-layer type descriptions.</summary>
        //UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
        private static System.Collections.Hashtable descriptions = new System.Collections.Hashtable();
        
        static LinkLayer()
        {
            {
                descriptions[(System.Int32) LinkLayers_Fields.NULL] = "no link-layer encapsulation";
                descriptions[(System.Int32) LinkLayers_Fields.EN10MB] = "10/100Mb ethernet";
                descriptions[(System.Int32) LinkLayers_Fields.EN3MB] = "3Mb experimental ethernet";
                descriptions[(System.Int32) LinkLayers_Fields.AX25] = "AX.25 amateur radio";
                descriptions[(System.Int32) LinkLayers_Fields.PRONET] = "proteon pronet token ring";
                descriptions[(System.Int32) LinkLayers_Fields.CHAOS] = "chaos";
                descriptions[(System.Int32) LinkLayers_Fields.IEEE802] = "IEEE802 network";
                descriptions[(System.Int32) LinkLayers_Fields.ARCNET] = "ARCNET";
                descriptions[(System.Int32) LinkLayers_Fields.SLIP] = "serial line IP";
                descriptions[(System.Int32) LinkLayers_Fields.PPP] = "point-to-point protocol";
                descriptions[(System.Int32) LinkLayers_Fields.FDDI] = "FDDI";
                descriptions[(System.Int32) LinkLayers_Fields.ATM_RFC1483] = "LLC/SNAP encapsulated ATM";
                descriptions[(System.Int32) LinkLayers_Fields.RAW] = "raw IP";
                descriptions[(System.Int32) LinkLayers_Fields.SLIP_BSDOS] = "BSD SLIP";
                descriptions[(System.Int32) LinkLayers_Fields.PPP_BSDOS] = "BSD PPP";
                descriptions[(System.Int32) LinkLayers_Fields.ATM_CLIP] = "IP over ATM";
                descriptions[(System.Int32) LinkLayers_Fields.PPP_SERIAL] = "PPP over HDLC";
                descriptions[(System.Int32) LinkLayers_Fields.CHDLC] = "Cisco HDLC";
                descriptions[(System.Int32) LinkLayers_Fields.IEEE802_11] = "802.11 wireless";
                descriptions[(System.Int32) LinkLayers_Fields.LOOP] = "OpenBSD loopback";
                descriptions[(System.Int32) LinkLayers_Fields.LINUX_SLL] = "Linux cooked sockets";
                descriptions[(System.Int32) LinkLayers_Fields.UNKNOWN] = "unknown link-layer type";
            }
        }
    }
}
