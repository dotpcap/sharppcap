using System;
using System.Net.NetworkInformation;
using NUnit.Framework;

using SharpPcap;
using SharpPcap.Packets;


namespace Test
{
    [TestFixture]
    public class IPv6PacketTest
    { 
        // icmpv6
        public void VerifyPacket0(Packet p)
        {
            Assert.IsNotNull(p);
            Console.WriteLine(p.ToString());

            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual(PhysicalAddress.Parse("00-A0-CC-D9-41-75"), e.SourceHwAddress);
            Assert.AreEqual(PhysicalAddress.Parse("33-33-00-00-00-02"), e.DestinationHwAddress);

            IPPacket ip = (IPPacket)p;
            Assert.AreEqual(System.Net.IPAddress.Parse("fe80::2a0:ccff:fed9:4175"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("ff02::2"), ip.DestinationAddress);
            Assert.AreEqual(IPPacket.IPVersions.IPv6, ip.IPVersion);
            Assert.AreEqual(IPProtocol.IPProtocolType.ICMPV6, ip.IPProtocol);
            Assert.AreEqual(16,  ip.IPPayloadLength);
            Assert.AreEqual(255, ip.HopLimit);
            Assert.AreEqual(255, ip.TimeToLive);
            Console.WriteLine("Failed: ip.ComputeIPChecksum() not implemented.");
//          Assert.AreEqual(0x5d50, ip.ComputeIPChecksum());
            Assert.AreEqual(1221145299, ip.Timeval.Seconds);
            Assert.AreEqual(453568.000, ip.Timeval.MicroSeconds);
        }

        // Test that we can load and parse an IPv6 packet
        [Test]
        public void IPv6PacketTestParsing()
        {
            var dev = new PcapOfflineDevice("../../capture_files/ipv6_icmpv6_packet.pcap");
            dev.Open();                                                                           

            Packet p;
            int packetIndex = 0;
            while((p = dev.GetNextPacket()) != null)
            {
                Console.WriteLine("got packet");
                switch(packetIndex)
                {
                case 0:
                    VerifyPacket0(p);
                    break;
                default:
                    Assert.Fail("didn't expect to get to packetIndex " + packetIndex);
                    break;
                }

                packetIndex++;
            }

            dev.Close();
        }

        // Test that we can load and parse an IPv6 TCP packet
        [Test]
        public void TCPChecksumIPv6()
        {
            var dev = new PcapOfflineDevice("../../capture_files/ipv6_http.pcap");
            dev.Open();

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
            while ((p = dev.GetNextPacket()) != null)
            {
                Assert.IsTrue(p is TCPPacket);
                TCPPacket t = (TCPPacket)p;
                Assert.IsTrue(t.ValidChecksum);

                // compare the computed checksum to the expected one
                Assert.AreEqual(expectedChecksum[packetIndex],
                                t.ComputeTCPChecksum());

                packetIndex++;
            }

            dev.Close();
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
