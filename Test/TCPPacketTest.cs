using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.Packets;

namespace Test
{
    [TestFixture]
    public class TCPPacketTest
    {
        [Test]
        public virtual void TCPData()
        {
            var dev = new OfflinePcapDevice("../../capture_files/tcp_with_extra_bytes.pcap");
            dev.Open();

            Packet p;
            p = dev.GetNextPacket();

            Assert.IsNotNull(p);

            Console.WriteLine(p.GetType());
            Assert.IsTrue(p is TCPPacket);

            TCPPacket t = (TCPPacket)p;

            // even though the packet has 6 bytes of extra data, the ip packet shows a size of
            // 40 and the ip header has a length of 20. The TCP header is also 20 bytes so
            // there should be zero bytes in the TCPData value
            int expectedTcpDataLength = 0;
            Assert.AreEqual(expectedTcpDataLength, t.TCPData.Length);

            dev.Close();
        }        

        [Test]
        public virtual void Checksum()
        {
            var dev = new OfflinePcapDevice("../../capture_files/tcp.pcap");
            dev.Open();

            Packet p;
            p = dev.GetNextPacket();

            Assert.IsNotNull(p);

            Console.WriteLine(p.GetType());
            Assert.IsTrue(p is TCPPacket);

            TCPPacket t = (TCPPacket)p;

            Console.WriteLine("Checksum: "+t.Checksum.ToString("X"));
            Assert.IsTrue(t.ValidChecksum);

            dev.Close();
        }

        // Test that we can load and parse an IPv6 TCP packet
        [Test]
        public void IPDataSet()
        {
            TCPPacket p = TCPPacket.RandomPacket(IPPacket.IPVersions.IPv4);

            p.IPTotalLength = 100;
            Assert.AreEqual(100, p.IPTotalLength);
            Assert.AreEqual(80, p.IPPayloadLength);

            p.IPPayloadLength = 200;
            Assert.AreEqual(200, p.IPPayloadLength);
            Assert.AreEqual(220, p.IPTotalLength);
        }

        // Test that we can load and parse an IPv6 TCP packet
        [Test]
        public void TCPDataIPv4()
        {
            String s = "-++++=== HELLLLOOO ===++++-";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s);

            //create random pkt
            TCPPacket p = TCPPacket.RandomPacket(IPPacket.IPVersions.IPv4);

            //replace pkt's data with our string
            p.TCPData = data;

            //sanity check
            Assert.AreEqual(s, System.Text.Encoding.Default.GetString(p.TCPData));
        }

        [Test]
        public void TCPOptions()
        {
            var dev = new OfflinePcapDevice("../../capture_files/tcp.pcap");
            dev.Open();

            Packet p;
            p = dev.GetNextPacket();

            Assert.IsNotNull(p);

            Console.WriteLine(p.GetType());
            Assert.IsTrue(p is TCPPacket);

            TCPPacket t = (TCPPacket)p;

            // verify that the options byte match what we expect
            byte[] expectedOptions = new byte[] { 0x1, 0x1, 0x8, 0xa, 0x0, 0x14,
                                                  0x3d, 0xe5, 0x1d, 0xf5, 0xf8, 0x84 };
            Assert.AreEqual(expectedOptions, t.Options);

            dev.Close();
        }

        [Test]
        public void TCPConstructorFromValues()
        {
            byte[] srcHwAddressBytes = new byte[EthernetFields_Fields.MAC_ADDRESS_LENGTH];
            for(int i = 0; i < srcHwAddressBytes.Length; i++)
            {
                srcHwAddressBytes[i] = (byte)i;
            }

            byte[] dstHwAddressBytes = new byte[EthernetFields_Fields.MAC_ADDRESS_LENGTH];
            for(int i = 0; i < dstHwAddressBytes.Length; i++)
            {
                dstHwAddressBytes[i] = (byte)(dstHwAddressBytes.Length - i);
            }

            var srcHwAddress = new System.Net.NetworkInformation.PhysicalAddress(srcHwAddressBytes);
            var dstHwAddress = new System.Net.NetworkInformation.PhysicalAddress(dstHwAddressBytes);

            var srcIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            var dstIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
            var ipPayload = new byte[4];
            ipPayload[0] = 10;
            ipPayload[1] = 5;
            ipPayload[2] = 2;
            ipPayload[3] = 12;
            var ethernetPacket = new EthernetPacket(srcHwAddress, dstHwAddress, EthernetPacketType.None, null);
            var ipPacket = new IPPacket(IPPacket.IPVersions.IPv4,
                                        IPProtocol.IPProtocolType.NONE,
                                              srcIpAddress,
                                              dstIpAddress,
                                              ethernetPacket,
                                              ipPayload);

            int sourcePort = 1234;
            int destinationPort = 5678;
            byte[] tcpPayload = new byte[10];
            for(int i = 0; i < tcpPayload.Length; i++)
            {
                tcpPayload[i] = (byte)(i * 2);
            }

            var tcpPacket = new TCPPacket(ipPacket, sourcePort, destinationPort,
                                          tcpPayload);

            Console.WriteLine("tcpPacket: {0}", tcpPacket.ToString());
        }
    }
}