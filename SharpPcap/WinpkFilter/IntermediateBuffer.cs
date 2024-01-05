// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;

namespace SharpPcap.WinpkFilter
{

    /// <summary>
    /// Contains packet both metadata and data
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct IntermediateBuffer
    {
        internal IntermediateBufferHeader Header;
        internal fixed byte Frame[NativeMethods.MAX_ETHER_FRAME];
    }

    /// <summary>
    /// Contains packet NDIS flags and WinPkFilter specific flags.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IntermediateBufferHeader
    {
        #region LIST_ENTRY members
        internal IntPtr Flink;
        internal IntPtr Blink;
        #endregion

        internal PacketSource Source;
        internal uint Length;
        internal uint Flags;
        internal uint Dot1q;
        internal uint FilterId;

        internal uint Reserved0;
        internal uint Reserved1;
        internal uint Reserved2;
        internal uint Reserved3;
    }

}