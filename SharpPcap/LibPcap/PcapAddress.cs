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
        public Sockaddr Dstaddr {get; internal set; }

        internal PcapAddress()
        { }

        internal PcapAddress(PcapUnmanagedStructures.pcap_addr pcap_addr)
        {
            if(pcap_addr.Addr != IntPtr.Zero)
                Addr = new Sockaddr( pcap_addr.Addr );
            if(pcap_addr.Netmask != IntPtr.Zero)
                Netmask = new Sockaddr( pcap_addr.Netmask );
            if(pcap_addr.Broadaddr !=IntPtr.Zero)
                Broadaddr = new Sockaddr( pcap_addr.Broadaddr );
            if(pcap_addr.Dstaddr != IntPtr.Zero)
                Dstaddr = new Sockaddr( pcap_addr.Dstaddr );
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

            if(Addr != null)
                sb.AppendFormat("Addr:      {0}\n", Addr.ToString());

            if(Netmask != null)
                sb.AppendFormat("Netmask:   {0}\n", Netmask.ToString());

            if(Broadaddr != null)
                sb.AppendFormat("Broadaddr: {0}\n", Broadaddr.ToString());

            if(Dstaddr != null)
                sb.AppendFormat("Dstaddr:   {0}\n", Dstaddr.ToString());

            return sb.ToString();
        }
    }
}
