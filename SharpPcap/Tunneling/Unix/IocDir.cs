// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;

namespace SharpPcap.Tunneling.Unix
{
    [Flags]
    internal enum IocDir : uint
    {
        None = 1U,
        Read = 2U,
        Write = 4U,
    }
}
