using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using SharpPcap;
using SharpPcap.Packets;

namespace Test
{
    
    public class UdpTest
    {
       
        public static void Run(string[] args)
        {
            int lLen = EthernetFields_Fields.ETH_HEADER_LEN;      
            const int MIN_PKT_LEN = 42;
            byte[] data = System.Text.Encoding.ASCII.GetBytes("HELLO");
            byte[] bytes = new byte[MIN_PKT_LEN + data.Length];
            Array.Copy(data, 0, bytes, MIN_PKT_LEN, data.Length);

            List<PcapDevice> devices = Pcap.GetAllDevices();
            PcapDevice device = devices[2];

            UDPPacket packet = new UDPPacket(lLen, bytes);

            //Ethernet Fields 
            packet.DestinationHwAddress = PhysicalAddress.Parse("001122334455");
            // NOTE: the source hw address will be filled in by the network stack or the
            //       network hardware
//          packet.SourceHwAddress = device.MacAddress;
            packet.EthernetProtocol = EthernetPacketType.IP;

            //IP Fields
            packet.DestinationAddress = System.Net.IPAddress.Parse("58.100.187.167");

            // NOTE: the source address will be filled in by the network stack based on
            //       the device used for sending
//          packet.SourceAddress = System.Net.IPAddress.Parse(device.IpAddress);
            packet.IPProtocol = IPProtocol.IPProtocolType.UDP;
            packet.TimeToLive = 20;
            packet.ipv4.Id = 100;
            packet.IPVersion = IPPacket.IPVersions.IPv4;
            packet.ipv4.IPTotalLength = bytes.Length - lLen;
            packet.ipv4.IPHeaderLength = IPv4Fields_Fields.IP_HEADER_LEN;

            //UDP Fields
            packet.DestinationPort = 9898;
            packet.SourcePort = 80;
            //TODO: checksum methods are disabled due to unfinished ipv4/ipv6 work
            throw new System.NotImplementedException();
//          packet.ComputeIPChecksum();
//          packet.ComputeUDPChecksum();
 
            device.Open();
            device.SendPacket(packet);
            device.Close();
        }
    }
}
