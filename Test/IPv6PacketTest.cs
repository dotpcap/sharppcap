using System;
using System.Net;
using System.Collections.Generic;
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

        // Test that we can load and parse an IPv6 TCP packet
        [Test]
        public void TCPChecksumIPv6()
        {
            PcapOfflineDevice dev = Pcap.GetPcapOfflineDevice("../../capture_files/ipv6_http.pcap");
            dev.PcapOpen();

            Packet p;

            // checksums from wireshark of the capture file
            int[] expectedChecksum = {0x41a2,
                                      0x4201,
                                      0x5728,
                                      0xf448,
                                      0xee07,
                                      0x939c,
                                      0x63e4,
                                      0x4590,
                                      0x3725,
                                      0x3723};

            int packetIndex = 0;
            while ((p = dev.PcapGetNextPacket()) != null)
            {
                Assert.IsTrue(p is TCPPacket);
                TCPPacket t = (TCPPacket)p;
                Assert.IsTrue(t.ValidChecksum);

                // compare the computed checksum to the expected one
                Assert.AreEqual(expectedChecksum[packetIndex],
                                t.ComputeTCPChecksum());

                packetIndex++;
            }

            dev.PcapClose();
        }

        // Test that we can correctly set the data section of a IPv6 packet
        [Test]
        public void TCPDataIPv6()
        {
            String s = "-++++=== HELLLLOOO ===++++-";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s);

            //create random pkt
            TCPPacket p = TCPPacket.RandomPacket(IPPacket.IPVersions.IPv6);

            //replace pkt's data with our string
            p.TCPData = data;

            //sanity check
            Assert.AreEqual(s, System.Text.Encoding.Default.GetString(p.TCPData));
        }
    }
}
