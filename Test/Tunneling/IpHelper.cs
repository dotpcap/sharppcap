using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace Test.Tunneling
{
    class IpHelper
    {

        /// <summary>
        /// Wait for interface to have an effective IP, and return it
        /// </summary>
        /// <param name="nic"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        internal static IPAddress GetIPAddress(NetworkInterface nic, int retry = 10)
        {
            IPAddress pendingIp = null;
            // Update interface reference, since addresses could change after interface opened
            nic = NetworkInterface.GetAllNetworkInterfaces()
                .First(n => n.Id.Equals(nic.Id));
            foreach (var addr in nic.GetIPProperties().UnicastAddresses)
            {
                if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                        addr.DuplicateAddressDetectionState != DuplicateAddressDetectionState.Preferred)
                    {  
                        // We have an IP, but it's not ready for use yet
                        pendingIp = addr.Address;
                    }
                    else
                    {
                        return addr.Address;
                    }
                }
            }
            if (retry > 0)
            {
                while (nic.OperationalStatus != OperationalStatus.Up)
                {
                    Console.WriteLine($"interface '{nic.Name}' is {nic.OperationalStatus}");
                }
                if (pendingIp != null)
                {
                    Console.WriteLine($"interface '{nic.Name}' have pending IP {pendingIp}");
                    Thread.Sleep(1000);
                }
                return GetIPAddress(nic, retry - 1);
            }
            throw new NotSupportedException($"Failed to get interface '{nic.Name}' address.");
        }
    }

}
