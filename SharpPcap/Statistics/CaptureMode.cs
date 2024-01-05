// Copyright 2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap.Statistics
{
    /// <summary>
    /// The working mode of a Pcap device
    /// </summary>
    internal enum CaptureMode : int
    {
        /// <summary>
        /// Set a Pcap device to capture packets, Capture mode
        /// </summary>
        Packets = 0,

        /// <summary>
        /// Set a Pcap device to report statistics.
        /// <br/>
        /// Statistics mode is only supported in Npcap
        /// </summary>
        Statistics = 1
    };
}
