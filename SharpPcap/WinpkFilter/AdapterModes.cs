// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;

namespace SharpPcap.WinpkFilter
{
    /// <summary>
    /// Used to set the adapter flags.
    /// </summary>
    [Flags]
    public enum AdapterModes : uint
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Receive packets sent by MSTCP to network interface.
        /// The original packet is dropped.
        /// </summary>
        SentTunnel = 0x00000001,

        /// <summary>
        /// Receive packets sent from network interface to MSTCP.
        /// The original packet is dropped.
        /// </summary>
        RecvTunnel = 0x00000002,

        /// <summary>
        /// Receive packets sent from and to MSTCP and network interface.
        /// The original packet is dropped.
        /// </summary>
        Tunnel = SentTunnel | RecvTunnel,

        /// <summary>
        /// Receive packets sent by MSTCP to network interface.
        /// The original packet is still delivered to the network.
        /// </summary>
        SentListen = 0x00000004,

        /// <summary>
        /// Receive packets sent from network interface to MSTCP
        /// The original packet is still delivered to the network.
        /// </summary>
        RecvListen = 0x00000008,

        /// <summary>
        /// Receive packets sent from and to MSTCP and network interface.
        /// The original packet is dropped.
        /// </summary>
        Listen = SentListen | RecvListen,

        /// <summary>
        /// In promiscuous mode TCP/IP stack receives all.
        /// </summary>
        FilterDirect = 0x00000010,

        /// <summary>
        /// Passes loopback packet for processing.
        /// </summary>
        LoopbackFilter = 0x00000020,

        /// <summary>
        /// Silently drop loopback packets.
        /// </summary>
        LoopbackBlock = 0x00000040
    }

}