using System;
using SharpPcap;
using PacketDotNet;
using NUnit.Framework;
using SharpPcap.LibPcap;

namespace Test.Performance
{
    [TestFixture]
    public class PacketParsing
    {
        private readonly int packetsToRead = 10000000;

        [Category("Performance")]
        [Test]
        public void Benchmark()
        {
            int packetsRead = 0;
            var startTime = DateTime.Now;
            while (packetsRead < packetsToRead)
            {
                using var captureDevice = new CaptureFileReaderDevice(TestHelper.GetFile("10k_packets.pcap"));
                captureDevice.Open();

                RawCapture rawCapture = null;
                do
                {
                    rawCapture = captureDevice.GetNextPacket();

                    // Parse the packet using PacketDotNet
                    if (rawCapture != null)
                        Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

                    packetsRead++;
                }
                while (rawCapture != null);

            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets parsed");

            Console.WriteLine("{0}", rate.ToString());
        }

        /// <summary>
        /// Test the performance of a device used via the ICaptureDevice interface
        /// </summary>
        [Category("Performance")]
        [Test]
        public void BenchmarkICaptureDevice()
        {
            int packetsRead = 0;
            var startTime = DateTime.Now;
            while (packetsRead < packetsToRead)
            {
                using var captureDevice = new CaptureFileReaderDevice(TestHelper.GetFile("10k_packets.pcap"));
                captureDevice.Open();

                RawCapture rawCapture = null;
                do
                {
                    rawCapture = captureDevice.GetNextPacket();

                    // Parse the packet using PacketDotNet
                    if (rawCapture != null)
                        Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

                    packetsRead++;
                }
                while (rawCapture != null);

            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets parsed");

            Console.WriteLine("{0}", rate.ToString());
        }

        /// <summary>
        /// Test the performance of a device used via the ICaptureDevice interface
        /// </summary>
        [Category("Performance")]
        [Test]
        public unsafe void BenchmarkICaptureDeviceUnsafe()
        {
            int packetsRead = 0;
            var startTime = DateTime.Now;
            while (packetsRead < packetsToRead)
            {
                using var captureDevice = new CaptureFileReaderDevice(TestHelper.GetFile("10k_packets.pcap"));
                captureDevice.Open();

                RawCapture rawCapture = null;
                do
                {
                    rawCapture = captureDevice.GetNextPacket();

                    // Parse the packet using PacketDotNet
                    if (rawCapture != null)
                        Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

                    packetsRead++;
                }
                while (rawCapture != null);

            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets parsed");

            Console.WriteLine("{0}", rate.ToString());
        }
    }
}

