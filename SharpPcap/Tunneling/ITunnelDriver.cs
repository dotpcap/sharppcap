// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Net.NetworkInformation;

namespace SharpPcap.Tunneling
{
    internal interface ITunnelDriver
    {
        bool IsTunnelInterface(NetworkInterface networkInterface);
        FileStream Open(NetworkInterface networkInterface, IPAddressConfiguration address, DeviceConfiguration configuration);
        Version GetVersion(NetworkInterface networkInterface, SafeFileHandle handle);
    }

}
