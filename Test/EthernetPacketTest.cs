using System;
using SharpPcap;
using SharpPcap.Packets;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class EthernetPacketTest
    {
        // tcp
        public void VerifyPacket0(Packet p)
        {
            Console.WriteLine(p.ToString());
            TCPPacket tcp = (TCPPacket)(p);
            Console.WriteLine(tcp.SourceHwAddress);

            EthernetPacket e = (EthernetPacket)p;            
            Assert.AreEqual(e.SourceHwAddress, "00:13:10:03:71:47");
            Assert.AreEqual(e.DestinationHwAddress, "00:e0:4c:e5:73:ad");

            IPPacket ip = (IPPacket)p;
            Assert.AreEqual(System.Net.IPAddress.Parse("82.165.240.134"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("192.168.1.221"), ip.DestinationAddress);
        }

        // tcp
        public void VerifyPacket1(Packet p)
        {
            Console.WriteLine(p.ToString());

            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual(e.SourceHwAddress, "00:16:cf:c9:1e:29");
            Assert.AreEqual(e.DestinationHwAddress,"00:14:bf:f2:ef:0a");

            IPPacket ip = (IPPacket)p;
            Assert.AreEqual(ip.SourceAddress, System.Net.IPAddress.Parse("192.168.1.104"));
            Assert.AreEqual(ip.DestinationAddress, System.Net.IPAddress.Parse("86.42.196.13"));
        }

        // udp
        public void VerifyPacket2(Packet p)
        {
            Console.WriteLine(p.ToString());
            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual(e.SourceHwAddress, "00:14:bf:f2:ef:0a");
            Assert.AreEqual(e.DestinationHwAddress, "00:16:cf:c9:1e:29");

            IPPacket ip = (IPPacket)p;
            Assert.AreEqual(System.Net.IPAddress.Parse("172.210.164.56"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("192.168.1.104"), ip.DestinationAddress);
        }

        // dns
        public void VerifyPacket3(Packet p)
        {
            Console.WriteLine(p.ToString());
            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual(e.SourceHwAddress, "00;16:cf:c9:1e:29");
            Assert.AreEqual(e.DestinationHwAddress, "00:14:bf:f2:ef:0a");

            IPPacket ip = (IPPacket)p;
            Assert.AreEqual(System.Net.IPAddress.Parse("192.168.1.172"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("66.189.0.29"), ip.DestinationAddress);
        }

        // arp
        public void VerifyPacket4(Packet p)
        {
            Console.WriteLine(p.ToString());
            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual(e.SourceHwAddress, "00:18:f8:4b:17:a0");
            Assert.AreEqual(e.DestinationHwAddress, "ff:ff:ff:ff:ff:ff");
        }

        // icmp
        public void VerifyPacket5(Packet p)
        {
            Console.WriteLine(p.ToString());
            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual(e.SourceHwAddress, "00:16:cf:c9:1e:29");
            Assert.AreEqual(e.DestinationHwAddress, "00:14:bf:f2:ef:0a");

            IPPacket ip = (IPPacket)p;
            Assert.AreEqual(System.Net.IPAddress.Parse("192.168.1.104"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("85.195.52.22"), ip.DestinationAddress);
        }

        /// <summary>
        /// Test parsing a handful of packets with known contents as verified by
        /// wireshark.
        /// </summary>
        [Test]
        public void TestParsingKnownPackets()
        {
            PcapOfflineDevice dev = Pcap.GetPcapOfflineDevice("../../capture_files/test_stream.pcap");
            dev.PcapOpen();

            Packet p;
            int packetIndex = 0;
            while((p = dev.PcapGetNextPacket()) != null)
            {
                Console.WriteLine("got packet");
                switch(packetIndex)
                {
                case 0:
                    VerifyPacket0(p);
                    break;
                case 1:
                    VerifyPacket1(p);
                    break;
                case 2:
                    VerifyPacket2(p);
                    break;
                case 3:
                    VerifyPacket3(p);
                    break;
                case 4:
                    VerifyPacket4(p);
                    break;
                case 5:
                    VerifyPacket5(p);
                    break;
                default:
                    Assert.Fail("didn't expect to get to packetIndex " + packetIndex);
                    break;
                }

                packetIndex++;
            }

            dev.PcapClose();
        }
    }
}
