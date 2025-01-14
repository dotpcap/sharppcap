// Copyright 2017 Noah Potash <noah.potash@outbreaklabs.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// The activation of monitor mode used when opening a device
    /// </summary>
    public enum MonitorMode : short
    {
        /// <summary>
        /// Monitor mode.
        /// Allows capturing of 802.11 wireless packets even when not associated
        /// with a network.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Not monitor mode
        /// </summary>
        Inactive = 0
    }
}
