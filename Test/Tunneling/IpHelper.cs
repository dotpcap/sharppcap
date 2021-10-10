using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Test.WinTap
{
    class IpHelper
    {
        internal static void SetIPv4Address(NetworkInterface networkInterface, IPAddress ip)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("netsh", $"interface ip set address name=\"{networkInterface.Name}\" static {ip} 255.255.255.0").WaitForExit();
            }
            else
            {
                Process.Start("ip", $"address set {ip}/24 dev {networkInterface.Name}").WaitForExit();
            }
        }
    }

}
