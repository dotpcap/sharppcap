using System;
using SharpPcap;
using NUnit.Framework;
using SharpPcap.LibPcap;

namespace Test.Performance
{
    [TestFixture]
    public class PacketReading
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
                    packetsRead++;
                }
                while (rawCapture != null);

            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("{0}", rate.ToString());
        }

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
                    packetsRead++;
                }
                while (rawCapture != null);
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("{0}", rate.ToString());
        }

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
                    packetsRead++;
                }
                while (rawCapture != null);
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("{0}", rate.ToString());
        }
    }
}

