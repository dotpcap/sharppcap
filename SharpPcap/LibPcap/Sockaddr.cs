/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/* 
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

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
        public System.Net.IPAddress ipAddress;

        /// <summary>
        /// If type == HARDWARE
        /// </summary>
        public PhysicalAddress hardwareAddress;

        private int _sa_family;

        /// <summary>
        /// Address family
        /// </summary>
        public int sa_family
        {
            get { return _sa_family; }
        }

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
            // A sockaddr struct. We use this to determine the address family
            PcapUnmanagedStructures.sockaddr saddr;

            // Marshal memory pointer into a struct
            saddr = (PcapUnmanagedStructures.sockaddr)Marshal.PtrToStructure(sockaddrPtr,
                                                     typeof(PcapUnmanagedStructures.sockaddr));

            // record the sa_family for informational purposes
            _sa_family = saddr.sa_family;

            byte[] addressBytes;
            if(saddr.sa_family == Pcap.AF_INET)
            {
                type = AddressTypes.AF_INET_AF_INET6;
                PcapUnmanagedStructures.sockaddr_in saddr_in = 
                    (PcapUnmanagedStructures.sockaddr_in)Marshal.PtrToStructure(sockaddrPtr,
                                                                                typeof(PcapUnmanagedStructures.sockaddr_in));
                ipAddress = new System.Net.IPAddress(saddr_in.sin_addr.s_addr);
            } else if(saddr.sa_family == Pcap.AF_INET6)
            {
                type = AddressTypes.AF_INET_AF_INET6;
                addressBytes = new byte[16];
                PcapUnmanagedStructures.sockaddr_in6 sin6 =
                    (PcapUnmanagedStructures.sockaddr_in6)Marshal.PtrToStructure(sockaddrPtr,
                                                         typeof(PcapUnmanagedStructures.sockaddr_in6));
                Array.Copy(sin6.sin6_addr, addressBytes, addressBytes.Length);
                ipAddress = new System.Net.IPAddress(addressBytes);
            } else if(saddr.sa_family == Pcap.AF_PACKET)
            {
                type = AddressTypes.HARDWARE;

                PcapUnmanagedStructures.sockaddr_ll saddr_ll =
                    (PcapUnmanagedStructures.sockaddr_ll)Marshal.PtrToStructure(sockaddrPtr,
                                                      typeof(PcapUnmanagedStructures.sockaddr_ll));

                byte[] hardwareAddressBytes = new byte[saddr_ll.sll_halen];
                for(int x = 0; x < saddr_ll.sll_halen; x++)
                {
                    hardwareAddressBytes[x] = saddr_ll.sll_addr[x];
                }
                hardwareAddress = new PhysicalAddress(hardwareAddressBytes); // copy into the PhysicalAddress class
            } else
            {
                type = AddressTypes.UNKNOWN;

                // place the sockaddr.sa_data into the hardware address just in case
                // someone wants access to the bytes
                byte[] hardwareAddressBytes = new byte[saddr.sa_data.Length];
                for(int x = 0; x < saddr.sa_data.Length; x++)
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
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            if(type == AddressTypes.AF_INET_AF_INET6)
            {
                return ipAddress.ToString();
            } else if(type == AddressTypes.HARDWARE)
            {
                return "HW addr: " + hardwareAddress.ToString();
            } else if(type == AddressTypes.UNKNOWN)
            {
                return String.Empty;
            }

            return String.Empty;
        }
    }
}
