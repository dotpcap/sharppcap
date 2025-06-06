// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System.Net;

namespace SharpPcap.Tunneling
{
    /// <summary>
    /// IP Address configuration of the tunnel interface
    /// </summary>
    public class IPAddressConfiguration
    {
        // property names based on UnicastIPAddressInformation
        public IPAddress? Address { get; set; }
        public IPAddress? IPv4Mask { get; set; }
    }
}
