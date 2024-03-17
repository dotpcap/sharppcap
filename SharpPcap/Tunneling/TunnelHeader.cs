// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT


namespace SharpPcap.Tunneling
{
    public class TunnelHeader : ICaptureHeader
    {
        public PosixTimeval Timeval { get; set; }

        public TunnelHeader()
        {
            Timeval = new PosixTimeval();
        }
    }
}
