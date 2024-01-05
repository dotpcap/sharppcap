// SPDX-FileCopyrightText: 2020 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap.WinDivert
{
    public enum WinDivertPacketFlags : byte
    {
        Sniffed = 1 << 0,
        Outbound = 1 << 1,
        Loopback = 1 << 2,
        Impostor = 1 << 3,
        IPv6 = 1 << 4,
        IPChecksum = 1 << 5,
        TCPChecksum = 1 << 6,
        UDPChecksum = 1 << 7,
    }
}
