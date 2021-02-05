/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/* 
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2009-2010 Chris Morgan <chmorgan@gmail.com>
 */

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
