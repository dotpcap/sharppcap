using System;

namespace SharpPcap
{
    /// <summary>
    /// thrown when pcap_stats() reports an error
    /// </summary>
    public class PcapStatisticsException : PcapException
    {
        public PcapStatisticsException(string msg) : base(msg)
        {
        }
    }
}
