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

            string senderMacAddress = "000461990154";
            string targetMacAddress = "000000000000";
            Assert.AreEqual(senderMacAddress, arpPacket.ARPSenderHwAddress.ToString());
            Assert.AreEqual(targetMacAddress, arpPacket.ARPTargetHwAddress.ToString());
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

            string senderMacAddress = "00216A020854";
            string targetMacAddress = "000461990154";
            Assert.AreEqual(senderMacAddress, arpPacket.ARPSenderHwAddress.ToString());
            Assert.AreEqual(targetMacAddress, arpPacket.ARPTargetHwAddress.ToString());
        }

        [Test]
        public void ParsingArpPacketRequestResponse()
        {
            var dev = new OfflinePcapDevice("../../capture_files/arp_request_response.pcap");
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
