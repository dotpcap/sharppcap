using System;

namespace SharpPcap
{
    /// <summary>
    /// The mode used when opening a device
    /// </summary>
    public enum DeviceMode : short
    {
        /// <summary>
        /// Promiscuous mode.
        /// Instructs the OS that we want to receive all packets, even those not
        /// intended for the adapter. On non-switched networks this can result in
        /// a large amount of addtional traffic.
        /// NOTE: Devices in this mode CAN be detected via the network
        /// </summary>
        Promiscuous = 1,

        /// <summary>
        /// Not promiscuous mode
        /// </summary>
        Normal = 0
    }
}
