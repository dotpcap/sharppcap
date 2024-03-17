// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2009 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Text;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Managed representation of the unmanaged pcap_addr structure
    /// </summary>
    public class PcapAddress
    {
        /// <summary>
        /// The address value of this PcapAddress, null if none is present
        /// </summary>
        public Sockaddr Addr { get; internal set; }

        /// <summary>
        /// Netmask of this PcapAddress, null if none is present
        /// </summary>
        public Sockaddr Netmask { get; internal set; }

        /// <summary>
        /// Broadcast address of this PcapAddress, null if none is present
        /// </summary>
        public Sockaddr Broadaddr { get; internal set; }

        /// <summary>
        /// Destination address, null if the interface isn't a point-to-point interface
        /// </summary>
        public Sockaddr Dstaddr { get; internal set; }

        internal PcapAddress()
        { }

        internal PcapAddress(PcapUnmanagedStructures.pcap_addr pcap_addr)
        {
            if (pcap_addr.Addr != IntPtr.Zero)
                Addr = new Sockaddr(pcap_addr.Addr);
            if (pcap_addr.Netmask != IntPtr.Zero)
                Netmask = new Sockaddr(pcap_addr.Netmask);
            if (pcap_addr.Broadaddr != IntPtr.Zero)
                Broadaddr = new Sockaddr(pcap_addr.Broadaddr);
            if (pcap_addr.Dstaddr != IntPtr.Zero)
                Dstaddr = new Sockaddr(pcap_addr.Dstaddr);
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Addr != null)
                sb.AppendFormat("Addr:      {0}\n", Addr.ToString());

            if (Netmask != null)
                sb.AppendFormat("Netmask:   {0}\n", Netmask.ToString());

            if (Broadaddr != null)
                sb.AppendFormat("Broadaddr: {0}\n", Broadaddr.ToString());

            if (Dstaddr != null)
                sb.AppendFormat("Dstaddr:   {0}\n", Dstaddr.ToString());

            return sb.ToString();
        }
    }
}
