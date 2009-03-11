using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.Packets;

namespace Test
{
    [TestFixture]
    public class UDPPacketTest
    {
        [Test]
        public void UDPData()
        {
            Packet p;
            UDPPacket u;

            PcapOfflineDevice dev = Pcap.GetPcapOfflineDevice("../../capture_files/udp_dns_request_response.pcap");
            dev.Open();

            // check the first packet
            p = dev.GetNextPacket();

            Assert.IsNotNull(p);
            Assert.IsTrue(p is UDPPacket);

            u = (UDPPacket)p;
            Assert.AreEqual(41 - u.UDPHeader.Length, u.UDPData.Length, "UDPData.Length mismatch");


            // check the second packet
            p = dev.GetNextPacket();

            Assert.IsNotNull(p);
            Assert.IsTrue(p is UDPPacket);

            u = (UDPPacket)p;
            Assert.AreEqual(356 - u.UDPHeader.Length, u.UDPData.Length, "UDPData.Length mismatch");

            Console.WriteLine("u is {0}", u.ToString());

            dev.Close();
        }
    }
}
