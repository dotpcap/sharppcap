using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.Packets;
using SharpPcap.Util;

namespace Test
{
    [TestFixture]
    public class InvalidPacketTest
    {
        // Test behavior when a IPPacket with an invalid length is parsed
        // We expect an exception to be thrown at the time of parsing, via PcapDevice.GetNextPacket()
        [Test]
        public void IPPacketInvalidLength()
        {
            PcapOfflineDevice dev = Pcap.GetPcapOfflineDevice("../../capture_files/ip_packet_bogus_length.pcap");
            dev.Open();

            bool caughtException = false;
            try
            {
                Packet p;
                int packetIndex = 0;
                while((p = dev.GetNextPacket()) != null)
                {
                    Console.WriteLine("got packet");
    
                    Assert.IsTrue(p is TCPPacket);

                    packetIndex++;
                }
            } catch
            {
                // expected to end up here
                caughtException = true;
            } finally
            {
                dev.Close();
            }

            Assert.IsTrue(caughtException, "We didn't catch the proper PcapException when parsing the invalid IPv4 packet");
        }
    }

}
