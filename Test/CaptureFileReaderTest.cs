using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class CaptureFileReaderDeviceTest
    {
        private static int capturedPackets;

        /// <summary>
        /// Test that we can retrieve packets from a pcap file just as we would from
        /// a live capture device and that all packets are captured
        /// </summary>
        [Test]
        public void CaptureInfinite()
        {
            var device = new CaptureFileReaderDevice("../../capture_files/ipv6_http.pcap");
            device.OnPacketArrival += HandleDeviceOnPacketArrival;
            device.Open();

            var expectedPackets = 10;
            capturedPackets = 0;
            device.Capture();

            Assert.AreEqual(expectedPackets, capturedPackets);
        }

        /// <summary>
        /// Test that if we ask to capture a finite number of packets that
        /// only this number of packets will be captured
        /// </summary>
        [Test]
        public void CaptureFinite()
        {
            var device = new CaptureFileReaderDevice("../../capture_files/ipv6_http.pcap");
            device.OnPacketArrival += HandleDeviceOnPacketArrival;
            device.Open();

            var expectedPackets = 3;
            capturedPackets = 0;
            device.Capture(expectedPackets);

            Assert.AreEqual(expectedPackets, capturedPackets);
        }

        void HandleDeviceOnPacketArrival (object sender, CaptureEventArgs e)
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
            var device = new CaptureFileReaderDevice("../../capture_files/ipv6_http.pcap");

            var caughtExpectedException = false;
            try
            {
#pragma warning disable 0168
                var stats = device.Statistics;
#pragma warning restore 0168
            } catch(NotSupportedOnCaptureFileException)
            {
                caughtExpectedException = true;
            }

            Assert.IsTrue(caughtExpectedException);
        }

        [Test]
        public void SetFilter()
        {
            var device = new CaptureFileReaderDevice("../../capture_files/test_stream.pcap");

            device.Open();
            device.Filter = "port 53";

            RawCapture rawPacket;
            int count = 0;
            do
            {
                rawPacket = device.GetNextPacket();
                if(rawPacket != null)
                {
                    Packet p = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                    var udpPacket = UdpPacket.GetEncapsulated(p);
                    Assert.IsNotNull(udpPacket);
                    int dnsPort = 53;
                    Assert.AreEqual(dnsPort, udpPacket.DestinationPort);
                    count++;
                }
            } while(rawPacket != null);

            Assert.AreEqual(1, count);

            device.Close(); // close the device
        }
    }
}
