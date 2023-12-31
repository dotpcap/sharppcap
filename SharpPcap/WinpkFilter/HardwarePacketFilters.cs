// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;

namespace SharpPcap.WinpkFilter
{

    /// <summary>
    /// The NDIS_PACKET_TYPE enum.
    /// See https://docs.microsoft.com/en-us/windows-hardware/drivers/network/oid-gen-current-packet-filter
    /// See https://www.ntkernel.com/docs/windows-packet-filter-documentation/ndisapi-c-2/sethwpacketfilter/
    /// </summary>
    [Flags]
    public enum HardwarePacketFilters : uint
    {
        /// <summary>
        /// Used to reset the default filter
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// Only packets directed to the workstation’s adapter are accepted
        /// </summary>
        Directed = 0x0001,
        /// <summary>
        /// Only the multicast packets belonging to the groups of which this adapter is a member are accepted.
        /// </summary>
        Multicast = 0x0002,
        /// <summary>
        /// Every multicast packet is accepted.
        /// </summary>
        AllMulticast = 0x0004,
        /// <summary>
        /// Only the broadcast packets are accepted.
        /// </summary>
        Broadcast = 0x0008,
        /// <summary>
        /// The promiscuous mode. Every incoming packet is accepted by the adapter.
        /// </summary>
        Promiscuous = 0x0020,
        /// <summary>
        /// All local packets, i.e. Directed + Broadcast + Multicast
        /// </summary>
        AllLocal = 0x0080,
    }

}