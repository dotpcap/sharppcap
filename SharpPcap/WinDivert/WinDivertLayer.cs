// SPDX-FileCopyrightText: 2020 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap.WinDivert
{
    /// <summary>
    /// Represents which part of the networking layer a WinDivert handle is operating on.
    /// </summary>
    public enum WinDivertLayer : uint
    {
        /// <summary>
        /// Represents the networking layer sans forwarded packets.
        /// </summary>
        Network = 0,

        /// <summary>
        /// Represents forwarded packets exclusively on the network layer.
        /// </summary>
        Forward = 1,
        
        Flow = 2,
        Socket = 3,
        Reflect = 4
    }
}