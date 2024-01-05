// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT


namespace SharpPcap.WinpkFilter
{
    public class WinpkFilterHeader : ICaptureHeader
    {
        public PosixTimeval Timeval { get; set; }
        public PacketSource Source { get; set; }
        public uint Dot1q { get; set; }

        public WinpkFilterHeader()
        {
            Timeval = new PosixTimeval();
        }
    }
}
