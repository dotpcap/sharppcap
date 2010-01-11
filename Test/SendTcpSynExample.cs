using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using SharpPcap;
using SharpPcap.Packets;

/*
Copyright (c) 2006 Tamir Gal, http://www.tamirgal.com, All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice,
        this list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright 
        notice, this list of conditions and the following disclaimer in 
        the documentation and/or other materials provided with the distribution.

    3. The names of the authors may not be used to endorse or promote products
        derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace Test
{
    /// <summary>
    /// A sample showing how to build a SYN packet.
    /// The program prompt the user for a Network Interface, builds a TCP SYN packet,
    /// injects it through the interface and print the SYN/ACK reply from the remote
    /// host.
    /// </summary>
    public class SendTcpSynExample
    {
        static int lLen = EthernetFields_Fields.ETH_HEADER_LEN;

        /// <summary>
        /// The IP address of the remote host, please change to your desired IP address
        /// </summary>
        static System.Net.IPAddress destIP = System.Net.IPAddress.Parse("10.0.0.100");
        
        /// <summary>
        /// The MAC address of the next hop (e.g. gateway or a host on local LAN)
        /// </summary>      
        static string destMAC = "00:00:44:77:ac:01";

        /// <summary>
        /// Destination port
        /// </summary>
        static int destPort = 80;

        /// <summary>
        /// Arbitrary source port
        /// </summary>
        static int sourcePort = 2222;
        

        public static void SendTcpSyn(PcapDevice dev)
        {
            byte[] bytes = new byte[54];

            TCPPacket tcp = new TCPPacket(lLen, bytes, true);

            //Ethernet fields
//          tcp.SourceHwAddress = dev.MacAddress;           //Set the source mac of the local device
            tcp.DestinationHwAddress = PhysicalAddress.Parse(destMAC);     //Set the dest MAC of the gateway
            tcp.EthernetProtocol = EthernetPacketType.IpV4;

            //IP fields
            tcp.DestinationAddress = destIP;            //The IP of the destination host
//          tcp.SourceAddress = System.Net.IPAddress.Parse(dev.IpAddress);          //The IP of the local device
            tcp.IPProtocol = IPProtocol.IPProtocolType.TCP;
            tcp.TimeToLive = 20;
            tcp.ipv4.Id = 100;          
            tcp.IPVersion = IPPacket.IPVersions.IPv4;
            tcp.ipv4.IPTotalLength = bytes.Length-lLen;         //Set the correct IP length
            tcp.ipv4.IPHeaderLength = IPv4Fields_Fields.IP_HEADER_LEN;

            //TCP fields
            tcp.SourcePort = sourcePort;                //The TCP source port
            tcp.DestinationPort = destPort;             //The TCP dest port
            tcp.Syn = true;                     //Set the SYN flag on
            tcp.WindowSize = 555;
            tcp.AcknowledgmentNumber = 1000;
            tcp.SequenceNumber = 1000;          
            tcp.TCPHeaderLength = TCPFields_Fields.TCP_HEADER_LEN;          //Set the correct TCP header length

            //tcp.SetData( System.Text.Encoding.ASCII.GetBytes("HELLO") );

            //Calculate checksums
            //TODO: need to implement the checksumming routines
            throw new System.NotImplementedException();
//          tcp.ComputeIPChecksum();
//          tcp.ComputeTCPChecksum();

            dev.Open(DeviceMode.Promiscuous, 20);
            
            //Set a filter to capture only replies
            //FIXME: PcapDevice doesn't have an IpAddress. Not sure if the more permissive
            //       filter will work the same
//          dev.PcapSetFilter("ip src "+destIP+" and ip dst "+
//              dev.IpAddress+" and tcp src port "+destPort+" and tcp dst port "+sourcePort);
            dev.SetFilter("ip src "+destIP+
                " and tcp src port "+destPort+" and tcp dst port "+sourcePort);

            //Send the packet
            Console.Write("Sending packet: "+tcp+"...");
            dev.SendPacket(tcp);
            Console.WriteLine("Packet sent.");

            //Holds the reply
            Packet reply;
            //Wait till you get a reply
            while((reply=dev.GetNextPacket())==null);
            //print the reply
            Console.WriteLine("Reply received: "+reply);

            dev.Close();
        }

        public static void Main1(string[] args)
        {
            string ver = SharpPcap.Version.VersionString;
            /* Print SharpPcap version */
            Console.WriteLine("SharpPcap {0}, SendTcpSynExample.cs", ver);
            Console.WriteLine();

            /* Retrieve the device list */
            List<PcapDevice> devices = Pcap.GetAllDevices();

            /*If no device exists, print error */
            if(devices.Count<1)
            {
                Console.WriteLine("No device found on this machine");
                return;
            }
            
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i=0;

            /* Scan the list printing every entry */
            foreach(PcapDevice dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1}",i,dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device for sending: ");
            i = int.Parse( Console.ReadLine() );

            PcapDevice device = devices[i];

            SendTcpSyn(device);
        }
    }
}
