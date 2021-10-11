using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Test.Tunneling
{
    class IpHelper
    {
        internal static void SetIPv4Address(NetworkInterface nic, IPAddress ip)
        {
            var name = nic.Name;
            Process p;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                p = Process.Start("netsh", $"interface ip set address name=\"{name}\" static {ip} 255.255.255.0");
            }
            else
            {
                p = Process.Start("ip", $"address replace {ip}/24 dev {name}");
            }
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                throw new NotSupportedException($"Failed to set interface '{name}' address. Exit code {p.ExitCode}");
            }
            // Update interface reference, since addresses could change after interface opened
            // Also wait for interface to be up again
            var sw = Stopwatch.StartNew();
            while (nic.OperationalStatus != OperationalStatus.Up && sw.ElapsedMilliseconds < 10000)
            {
                nic = NetworkInterface.GetAllNetworkInterfaces()
                    .First(n => n.Id.Equals(nic.Id));
            }
            foreach (UnicastIPAddressInformation info in nic.GetIPProperties().UnicastAddresses)
            {
                if (ip.Equals(info.Address))
                {
                    return;
                }
            }
            throw new NotSupportedException($"Failed to set interface '{name}' address.");
        }

        internal static IPAddress GetIPAddress(NetworkInterface nic)
        {
            foreach (var ip in nic.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.Address;
                }
            }
            return null;
        }
    }

}
