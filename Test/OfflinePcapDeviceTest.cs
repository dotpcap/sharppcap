using System;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class OfflinePcapDeviceTest
    {
        private static int capturedPackets;

        /// <summary>
        /// Test that we can retrieve packets from a pcap file just as we would from
        /// a live capture device
        /// </summary>
        [Test]
        public void OfflineDeviceWithCallback()
        {
            var offlineDevice = new OfflinePcapDevice("../../capture_files/ipv6_http.pcap");
            offlineDevice.OnPacketArrival += HandleOfflineDeviceOnPacketArrival;
            offlineDevice.Open();

            offlineDevice.Capture();

            offlineDevice.StopCapture();

            var expectedPackets = 10;

            Assert.AreEqual(expectedPackets, capturedPackets);
        }

        void HandleOfflineDeviceOnPacketArrival (object sender, CaptureEventArgs e)
        {
            Console.WriteLine("got packet " + e.Packet.ToString());
            capturedPackets++;
        }

        /// <summary>
        /// Test that we get the expected exception thrown when we call the Statistics()
        /// method on OfflinePcapDevice
        /// </summary>
        [Test]
        public void TestStatisticsException()
        {
            var offlineDevice = new OfflinePcapDevice("../../capture_files/ipv6_http.pcap");

            var caughtExpectedException = false;
            try
            {
                offlineDevice.Statistics();
            } catch(NotSupportedOnOfflineDeviceException)
            {
                caughtExpectedException = true;
            }

            Assert.IsTrue(caughtExpectedException);
        }

        [Test]
        public void SetFilter()
        {
            var offlineDevice = new OfflinePcapDevice("../../capture_files/test_stream.pcap");

            offlineDevice.Open();
            offlineDevice.SetFilter("port 53");

            RawPacket rawPacket;
            int count = 0;
            do
            {
                rawPacket = offlineDevice.GetNextPacket();
                if(rawPacket != null)
                {
                    Packet p = Packet.ParsePacket(rawPacket);
                    var udpPacket = UdpPacket.GetType(p);
                    Assert.IsNotNull(udpPacket);
                    int dnsPort = 53;
                    Assert.AreEqual(dnsPort, udpPacket.DestinationPort);
                    count++;
                }
            } while(rawPacket != null);

            Assert.AreEqual(1, count);

            offlineDevice.Close(); // close the device
        }
    }
}
