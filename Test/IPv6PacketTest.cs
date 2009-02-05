using System;
using System.Net;
using NUnit.Framework;

using SharpPcap;
using SharpPcap.Packets;


namespace Test
{
    [TestFixture]
    public class IPv6PacketTest
    {   
        // Test that we can load and parse an IPv6 packet
        [Test]
        public void IPv6PacketTestParsing()
        {
            PcapOfflineDevice dev = Pcap.GetPcapOfflineDevice("../../capture_files/ipv6_icmpv6_packet.pcap");
            dev.PcapOpen();                                                                           

            Packet p;
            p = dev.PcapGetNextPacket();

            Assert.IsNotNull(p);

            Console.WriteLine(p.GetType());
            IPPacket ipPacket = (IPPacket)p;

            System.Net.IPAddress sourceAddress = System.Net.IPAddress.Parse("fe80::2a0:ccff:fed9:4175");
            Console.WriteLine("sourceAddress {0}", sourceAddress);
            Console.WriteLine("SourceAddress {0}", ipPacket.SourceAddress);
            Assert.AreEqual(sourceAddress, ipPacket.SourceAddress);

            System.Net.IPAddress destinationAddress = System.Net.IPAddress.Parse("ff02::2");
            Console.WriteLine("destinationAddress {0}", destinationAddress);
            Console.WriteLine("DestinationAddress {0}", ipPacket.DestinationAddress);
            Assert.AreEqual(destinationAddress, ipPacket.DestinationAddress);

            Assert.AreEqual(255, ipPacket.HopLimit);
            Assert.AreEqual(IPProtocol.IPProtocolType.ICMPV6, ipPacket.NextHeader);
            Assert.AreEqual(16, ipPacket.IPPayloadLength);

            dev.PcapClose();
        }
    }
}
