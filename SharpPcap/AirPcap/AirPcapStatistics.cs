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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Runtime.InteropServices;
using SharpPcap;

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// Device statistics
    /// </summary>
    public class AirPcapStatistics : ICaptureStatistics
    {
        /// <value>
        /// Number of packets received
        /// </value>
        public uint ReceivedPackets { get; set; }

        /// <value>
        /// Number of packets dropped
        /// </value>
        public uint DroppedPackets { get; set; }

        /// <value>
        /// Number of interface dropped packets
        /// </value>
        public uint InterfaceDroppedPackets { get; set; }

        /// <summary>
        /// Number of packets that pass the BPF filter, find place in the kernel buffer and
        /// therefore reach the application.
        /// </summary>
        public uint CapturedPackets { get; set; }

        internal AirPcapStatistics(IntPtr AirPcapDeviceHandle)
        {
            // allocate memory for the struct
            IntPtr stat = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(AirPcapUnmanagedStructures.AirpcapStats)));

            AirPcapSafeNativeMethods.AirpcapGetStats(AirPcapDeviceHandle, stat);

            var managedStat = (AirPcapUnmanagedStructures.AirpcapStats)Marshal.PtrToStructure(stat,
                                                                                              typeof(AirPcapUnmanagedStructures.AirpcapStats));

            this.ReceivedPackets = managedStat.Recvs;
            this.DroppedPackets = managedStat.Drops;
            this.InterfaceDroppedPackets = managedStat.IfDrops;
            this.CapturedPackets = managedStat.Capt;

            // free the stats memory so we don't leak
            Marshal.FreeHGlobal(stat);
        }

        /// <summary>
        /// ToString override 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[AirPcapStatistics {0}, CapturedPackets: {1}]",
                                 base.ToString(),
                                 CapturedPackets);
        }
    }
}
