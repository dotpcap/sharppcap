using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Test.WinTap
{
    class IpHelper
    {

        /// <summary>
        /// Ger IPv4 address from interface, or create one if none exists
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        internal static IPAddress EnsureIPv4Address(NetworkInterface networkInterface)
        {
            var addr = GetIPv4Address(networkInterface);
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Pick a range that no CI is likely to use
                Process.Start("ip", "address add 10.255.255.1/24 dev tap0").WaitForExit();
                addr = GetIPv4Address(networkInterface);
            }
            if (addr == null)
            {
                // Failed to set an IP
                // Windows auto assign an IP, maybe macos?
                throw new NotSupportedException();
            }
            return addr;
        }

        private static IPAddress GetIPv4Address(NetworkInterface networkInterface)
        {
            // Update interface reference, since addresses could change after interface opened
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                .First(n => n.Id.Equals(networkInterface.Id));
            foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses)
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
