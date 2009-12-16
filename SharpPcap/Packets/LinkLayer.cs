// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************
namespace SharpPcap.Packets
{
    /// <summary> Information about network link layers.
    /// 
    /// </summary>
    public class LinkLayer
    {
        /// <summary> Fetch the header length associated with various link-layer types.</summary>
        /// <param name="layerType">the link-layer code
        /// </param>
        /// <returns> the length of the header for the specified link-layer
        /// </returns>
        public static int LinkLayerLength(LinkLayers layerType)
        {
            switch (layerType)
            {                
            case LinkLayers.ArcNet: 
                return 6;

            case LinkLayers.Slip:
                return 16;

            case LinkLayers.SlipBSD:
                return 24;

            case LinkLayers.Null:
            case LinkLayers.Loop:
                return 4;

            case LinkLayers.Ppp:
            case LinkLayers.CiscoHDLC:
            case LinkLayers.PppSerial:
                return 4;

            case LinkLayers.PppBSD:
                return 24;

            case LinkLayers.Fddi:
                return 21;

            case LinkLayers.Ieee80211:
                return 22;

            case LinkLayers.AtmRfc1483:
                return 8;

            case LinkLayers.Raw:
                return 0;

            case LinkLayers.AtmClip:
                return 8;

            case LinkLayers.LinuxSLL:
                return 16;

            case LinkLayers.Ethernet10Mb:
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
        public static int ProtocolOffset(LinkLayers layerType)
        {
            switch (layerType)
            {
            case LinkLayers.ArcNet: 
                return 2;

            case LinkLayers.Slip:
                return -1;

            case LinkLayers.SlipBSD:
                return -1;

            case LinkLayers.Null:
            case LinkLayers.Loop:
                return 0;

            case LinkLayers.Ppp:
            case LinkLayers.CiscoHDLC:
            case LinkLayers.PppSerial:
                return 2;

            case LinkLayers.PppBSD:
                return 5;

            case LinkLayers.Fddi:
                return 13;

            case LinkLayers.Ieee80211:
                return 14;

            case LinkLayers.AtmRfc1483:
                return 6;

            case LinkLayers.Raw:
                return -1;

            case LinkLayers.AtmClip:
                return 6;

            case LinkLayers.LinuxSLL:
                return 14;

            case LinkLayers.Ethernet10Mb:
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
        private static System.Collections.Hashtable descriptions = new System.Collections.Hashtable();

        static LinkLayer()
        {
            {
                descriptions[(System.Int32) LinkLayers.Null] = "no link-layer encapsulation";
                descriptions[(System.Int32) LinkLayers.Ethernet10Mb] = "10/100Mb ethernet";
                descriptions[(System.Int32) LinkLayers.ExperimentalEthernet3MB] = "3Mb experimental ethernet";
                descriptions[(System.Int32) LinkLayers.AmateurRadioAX25] = "AX.25 amateur radio";
                descriptions[(System.Int32) LinkLayers.ProteonProNetTokenRing] = "proteon pronet token ring";
                descriptions[(System.Int32) LinkLayers.Chaos] = "chaos";
                descriptions[(System.Int32) LinkLayers.Ieee802] = "IEEE802 network";
                descriptions[(System.Int32) LinkLayers.ArcNet] = "ARCNET";
                descriptions[(System.Int32) LinkLayers.Slip] = "serial line IP";
                descriptions[(System.Int32) LinkLayers.Ppp] = "point-to-point protocol";
                descriptions[(System.Int32) LinkLayers.Fddi] = "FDDI";
                descriptions[(System.Int32) LinkLayers.AtmRfc1483] = "LLC/SNAP encapsulated ATM";
                descriptions[(System.Int32) LinkLayers.Raw] = "raw IP";
                descriptions[(System.Int32) LinkLayers.SlipBSD] = "BSD SLIP";
                descriptions[(System.Int32) LinkLayers.PppBSD] = "BSD PPP";
                descriptions[(System.Int32) LinkLayers.AtmClip] = "IP over ATM";
                descriptions[(System.Int32) LinkLayers.PppSerial] = "PPP over HDLC";
                descriptions[(System.Int32) LinkLayers.CiscoHDLC] = "Cisco HDLC";
                descriptions[(System.Int32) LinkLayers.Ieee80211] = "802.11 wireless";
                descriptions[(System.Int32) LinkLayers.Loop] = "OpenBSD loopback";
                descriptions[(System.Int32) LinkLayers.LinuxSLL] = "Linux cooked sockets";
                descriptions[(System.Int32) LinkLayers.Unknown] = "unknown link-layer type";
            }
        }
    }
}
