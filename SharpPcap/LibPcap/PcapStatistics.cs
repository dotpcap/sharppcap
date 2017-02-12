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
 * Copyright 2009-2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Adapter statistics, received, dropped packet counts etc
    /// </summary>
    public class PcapStatistics : ICaptureStatistics
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

        internal PcapStatistics() { }

        /// <summary>
        /// Retrieve pcap statistics from the adapter
        /// </summary>
        /// <param name="pcap_t">
        /// pcap_t* for the adapter
        /// A <see cref="IntPtr"/>
        /// </param>
        internal PcapStatistics(IntPtr pcap_t)
        {
            IntPtr stat;

            if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                // allocate memory for the struct
                stat = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PcapUnmanagedStructures.pcap_stat_unix)));
            } else
            {
                // allocate memory for the struct
                stat = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PcapUnmanagedStructures.pcap_stat_windows)));
            }

            // retrieve the stats
            var result = (PcapUnmanagedStructures.PcapStatReturnValue)LibPcapSafeNativeMethods.pcap_stats(pcap_t, stat);

            // process the return value
            switch(result)
            {
            case PcapUnmanagedStructures.PcapStatReturnValue.Error:
                // retrieve the error information
                var error = LibPcapLiveDevice.GetLastError(pcap_t);

                // free the stats memory so we don't leak before we throw
                Marshal.FreeHGlobal(stat);

                throw new StatisticsException(error);
            case PcapUnmanagedStructures.PcapStatReturnValue.Success:
                // nothing to do upon success
                break;
            }
  
            // marshal the unmanaged memory into an object of the proper type
            if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                var managedStat = (PcapUnmanagedStructures.pcap_stat_unix)Marshal.PtrToStructure(stat,
                                                                                                 typeof(PcapUnmanagedStructures.pcap_stat_unix));

                // copy the values
                this.ReceivedPackets = (uint)managedStat.ps_recv;
                this.DroppedPackets = (uint)managedStat.ps_drop;
//                this.InterfaceDroppedPackets = (uint)managedStat.ps_ifdrop;
            } else
            {
                var managedStat = (PcapUnmanagedStructures.pcap_stat_windows)Marshal.PtrToStructure(stat,
                                                                                                    typeof(PcapUnmanagedStructures.pcap_stat_windows));

                // copy the values
                this.ReceivedPackets = (uint)managedStat.ps_recv;
                this.DroppedPackets = (uint)managedStat.ps_drop;
//                this.InterfaceDroppedPackets = (uint)managedStat.ps_ifdrop;
            }

            // NOTE: Not supported on unix or winpcap, no need to
            //       put a bogus value in this field
            this.InterfaceDroppedPackets = 0;

            // free the stats
            Marshal.FreeHGlobal(stat);
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[PcapStatistics: ReceivedPackets={0}, DroppedPackets={1}, InterfaceDroppedPackets={2}]",
                                 ReceivedPackets, DroppedPackets, InterfaceDroppedPackets);
        }
    }
}
