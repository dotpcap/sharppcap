// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;

namespace SharpPcap.WinpkFilter
{
    /// <summary>
    /// Used for passing the read packet request to the driver.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct EthRequest
    {
        internal IntPtr AdapterHandle;
        internal IntPtr Buffer;
    }
}