// Copyright 2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// The types of transmit modes allowed by the Npcap specific send queue
    /// implementation
    /// </summary>
    public enum SendQueueTransmitModes
    {
        /// <summary>
        /// Packets are sent as fast as possible
        /// </summary>
        Normal,

        /// <summary>
        /// Packets are synchronized in the kernel with a high precision timestamp
        /// </summary>
        Synchronized
    }
}
