// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;

namespace SharpPcap.Tunneling.WinTap
{
    /// <summary>
    /// See https://docs.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew
    /// </summary>
    [Flags]
    enum WinFileAccess : uint
    {
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000,
    }
}
