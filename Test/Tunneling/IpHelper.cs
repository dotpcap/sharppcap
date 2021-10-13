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

        /// <summary>
        /// Linux machines tend to create a tap that have no IPs
        /// Windows usually auto assign an IP
        /// </summary>
        /// <param name="nic"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        internal static IPAddress EnsureIPv4Address(NetworkInterface nic)
        {
            var ip = GetIPAddress(nic);
            if (ip != null)
            {
                return ip;
            }
            // Pick a range that no CI is likely to use
            ip = IPAddress.Parse("10.225.255.100");
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
            if (ip.Equals(GetIPAddress(nic)))
            {
                return ip;
            }
            throw new NotSupportedException($"Failed to set interface '{name}' address.");
        }

        internal static IPAddress GetIPAddress(NetworkInterface nic)
        {
            // Update interface reference, since addresses could change after interface opened
            nic = NetworkInterface.GetAllNetworkInterfaces()
                .First(n => n.Id.Equals(nic.Id));
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
