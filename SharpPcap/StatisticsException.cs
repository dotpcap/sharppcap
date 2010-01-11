using System;

namespace SharpPcap
{
    /// <summary>
    /// thrown when pcap_stats() reports an error
    /// </summary>
    public class StatisticsException : PcapException
    {
        public StatisticsException(string msg) : base(msg)
        {
        }
    }
}
