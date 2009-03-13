using System;
using System.Net;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.Packets;

namespace Test
{
    [TestFixture]
    public class ArpPacketTest
    {
        // arp request
        private void VerifyPacket0(Packet p)
        {
            Assert.IsTrue(p is ARPPacket, "p isn't an ARPPacket");

            ARPPacket arpPacket = (ARPPacket)p;

            IPAddress senderIp = IPAddress.Parse("192.168.1.202");
            IPAddress targetIp = IPAddress.Parse("192.168.1.214");

            Assert.AreEqual(senderIp, arpPacket.ARPSenderProtoAddress);
            Assert.AreEqual(targetIp, arpPacket.ARPTargetProtoAddress);

            string senderMacAddress = "00:04:61:99:01:54";
            string targetMacAddress = "00:00:00:00:00:00";
            Assert.AreEqual(senderMacAddress, arpPacket.ARPSenderHwAddress);
            Assert.AreEqual(targetMacAddress, arpPacket.ARPTargetHwAddress);
        }

        // arp response
        private void VerifyPacket1(Packet p)
        {
            Assert.IsTrue(p is ARPPacket, "p isn't an ARPPacket");
            ARPPacket arpPacket = (ARPPacket)p;

            IPAddress senderIp = IPAddress.Parse("192.168.1.214");
            IPAddress targetIp = IPAddress.Parse("192.168.1.202");

            Assert.AreEqual(senderIp, arpPacket.ARPSenderProtoAddress);
            Assert.AreEqual(targetIp, arpPacket.ARPTargetProtoAddress);

            string senderMacAddress = "00:21:6a:02:08:54";
            string targetMacAddress = "00:04:61:99:01:54";
            Assert.AreEqual(senderMacAddress, arpPacket.ARPSenderHwAddress);
            Assert.AreEqual(targetMacAddress, arpPacket.ARPTargetHwAddress);
        }

        [Test]
        public void ParsingArpPacketRequestResponse()
        {
            PcapOfflineDevice dev = Pcap.GetPcapOfflineDevice("../../capture_files/arp_request_response.pcap");
            dev.Open();                                                                           

            Packet p;
            int packetIndex = 0;
            while((p = dev.GetNextPacket()) != null)
            {
                Console.WriteLine("got packet");
                Console.WriteLine("{0}", p.ToString());
                switch(packetIndex)
                {
                case 0:
                    VerifyPacket0(p);
                    break;
                case 1:
                    VerifyPacket1(p);
                    break;
                default:
                    Assert.Fail("didn't expect to get to packetIndex " + packetIndex);
                    break;
                }

                packetIndex++;
            }

            dev.Close();
        }
    }
}
