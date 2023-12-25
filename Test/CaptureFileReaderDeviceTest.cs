using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.IO;
using System.Collections.Generic;

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
        [TestCase(TimestampResolution.Nanosecond, "1186341404.189852000s")]
        [TestCase(TimestampResolution.Microsecond, "1186341404.189852s")]
        public void CaptureTimestampResolution(TimestampResolution resolution, string timeval)
        {
            var filename = "ipv6_http.pcap";
            using var device = new CaptureFileReaderDevice(TestHelper.GetFile(filename));
            var configuration = new DeviceConfiguration
            {
                TimestampResolution = resolution
            };
            device.Open(configuration);
            Assert.That(device.TimestampResolution, Is.EqualTo(resolution));
            device.GetNextPacket(out var packet);
            Assert.That(packet.Header.Timeval.ToString(), Is.EqualTo(timeval));
        }

        [Test]
        public void CaptureProperties()
        {
            var filename = "ipv6_http.pcap";
            using var device = new CaptureFileReaderDevice(TestHelper.GetFile(filename));
            device.Open();
            Assert.That(device.Description, Is.Not.Empty);
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
            Assert.That(device.FileSize, Is.EqualTo(ipv6FilesizeInBytes));
            Assert.That(device.Name, Is.EqualTo(fileToOpen));
            Assert.That(device.FileName, Is.EqualTo(filename));

            var expectedPackets = 10;
            capturedPackets = 0;
            device.Capture();

            Assert.That(capturedPackets, Is.EqualTo(expectedPackets));
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

            Assert.That(capturedPackets, Is.EqualTo(expectedPackets));
        }

        void HandleDeviceOnPacketArrival(object sender, PacketCapture e)
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
                Assert.That(device.Statistics, Is.Null);
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
            PacketCapture e;
            GetPacketStatus retval;
            do
            {
                retval = device.GetNextPacket(out e);
                if (retval == GetPacketStatus.PacketRead)
                {
                    rawPacket = e.GetPacket();
                    Packet p = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                    var udpPacket = p.Extract<UdpPacket>();
                    Assert.That(udpPacket, Is.Not.Null);
                    int dnsPort = 53;
                    Assert.That(udpPacket.DestinationPort, Is.EqualTo(dnsPort));
                    count++;
                }
            } while (retval == GetPacketStatus.PacketRead);

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        [LibpcapVersion(">=1.10.0")]
        public void StringMarshalling()
        {
            var original = TestHelper.GetFile("tcp.pcap");
            var names = new List<string> {
                "اختبار",
                "Prüfung",
                "测试",
            };
            var baseDir = Path.GetDirectoryName(GetType().Assembly.Location);
            foreach (var name in names)
            {
                var file = Path.Combine(baseDir, name + ".pcap");
                File.Copy(original, file, true);

                using var device = new CaptureFileReaderDevice(file);
                device.Open();
                Assert.That(device.GetNextPacket(out var _), Is.EqualTo(GetPacketStatus.PacketRead));
            }
        }
    }
}
