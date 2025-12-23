// Copyright 2009-2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

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
        internal unsafe PcapStatistics(PcapHandle pcap_t)
        {
            PcapStatUnix statUnix = new();
            PcapStatWindows statWindows = new();
            int result;

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                result = LibPcapSafeNativeMethods.pcap_stats(pcap_t, ref statUnix);
            }
            else
            {
                result = LibPcapSafeNativeMethods.pcap_stats(pcap_t, ref statWindows);
            }

            // retrieve the stats

            // process the return value
            switch ((PcapStatReturnValue)result)
            {
                case PcapStatReturnValue.Error:
                    // retrieve the error information
                    var error = LibPcapLiveDevice.GetLastError(pcap_t);
                    throw new StatisticsException(error);
                case PcapStatReturnValue.Success:
                    // nothing to do upon success
                    break;
            }

            // marshal the unmanaged memory into an object of the proper type
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                // copy the values
                this.ReceivedPackets = (uint)statUnix.ps_recv;
                this.DroppedPackets = (uint)statUnix.ps_drop;
            }
            else
            {
                // copy the values
                this.ReceivedPackets = statWindows.ps_recv;
                this.DroppedPackets = statWindows.ps_drop;
            }

            // NOTE: Not supported on unix or npcap, no need to
            //       put a bogus value in this field
            this.InterfaceDroppedPackets = 0;
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
