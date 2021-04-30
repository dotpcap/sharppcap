using System;
namespace SharpPcap
{
    /// <summary>
    /// See https://www.tcpdump.org/manpages/pcap_next_ex.3pcap.html
    /// and https://github.com/the-tcpdump-group/libpcap/blob/fbcc461fbc2bd3b98de401cc04e6a4a10614e99f/pcap/pcap.h
    /// </summary>
    public enum GetPacketStatus
    {
        ReadTimeout = 0,

        PacketRead = 1,

        /// <summary>
        /// PCAP_ERROR
        /// </summary>
        Error = -1,

        /// <summary>
        /// PCAP_ERROR_BREAK
        /// </summary>
        NoRemainingPackets = -2,
    };
}
