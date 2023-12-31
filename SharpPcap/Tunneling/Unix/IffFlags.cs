// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;

namespace SharpPcap.Tunneling.Unix
{
    [Flags]
    internal enum IffFlags : short
    {
        Tun = 0x0001,
        Tap = 0x0002,
        NoPi = 0x1000,
    }
}
