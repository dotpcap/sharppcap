// Copyright 2022 Hendrik Eckardt
// SPDX-License-Identifier: MIT

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Base class for 'offline' devices.
    /// </summary>
    public abstract class CaptureReaderDevice : PcapDevice
    {
        /// <summary>
        /// Retrieves pcap statistics.
        ///
        /// Not supported for this device.
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public override ICaptureStatistics Statistics => null;
    }
}
