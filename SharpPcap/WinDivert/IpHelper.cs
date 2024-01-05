// SPDX-FileCopyrightText: 2020 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace SharpPcap.WinDivert
{
    /// <summary>
    /// Helper methods to facilitate finding interface index from IP addresses
    /// </summary>
    public static class IpHelper
    {
        private const string IPHLPAPI = "iphlpapi.dll";

        [DllImport(IPHLPAPI)]
        private extern static uint GetBestInterfaceEx(byte[] ipAddress, out int index);

        /// <summary>
        /// This routine converts an IPAddress into a byte array that represents the
        /// underlying sockaddr structure.
        /// </summary>
        /// <param name="address">IPAddress to convert to a binary form</param>
        /// <returns>Binary array of the serialized socket address structure</returns>
        private static byte[] GetSocketAddressBytes(IPAddress ip)
        {
            var address = new IPEndPoint(ip, 0).Serialize();
            byte[] bytes;
            bytes = new byte[address.Size];
            for (int i = 0; i < address.Size; i++)
            {
                bytes[i] = address[i];
            }
            return bytes;
        }

        /// <summary>
        /// wrapper around iphlpapi GetBestInterfaceEx,
        /// Allows finding the appropriate interface index from the destination ip address
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <returns></returns>
        public static int GetBestInterfaceIndex(IPAddress destinationAddress)
        {
            const uint NO_ERROR = 0;
            var addr = GetSocketAddressBytes(destinationAddress);
            int error = (int)GetBestInterfaceEx(addr, out int index);
            if (error != NO_ERROR)
            {
                throw new NetworkInformationException(error);
            }
            return index;
        }

        /// <summary>
        /// Helper method to find the NetworkInterface that should be used for a given destination address
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <returns></returns>
        public static NetworkInterface GetBestInterface(IPAddress destinationAddress)
        {
            var interfaceIndex = GetBestInterfaceIndex(destinationAddress);

            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ipProperties = networkInterface.GetIPProperties();

                if (networkInterface.Supports(NetworkInterfaceComponent.IPv4))
                {
                    var iPv4Properties = ipProperties.GetIPv4Properties();
                    if (iPv4Properties?.Index == interfaceIndex)
                    {
                        return networkInterface;
                    }
                }

                if (networkInterface.Supports(NetworkInterfaceComponent.IPv6))
                {
                    var iPv6Properties = ipProperties.GetIPv6Properties();
                    if (iPv6Properties?.Index == interfaceIndex)
                    {
                        return networkInterface;
                    }
                }
            }

            return null;
        }

        [DllImport(IPHLPAPI)]
        private static extern int GetBestRoute2(
            IntPtr interfaceLuid,
            int interfaceIndex,
            byte[] sourceAddress,
            byte[] destinationAddress,
            uint addressSortOptions,
            [In, Out] byte[] bestRoute,
            [In, Out] byte[] bestSourceAddress
        );

        /// <summary>
        /// We try to see if the addresses on a  given interface represent an inbound or outbound packet
        /// Simply we try to find a route from src to dst, if one is found, then it's outbound
        /// </summary>
        /// <param name="interfaceIndex"></param>
        /// <param name="srcAddr"></param>
        /// <param name="dstAddr"></param>
        /// <returns></returns>
        internal static bool IsOutbound(int interfaceIndex, IPAddress srcAddr, IPAddress dstAddr)
        {
            var src = GetSocketAddressBytes(srcAddr);
            var dst = GetSocketAddressBytes(dstAddr);
            const int bestSize = 28; // sizeof(SOCKADDR_INET)
            var best = new byte[bestSize];
            const int routeSize = 103; // sizeof(MIB_IPFORWARD_ROW2)
            var route = new byte[routeSize];
            var ret = GetBestRoute2(default, interfaceIndex, src, dst, 0, route, best);
            return ret == 0;
        }
    }
}
