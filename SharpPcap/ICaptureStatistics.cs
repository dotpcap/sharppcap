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
 * Copyright 2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;

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
