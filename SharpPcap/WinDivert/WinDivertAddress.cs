// SPDX-FileCopyrightText: 2020 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;

namespace SharpPcap.WinDivert
{
    /// <summary>
    /// The WinDivertAddress structure represents the "address" of a captured or injected packet. The
    /// address includes the packet's timestamp, network interfaces, direction and other information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct WinDivertAddress
    {
        public long Timestamp;
        public byte Layer;
        public byte Event;
        public WinDivertPacketFlags Flags;
        public uint IfIdx;
        public uint SubIfIdx;
        // Added bytes to match the struct size of WINDIVERT_ADDRESS, since we only map the first few fields
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)]
        internal byte[] Padding;
    }
}