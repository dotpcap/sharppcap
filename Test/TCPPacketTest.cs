using System;
using NUnit.Framework;
using SharpPcap.Packets;

namespace SharpPcap.Util
{
    [TestFixture]
    public class TCPPacketTest
    {
                
        [TestFixtureSetUp]
        public virtual void  SetUp()
        {
        }

        [Test]
        public virtual void Checksum()
        {
            PcapOfflineDevice dev = Pcap.GetPcapOfflineDevice("../../capture_files/tcp.pcap");
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
    }
}