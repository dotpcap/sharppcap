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
            CaptureEventArgs e;
            GetPacketStatus retval;
            while (packetsRead < packetsToRead)
            {
                using var captureDevice = new CaptureFileReaderDevice(TestHelper.GetFile("10k_packets.pcap"));
                captureDevice.Open();

                do
                {
                    retval = captureDevice.GetNextPacket(out e);
                    if (retval == GetPacketStatus.PacketRead) packetsRead++;
                }
                while (retval == GetPacketStatus.PacketRead);

            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("Benchmark {0}", rate.ToString());
        }

        [Category("Performance")]
        [Test]
        public void BenchmarkGetNextPacketSpan()
        {
            int packetsRead = 0;
            var startTime = DateTime.Now;
            GetPacketStatus res;

            CaptureEventArgs e;
            while (packetsRead < packetsToRead)
            {
                using var captureDevice = new CaptureFileReaderDevice(TestHelper.GetFile("10k_packets.pcap"));
                captureDevice.Open();

                do
                {
                    res = captureDevice.GetNextPacket(out e);
                    if (res == GetPacketStatus.PacketRead) packetsRead++;
                }
                while (res == GetPacketStatus.PacketRead);
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("BenchmarkGetNextPacketSpan {0}", rate.ToString());
        }

        [Category("Performance")]
        [Test]
        public unsafe void BenchmarkICaptureDeviceUnsafe()
        {
            int packetsRead = 0;
            var startTime = DateTime.Now;
            CaptureEventArgs e;
            GetPacketStatus retval;
            while (packetsRead < packetsToRead)
            {
                using var captureDevice = new CaptureFileReaderDevice(TestHelper.GetFile("10k_packets.pcap"));
                captureDevice.Open();

                do
                {
                    retval = captureDevice.GetNextPacket(out e);
                    if (retval == GetPacketStatus.PacketRead) packetsRead++;
                }
                while (retval == GetPacketStatus.PacketRead);
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("BenchmarkICaptureDeviceUnsafe {0}", rate.ToString());
        }
    }
}

