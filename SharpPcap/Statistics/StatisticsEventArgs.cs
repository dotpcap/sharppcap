// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2009-2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using SharpPcap.LibPcap;
using System;

namespace SharpPcap.Statistics
{
    /// <summary>
    /// Event that contains statistics mode data
    /// NOTE: Npcap only
    /// </summary>
    public class StatisticsEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for a statistics mode event
        /// </summary>
        internal StatisticsEventArgs(StatisticsDevice device, PosixTimeval timeval, long packets, long bytes)
        {
            Device = device;
            Timeval = timeval;
            ReceivedPackets = packets;
            ReceivedBytes = bytes;
        }

        /// <summary>
        /// Device this EventArgs was generated for
        /// </summary>
        public StatisticsDevice Device { get; }

        /// <summary>
        /// This holds time value
        /// </summary>
        public PosixTimeval Timeval { get; }

        /// <summary>
        /// Number of packets received since last sample
        /// </summary>
        public long ReceivedPackets { get; }


        /// <summary>
        /// Number of bytes received since last sample
        /// </summary>
        public long ReceivedBytes { get; }

    }
}
