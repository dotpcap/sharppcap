// Copyright 2011 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// Adapter statistics, received, dropped packet counts etc
    /// </summary>
    public interface ICaptureStatistics
    {
        /// <value>
        /// Number of packets received
        /// </value>
        uint ReceivedPackets { get; set; }

        /// <value>
        /// Number of packets dropped
        /// </value>
        uint DroppedPackets { get; set; }

        /// <value>
        /// Number of interface dropped packets
        /// </value>
        uint InterfaceDroppedPackets { get; set; }
    }
}
