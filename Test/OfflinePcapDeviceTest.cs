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
        /// a live capture device and that all packets are captured
        /// </summary>
        [Test]
        public void CaptureInfinite()
        {
            var offlineDevice = new OfflineCaptureDevice("../../capture_files/ipv6_http.pcap");
            offlineDevice.OnPacketArrival += HandleOfflineDeviceOnPacketArrival;
            offlineDevice.Open();

            var expectedPackets = 10;
            capturedPackets = 0;
            offlineDevice.Capture();

            Assert.AreEqual(expectedPackets, capturedPackets);
        }

        /// <summary>
        /// Test that if we ask to capture a finite number of packets that
        /// only this number of packets will be captured
        /// </summary>
        [Test]
        public void CaptureFinite()
        {
            var offlineDevice = new OfflineCaptureDevice("../../capture_files/ipv6_http.pcap");
            offlineDevice.OnPacketArrival += HandleOfflineDeviceOnPacketArrival;
            offlineDevice.Open();

            var expectedPackets = 3;
            capturedPackets = 0;
            offlineDevice.Capture(expectedPackets);

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
            var offlineDevice = new OfflineCaptureDevice("../../capture_files/ipv6_http.pcap");

            var caughtExpectedException = false;
            try
            {
#pragma warning disable 0168
                var stats = offlineDevice.Statistics;
#pragma warning restore 0168
            } catch(NotSupportedOnOfflineDeviceException)
            {
                caughtExpectedException = true;
            }

            Assert.IsTrue(caughtExpectedException);
        }

        [Test]
        public void SetFilter()
        {
            var offlineDevice = new OfflineCaptureDevice("../../capture_files/test_stream.pcap");

            offlineDevice.Open();
            offlineDevice.Filter = "port 53";

            RawPacket rawPacket;
            int count = 0;
            do
            {
                rawPacket = offlineDevice.GetNextPacket();
                if(rawPacket != null)
                {
                    Packet p = Packet.ParsePacket(rawPacket);
                    var udpPacket = UdpPacket.GetEncapsulated(p);
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
