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

        [Test]
        public void CaptureNonExistant()
        {
            Assert.Throws<PcapException>(() =>
            {
                using var device = new CaptureFileReaderDevice("a_fake_filename.pcap");
                device.Open();
            });
        }

        [Category("Timestamp")]
        [Test]
        public void CaptureTimestampResolution()
        {
            var filename = "ipv6_http.pcap";
            using var device = new CaptureFileReaderDevice(TestHelper.GetFile(filename));

            // nanosecond configuration
            var configuration = new DeviceConfiguration
            {
                TimestampResolution = TimestampResolution.Nanosecond
            };
            device.Open(configuration);
            Assert.AreEqual(configuration.TimestampResolution, device.TimestampResolution);
            device.Close();

            // microsecond configuration
            configuration.TimestampResolution = TimestampResolution.Microsecond;
            device.Open(configuration);
            Assert.AreEqual(configuration.TimestampResolution, device.TimestampResolution);
            device.Close();
        }

        [Test]
        public void CaptureProperties()
        {
            var filename = "ipv6_http.pcap";
            using var device = new CaptureFileReaderDevice(TestHelper.GetFile(filename));
            device.Open();
            Assert.IsNotEmpty(device.Description);
        }

        /// <summary>
        /// Test that we can retrieve packets from a pcap file just as we would from
        /// a live capture device and that all packets are captured
        /// </summary>
        [Test]
        public void CaptureInfinite()
        {
            var filename = "ipv6_http.pcap";
            var fileToOpen = TestHelper.GetFile(filename);
            using var device = new CaptureFileReaderDevice(fileToOpen);
            device.OnPacketArrival += HandleDeviceOnPacketArrival;
            device.Open();
            var ipv6FilesizeInBytes = 3451;
            Assert.AreEqual(ipv6FilesizeInBytes, device.FileSize);
            Assert.AreEqual(fileToOpen, device.Name);
            Assert.AreEqual(filename, device.FileName);

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
            using var device = new CaptureFileReaderDevice(TestHelper.GetFile("ipv6_http.pcap"));
            device.OnPacketArrival += HandleDeviceOnPacketArrival;
            device.Open();

            var expectedPackets = 3;
            capturedPackets = 0;
            device.Capture(expectedPackets);

            Assert.AreEqual(expectedPackets, capturedPackets);
        }

        void HandleDeviceOnPacketArrival(object sender, CaptureEventArgs e)
        {
            Console.WriteLine("got packet " + e.GetPacket().ToString());
            capturedPackets++;
        }

        /// <summary>
        /// Test that we get expected unsupport indication when attempting to retrieve
        /// Statistics from this device
        /// </summary>
        [Test]
        public void StatisticsUnsupported()
        {
            using (var device = new CaptureFileReaderDevice(TestHelper.GetFile("ipv6_http.pcap")))
            {
                device.Open();
                Assert.IsNull(device.Statistics);
            }
        }

        [Test]
        public void SetFilter()
        {
            using var device = new CaptureFileReaderDevice(TestHelper.GetFile("test_stream.pcap"));

            device.Open();
            device.Filter = "port 53";

            RawCapture rawPacket;
            int count = 0;
            CaptureEventArgs e;
            GetPacketStatus retval;
            do
            {
                retval = device.GetNextPacket(out e);
                if (retval == GetPacketStatus.PacketRead)
                {
                    rawPacket = e.GetPacket();
                    Packet p = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                    var udpPacket = p.Extract<UdpPacket>();
                    Assert.IsNotNull(udpPacket);
                    int dnsPort = 53;
                    Assert.AreEqual(dnsPort, udpPacket.DestinationPort);
                    count++;
                }
            } while (retval == GetPacketStatus.PacketRead);

            Assert.AreEqual(1, count);
        }
    }
}
