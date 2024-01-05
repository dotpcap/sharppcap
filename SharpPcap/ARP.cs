// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using SharpPcap.LibPcap;
using System;
using System.Net.NetworkInformation;

namespace SharpPcap
{
    /// <summary>
    /// Resolves MAC addresses from IP addresses using the Address Resolution Protocol (ARP)
    /// </summary>
    public class ARP
    {
        private readonly PcapInterface pcapInterface;

        /// <summary>
        /// Constructs a new ARP Resolver
        /// </summary>
        /// <param name="device">The network device on which this resolver sends its ARP packets</param>
        public ARP(LibPcapLiveDevice device)
        {
            pcapInterface = device.Interface;
        }

        /// <summary>
        /// Timeout for a given call to Resolve()
        /// </summary>
        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 1);

        /// <summary>
        /// Resolves the MAC address of the specified IP address. The 'DeviceName' propery must be set
        /// prior to using this method.
        /// </summary>
        /// <param name="destIP">The IP address to resolve</param>
        /// <returns>The MAC address that matches to the given IP address or
        /// null if there was a timeout</returns>
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
                // attempt to find an ipv4 address.
                // ARP is ipv4, NDP is used for ipv6
                foreach (var address in pcapInterface.Addresses)
                {
                    if (address.Addr.type == Sockaddr.AddressTypes.AF_INET_AF_INET6)
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
                if (localIP == null)
                {
                    localIP = System.Net.IPAddress.Parse("127.0.0.1");
                }
            }

            // if no local mac address is specified attempt to find one from the device
            if (localMAC == null)
            {
                foreach (var address in pcapInterface.Addresses)
                {
                    if (address.Addr.type == Sockaddr.AddressTypes.HARDWARE)
                    {
                        localMAC = address.Addr.hardwareAddress;
                    }
                }
            }

            if (localMAC == null)
            {
                throw new InvalidOperationException("Unable to find local mac address");
            }
            using (var device = new LibPcapLiveDevice(pcapInterface))
            {
                //open the device with 20ms timeout
                device.Open(mode: DeviceModes.Promiscuous, read_timeout: 20);
                return Resolve(device, destIP, localIP, localMAC, Timeout);
            }
        }

        internal static PhysicalAddress Resolve(
            ILiveDevice device,
            System.Net.IPAddress destIP,
            System.Net.IPAddress localIP,
            PhysicalAddress localMAC,
            TimeSpan timeout)
        {
            //Build a new ARP request packet
            var request = BuildRequest(destIP, localMAC, localIP);

            //create a "tcpdump" filter for allowing only arp replies to be read
            String arpFilter = "arp and ether dst " + localMAC.ToString();

            //set the filter
            device.Filter = arpFilter;

            // set a last request time that will trigger sending the
            // arp request immediately
            var lastRequestTime = DateTime.FromBinary(0);

            var requestInterval = new TimeSpan(0, 0, 1);

            PacketDotNet.ArpPacket arpPacket = null;

            // attempt to resolve the address with the current timeout
            var timeoutDateTime = DateTime.Now + timeout;
            while (DateTime.Now < timeoutDateTime)
            {
                if (requestInterval < (DateTime.Now - lastRequestTime))
                {
                    // inject the packet to the wire
                    device.SendPacket(request);
                    lastRequestTime = DateTime.Now;
                }

                //read the next packet from the network
                var retval = device.GetNextPacket(out PacketCapture e);
                if (retval != GetPacketStatus.PacketRead)
                {
                    continue;
                }
                var reply = e.GetPacket();

                // parse the packet
                var packet = PacketDotNet.Packet.ParsePacket(reply.LinkLayerType, reply.Data);

                // is this an arp packet?
                arpPacket = packet.Extract<PacketDotNet.ArpPacket>();
                if (arpPacket == null)
                {
                    continue;
                }

                //if this is the reply we're looking for, stop
                if (arpPacket.SenderProtocolAddress.Equals(destIP))
                {
                    break;
                }
            }

            // the timeout happened
            if (DateTime.Now >= timeoutDateTime)
            {
                return null;
            }
            else
            {
                //return the resolved MAC address
                return arpPacket.SenderHardwareAddress;
            }
        }


        private static PacketDotNet.Packet BuildRequest(System.Net.IPAddress destinationIP,
                                                 PhysicalAddress localMac,
                                                 System.Net.IPAddress localIP)
        {
            // an arp packet is inside of an ethernet packet
            var ethernetPacket = new PacketDotNet.EthernetPacket(localMac,
                                                                 PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                                                                 PacketDotNet.EthernetType.Arp);
            var arpPacket = new PacketDotNet.ArpPacket(PacketDotNet.ArpOperation.Request,
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