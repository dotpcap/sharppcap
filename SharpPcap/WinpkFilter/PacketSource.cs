// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT


namespace SharpPcap.WinpkFilter
{
    /// <summary>
    /// The packet flag.
    /// </summary>
    public enum PacketSource : uint
    {
        /// <summary>
        /// The packet was intercepted from MSTCP.
        /// </summary>
        System = 0x00000001,

        /// <summary>
        /// The packet was intercepted from the network interface.
        /// </summary>
        Interface = 0x00000002
    }

}