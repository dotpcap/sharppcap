using System;
namespace SharpPcap
{
    /// <summary>
    /// Source of the timestamp
    /// See https://github.com/the-tcpdump-group/libpcap/blob/master/pcap/pcap.h#L498 for details
    /// </summary>
    public enum TimestampType : int
    {
        Host,
        HostLowPrecision,
        HostHighPrecision,
        Adapter,
        AdapterUnsynced,
        HostHighPrecisionUnsynced
    }
}
