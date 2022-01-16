﻿using System;
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
            Assert.AreEqual(resolution, device.TimestampResolution);
            device.GetNextPacket(out var packet);
            Assert.AreEqual(timeval, packet.Header.Timeval.ToString());
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
                    Assert.IsNotNull(udpPacket);
                    int dnsPort = 53;
                    Assert.AreEqual(dnsPort, udpPacket.DestinationPort);
                    count++;
                }
            } while (retval == GetPacketStatus.PacketRead);

            Assert.AreEqual(1, count);
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
                Assert.AreEqual(GetPacketStatus.PacketRead, device.GetNextPacket(out var _));
            }
        }
    }
}
