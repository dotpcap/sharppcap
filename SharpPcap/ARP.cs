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
using System.Net.NetworkInformation;

namespace SharpPcap
{
    /// <summary>
    /// Resolves MAC addresses from IP addresses using the Address Resolution Protocol (ARP)
    /// </summary>
    public class ARP
    {
        private LibPcap.LibPcapLiveDevice _device;

        /// <summary>
        /// Constructs a new ARP Resolver
        /// </summary>
        /// <param name="device">The network device on which this resolver sends its ARP packets</param>
        public ARP(LibPcap.LibPcapLiveDevice device)
        {
            _device = device;
        }

        private TimeSpan timeout = new TimeSpan(0, 0, 1);

        /// <summary>
        /// Timeout for a given call to Resolve()
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                return timeout;
            }

            set
            {
                timeout = value;
            }
        }

        /// <summary>
        /// Resolves the MAC address of the specified IP address. The 'DeviceName' propery must be set
        /// prior to using this method.
        /// </summary>
        /// <param name="destIP">The IP address to resolve</param>
        /// <returns>The MAC address that matches to the given IP address</returns>
        public PhysicalAddress Resolve(System.Net.IPAddress destIP)
        {
            return Resolve(destIP, null, null);
        }

        /// <summary>
        /// Resolves the MAC address of the specified IP address
        /// </summary>
        /// <param name="destIP">The IP address to resolve</param>
        /// <param name="localIP">The local IP address from which to send the ARP request, if null the local address will be discovered</param>
        /// <param name="localMAC">The localMAC address to use, if null the local mac will be discovered</param>
        /// <returns>The MAC address that matches to the given IP address or
        /// null if there was a timeout</returns>
        public PhysicalAddress Resolve(System.Net.IPAddress destIP,
                                       System.Net.IPAddress localIP,
                                       PhysicalAddress localMAC)
        {
            // if no local ip address is specified attempt to find one from the adapter
            if (localIP == null)
            {
                if (_device.Addresses.Count > 0)
                {
                    // attempt to find an ipv4 address.
                    // ARP is ipv4, NDP is used for ipv6
                    foreach(var address in _device.Addresses)
                    {
                        if(address.Addr.type == LibPcap.Sockaddr.AddressTypes.AF_INET_AF_INET6)
                        {
                            // make sure the address is ipv4
                            if (address.Addr.ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                localIP = address.Addr.ipAddress;
                                break; // break out of the foreach
                            }
                        }
                    }

                    // if we can't find either an ipv6 or an ipv4 address use the localhost address
                    if(localIP == null)
                    {
                        localIP = System.Net.IPAddress.Parse("127.0.0.1");
                    }
                }
            }

            // if no local mac address is specified attempt to find one from the device
            if(localMAC == null)
            {
                foreach(var address in _device.Addresses)
                {
                    if(address.Addr.type == LibPcap.Sockaddr.AddressTypes.HARDWARE)
                    {
                        localMAC = address.Addr.hardwareAddress;
                    }
                }
            }

            if(localIP == null)
            {
                throw new System.InvalidOperationException("Unable to find local ip address");
            }

            if(localMAC == null)
            {
                throw new System.InvalidOperationException("Unable to find local mac address");
            }

            //Build a new ARP request packet
            var request = BuildRequest(destIP, localMAC, localIP);

            //create a "tcpdump" filter for allowing only arp replies to be read
            String arpFilter = "arp and ether dst " + localMAC.ToString();

            //open the device with 20ms timeout
            _device.Open(DeviceMode.Promiscuous, 20);

            //set the filter
            _device.Filter = arpFilter;

            // set a last request time that will trigger sending the
            // arp request immediately
            var lastRequestTime = DateTime.FromBinary(0);

            var requestInterval = new TimeSpan(0, 0, 1);

            PacketDotNet.ARPPacket arpPacket = null;

            // attempt to resolve the address with the current timeout
            var timeoutDateTime = DateTime.Now + Timeout;
            while(DateTime.Now < timeoutDateTime)
            {
                if(requestInterval < (DateTime.Now - lastRequestTime))
                {
                    // inject the packet to the wire
                    _device.SendPacket(request);
                    lastRequestTime = DateTime.Now;
                }

                //read the next packet from the network
                var reply = _device.GetNextPacket();
                if(reply == null)
                {
                    continue;
                }

                // parse the packet
                var packet = PacketDotNet.Packet.ParsePacket(reply.LinkLayerType, reply.Data);

                // is this an arp packet?
                arpPacket = PacketDotNet.ARPPacket.GetEncapsulated(packet);
                if(arpPacket == null)
                {
                    continue;
                }

                //if this is the reply we're looking for, stop
                if(arpPacket.SenderProtocolAddress.Equals(destIP))
                {
                    break;
                }
            }

            // free the device
            _device.Close();

            // the timeout happened
            if(DateTime.Now >= timeoutDateTime)
            {
                return null;
            } else
            {
                //return the resolved MAC address
                return arpPacket.SenderHardwareAddress;
            }
        }

        private PacketDotNet.Packet BuildRequest(System.Net.IPAddress destinationIP,
                                                 PhysicalAddress localMac,
                                                 System.Net.IPAddress localIP)
        {
            // an arp packet is inside of an ethernet packet
            var ethernetPacket = new PacketDotNet.EthernetPacket(localMac,
                                                                 PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                                                                 PacketDotNet.EthernetPacketType.Arp);
            var arpPacket = new PacketDotNet.ARPPacket(PacketDotNet.ARPOperation.Request,
                                                       PhysicalAddress.Parse("00-00-00-00-00-00"),
                                                       destinationIP,
                                                       localMac,
                                                       localIP);

            // the arp packet is the payload of the ethernet packet
            ethernetPacket.PayloadPacket = arpPacket;

            return ethernetPacket;
        }
    }
}