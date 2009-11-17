using System;

namespace SharpPcap.Packets
{
    /// Copied from Pcap.Net @ 20091117
    /// 
    /// <summary>
    /// Code constants for well-defined ethernet protocols.
    /// 
    /// EtherType is a two-octet field in an Ethernet frame, as defined by the Ethernet II framing networking standard. 
    /// It is used to indicate which protocol is encapsulated in the payload.
    ///
    /// Also contains entries taken from linux/if_ether.h and tcpdump/ethertype.h
    /// </summary>
    public enum EthernetPacketType : ushort
    {
        /// <summary>
        /// No Ethernet type
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// Internet Protocol, Version 4 (IPv4)
        /// </summary>
        IpV4 = 0x0800,
        /// <summary>
        /// Address Resolution Protocol (ARP)
        /// </summary>
        Arp = 0x0806,
        /// <summary>
        /// Reverse Address Resolution Protocol (RARP)
        /// </summary>
        ReverseArp = 0x8035,
        /// <summary>
        /// AppleTalk (Ethertalk)
        /// </summary>
        AppleTalk = 0x809B,
        /// <summary>
        /// AppleTalk Address Resolution Protocol (AARP)
        /// </summary>
        AppleTalkArp = 0x80F3,
        /// <summary>
        /// VLAN-tagged frame (IEEE 802.1Q)
        /// </summary>
        VLanTaggedFrame = 0x8100,
        /// <summary>
        /// Novell IPX (alt)
        /// </summary>
        NovellInternetworkPacketExchange = 0x8137,
        /// <summary>
        /// Novell
        /// </summary>
        Novell = 0x8138,
        /// <summary>
        /// Internet Protocol, Version 6 (IPv6)
        /// </summary>
        IpV6 = 0x86DD,
        /// <summary>
        /// MAC Control
        /// </summary>
        MacControl = 0x8808,
        /// <summary>
        /// CobraNet
        /// </summary>
        CobraNet = 0x8819,
        /// <summary>
        /// MPLS unicast
        /// </summary>
        MultiprotocolLabelSwitchingUnicast = 0x8847,
        /// <summary>
        /// MPLS multicast
        /// </summary>
        MultiprotocolLabelSwitchingMulticast = 0x8848,
        /// <summary>
        /// PPPoE Discovery Stage
        /// </summary>
        PointToPointProtocolOverEthernetDiscoveryStage = 0x8863,
        /// <summary>
        /// PPPoE Session Stage 
        /// </summary>
        PointToPointProtocolOverEthernetSessionStage = 0x8864,
        /// <summary>
        /// EAP over LAN (IEEE 802.1X)
        /// </summary>
        ExtensibleAuthenticationProtocolOverLan = 0x888E,
        /// <summary>
        /// HyperSCSI (SCSI over Ethernet)
        /// </summary>
        HyperScsi = 0x889A,
        /// <summary>
        /// ATA over Ethernet
        /// </summary>
        AtaOverEthernet = 0x88A2,
        /// <summary>
        /// EtherCAT Protocol
        /// </summary>
        EtherCatProtocol = 0x88A4,
        /// <summary>
        /// Provider Bridging (IEEE 802.1ad)
        /// </summary>
        ProviderBridging = 0x88A8,
        /// <summary>
        /// AVB Transport Protocol (AVBTP)
        /// </summary>
        AvbTransportProtocol = 0x88B5,
        /// <summary>
        /// SERCOS III
        /// </summary>
        SerialRealTimeCommunicationSystemIii = 0x88CD,
        /// <summary>
        /// Circuit Emulation Services over Ethernet (MEF-8)
        /// </summary>
        CircuitEmulationServicesOverEthernet = 0x88D8,
        /// <summary>
        /// HomePlug
        /// </summary>
        HomePlug = 0x88E1,
        /// <summary>
        /// MAC security (IEEE 802.1AE)
        /// </summary>
        MacSecurity = 0x88E5,
        /// <summary>
        /// Precision Time Protocol (IEEE 1588)
        /// </summary>
        PrecisionTimeProtocol = 0x88f7,
        /// <summary>
        /// IEEE 802.1ag Connectivity Fault Management (CFM) Protocol / ITU-T Recommendation Y.1731 (OAM)
        /// </summary>
        ConnectivityFaultManagementOrOperationsAdministrationManagement = 0x8902,
        /// <summary>
        /// Fibre Channel over Ethernet
        /// </summary>
        FibreChannelOverEthernet = 0x8906,
        /// <summary>
        /// FCoE Initialization Protocol
        /// </summary>
        FibreChannelOverEthernetInitializationProtocol = 0x8914,
        /// <summary>
        /// Q-in-Q
        /// </summary>
        QInQ = 0x9100,
        /// <summary>
        /// Veritas Low Latency Transport (LLT)
        /// </summary>
        VeritasLowLatencyTransport = 0xCAFE,
        /// <summary>
        /// Ethernet loopback packet
        /// </summary>
        Loop = 0x0060,
        /// <summary>
        /// Ethernet echo packet
        /// </summary>
        Echo = 0x0200
    }
}
