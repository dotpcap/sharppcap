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
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SharpPcap;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// managed version of struct pcap_if
    /// NOTE: we can't use pcap_if directly because the class contains
    ///       a pointer to pcap_if that will be freed when the
    ///       device memory is freed, so instead convert the unmanaged structure
    ///       to a managed one to avoid this issue
    /// </summary>
    public class PcapInterface
    {
        /// <value>
        /// Name of the interface. Used internally when passed to pcap_open_live()
        /// </value>
        public string Name { get; internal set; }

        /// <value>
        /// Human readable interface name derived from System.Net.NetworkInformation.NetworkInterface.Name
        /// </value>
        public string FriendlyName { get; internal set; }

        /// <value>
        /// Text description of the interface as given by pcap/winpcap
        /// </value>
        public string Description { get; internal set; }

        /// <value>
        /// Gateway address of this device
        /// NOTE: May only be available on Windows
        /// </value>
        public System.Net.IPAddress GatewayAddress { get; internal set; }

        /// <value>
        /// Addresses associated with this device
        /// </value>
        public List<PcapAddress> Addresses { get; internal set; }

        /// <value>
        /// Pcap interface flags
        /// </value>
        public uint Flags { get; internal set; }

        private PcapAddress m_macAddress;

        /// <summary>
        /// MacAddress of the interface
        /// </summary>
        public System.Net.NetworkInformation.PhysicalAddress MacAddress
        {
            get
            {
                return m_macAddress.Addr.hardwareAddress;
            }

            internal set
            {
                // do we already have a hardware address for this device?
                if(m_macAddress != null)
                {
#if false
                    Console.WriteLine("Overwriting hardware address "
                                      + m_macAddress.Addr.hardwareAddress.ToString()
                                      + " with " +
                                      value.ToString());
#endif
                    // overwrite the value with the new value
                    m_macAddress.Addr.hardwareAddress = value;
                } else
                {
#if false
                    Console.WriteLine("Creating new PcapAddress entry for this hardware address");
#endif
                    // create a new entry for the mac address
                    PcapAddress newAddress = new PcapAddress();
                    newAddress.Addr = new Sockaddr(value);

                    // add the address to our addresses list
                    Addresses.Add(newAddress);

                    // m_macAddress should point to this hardware address
                    m_macAddress = newAddress;
                }
            }
        }

        internal PcapInterface(PcapUnmanagedStructures.pcap_if pcapIf)
        {
            Name = pcapIf.Name;
            Description = pcapIf.Description;
            Flags = pcapIf.Flags;

            // retrieve addresses
            Addresses = new List<PcapAddress>();
            IntPtr address = pcapIf.Addresses;
            while(address != IntPtr.Zero)
            {
                //A sockaddr struct
                PcapUnmanagedStructures.pcap_addr addr;

                //Marshal memory pointer into a struct
                addr = (PcapUnmanagedStructures.pcap_addr)Marshal.PtrToStructure(address,
                                                                                 typeof(PcapUnmanagedStructures.pcap_addr));

                PcapAddress newAddress = new PcapAddress(addr);
                Addresses.Add(newAddress);

                // is this a hardware address?
                // if so we should set our internal m_macAddress member variable
                if((newAddress.Addr != null) &&
                   (newAddress.Addr.type == Sockaddr.AddressTypes.HARDWARE))
                {
                    if(m_macAddress == null)
                    {
                        m_macAddress = newAddress;
                    } else
                    {
                        throw new System.InvalidOperationException("found multiple hardware addresses, existing addr "
                                                                   + MacAddress.ToString() + ", new address " + newAddress.Addr.hardwareAddress.ToString());
                    }
                }

                address = addr.Next; // move to the next address
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
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Name: {0}\n", Name);
            if(FriendlyName != null)
            {
                sb.AppendFormat("FriendlyName: {0}\n", FriendlyName);
            }

            if (GatewayAddress != null)
            {
                sb.AppendFormat("GatewayAddress: {0}\n", GatewayAddress);
            }

            sb.AppendFormat("Description: {0}\n", Description);
            foreach(PcapAddress addr in Addresses)
            {
                sb.AppendFormat("Addresses:\n{0}\n", addr);
            }
            sb.AppendFormat("Flags: {0}\n", Flags);
            return sb.ToString();
        }
    }
}
