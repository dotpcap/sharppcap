// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Container class that represents either an ip address or a mac address
    /// An analog to the 'sockaddr_' series of structures
    /// </summary>
    public class Sockaddr
    {
        /// <summary>
        /// Types of addresses a Sockaddr can represent
        /// </summary>
        public enum AddressTypes
        {
            /// <summary>
            /// Address represents an ipv4 or ipv6 address
            /// </summary>
            AF_INET_AF_INET6,

            /// <summary>
            /// Address represents a physical hardware address eg. a ethernet mac address
            /// </summary>
            HARDWARE,

            /// <summary>
            /// Unknown address type
            /// </summary>
            UNKNOWN
        }

        /// <summary>
        /// Address type represented by this Sockaddr
        /// </summary>
        public AddressTypes type;

        /// <summary>
        /// If type == AF_INET_AF_INET6
        /// </summary>
        public IPAddress ipAddress;

        /// <summary>
        /// If type == HARDWARE
        /// </summary>
        public PhysicalAddress hardwareAddress;

        /// <summary>
        /// Address family
        /// </summary>
        public int sa_family { get; }

        /// <summary>
        /// Create a Sockaddr from a PhysicalAddress which is presumed to
        /// be a hardware address
        /// </summary>
        /// <param name="hardwareAddress">
        /// A <see cref="PhysicalAddress"/>
        /// </param>
        public Sockaddr(PhysicalAddress hardwareAddress)
        {
            this.type = AddressTypes.HARDWARE;
            this.hardwareAddress = hardwareAddress;
        }

        internal Sockaddr(IntPtr sockaddrPtr)
        {
            // Marshal memory pointer into a struct
            var saddr = Marshal.PtrToStructure<sockaddr>(sockaddrPtr);

            // record the sa_family for informational purposes
            sa_family = saddr.sa_family;

            if (saddr.sa_family == Pcap.AF_INET)
            {
                type = AddressTypes.AF_INET_AF_INET6;
                var saddr_in = Marshal.PtrToStructure<sockaddr_in>(sockaddrPtr);
                ipAddress = new IPAddress(saddr_in.sin_addr.s_addr);
            }
            else if (saddr.sa_family == Pcap.AF_INET6)
            {
                type = AddressTypes.AF_INET_AF_INET6;
                var sin6 = Marshal.PtrToStructure<sockaddr_in6>(sockaddrPtr);
                ipAddress = new IPAddress(sin6.sin6_addr, sin6.sin6_scope_id);
            }
            else if (saddr.sa_family == Pcap.AF_PACKET)
            {
                type = AddressTypes.HARDWARE;

                var saddr_ll = Marshal.PtrToStructure<sockaddr_ll>(sockaddrPtr);

                var hwAddrBytes = new byte[saddr_ll.sll_halen];
                Buffer.BlockCopy(saddr_ll.sll_addr, 0, hwAddrBytes, 0, hwAddrBytes.Length);
                hardwareAddress = new PhysicalAddress(hwAddrBytes); // copy into the PhysicalAddress class
            }
            else
            {
                type = AddressTypes.UNKNOWN;

                // place the sockaddr.sa_data into the hardware address just in case
                // someone wants access to the bytes
                byte[] hardwareAddressBytes = new byte[saddr.sa_data.Length];
                for (int x = 0; x < saddr.sa_data.Length; x++)
                {
                    hardwareAddressBytes[x] = saddr.sa_data[x];
                }
                hardwareAddress = new PhysicalAddress(hardwareAddressBytes);
            }
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        public override string ToString()
        {
            if (type == AddressTypes.AF_INET_AF_INET6)
            {
                return ipAddress.ToString();
            }
            else if (type == AddressTypes.HARDWARE)
            {
                return "HW addr: " + hardwareAddress.ToString();
            }
            else if (type == AddressTypes.UNKNOWN)
            {
                return String.Empty;
            }

            return String.Empty;
        }
    }
}
