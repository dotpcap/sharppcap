// Copyright 2009-2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

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
        internal PcapStatistics(PcapHandle pcap_t)
        {
            IntPtr stat;

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                // allocate memory for the struct
                stat = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PcapUnmanagedStructures.pcap_stat_unix)));
            }
            else
            {
                // allocate memory for the struct
                stat = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PcapUnmanagedStructures.pcap_stat_windows)));
            }

            // retrieve the stats
            var result = (PcapUnmanagedStructures.PcapStatReturnValue)LibPcapSafeNativeMethods.pcap_stats(pcap_t, stat);

            // process the return value
            switch (result)
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
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                var managedStat = Marshal.PtrToStructure<PcapUnmanagedStructures.pcap_stat_unix>(stat);

                // copy the values
                this.ReceivedPackets = (uint)managedStat.ps_recv.ToInt64();
                this.DroppedPackets = (uint)managedStat.ps_drop.ToInt64();
                //                this.InterfaceDroppedPackets = (uint)managedStat.ps_ifdrop;
            }
            else
            {
                var managedStat = Marshal.PtrToStructure<PcapUnmanagedStructures.pcap_stat_windows>(stat);

                // copy the values
                this.ReceivedPackets = (uint)managedStat.ps_recv;
                this.DroppedPackets = (uint)managedStat.ps_drop;
                //                this.InterfaceDroppedPackets = (uint)managedStat.ps_ifdrop;
            }

            // NOTE: Not supported on unix or npcap, no need to
            //       put a bogus value in this field
            this.InterfaceDroppedPackets = 0;

            // free the stats
            Marshal.FreeHGlobal(stat);
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("[PcapStatistics: ReceivedPackets={0}, DroppedPackets={1}, InterfaceDroppedPackets={2}]",
                                 ReceivedPackets, DroppedPackets, InterfaceDroppedPackets);
        }
    }
}
