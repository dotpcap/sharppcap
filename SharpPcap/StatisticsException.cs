// Copyright 2009-2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// thrown when pcap_stats() reports an error
    /// </summary>
    public class StatisticsException : PcapException
    {
        /// <summary>
        /// string constructor
        /// </summary>
        /// <param name="msg">
        /// A <see cref="string"/>
        /// </param>
        public StatisticsException(string msg) : base(msg)
        {
        }
    }
}
