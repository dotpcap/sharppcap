using System;
using SharpPcap;
using NUnit.Framework;

namespace Test.Performance
{
    [TestFixture]
    public class PacketReading
    {
        private int packetsToRead = 50000000;

        [Test]
        public void Benchmark()
        {
            int packetsRead = 0;
            var startTime = DateTime.Now;
            while(packetsRead < packetsToRead)
            {
                var captureDevice = new SharpPcap.LibPcap.CaptureFileReaderDevice("../../capture_files/10k_packets.pcap");
                captureDevice.Open();

                RawCapture rawCapture = null;
                do
                {
                    rawCapture = captureDevice.GetNextPacket();
                    packetsRead++;
                }
                while(rawCapture != null);

                captureDevice.Close();
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("{0}", rate.ToString());
        }

        [Test]
        public void BenchmarkICaptureDevice()
        {
            int packetsRead = 0;
            var startTime = DateTime.Now;
            while(packetsRead < packetsToRead)
            {
                ICaptureDevice captureDevice = new SharpPcap.LibPcap.CaptureFileReaderDevice("../../capture_files/10k_packets.pcap");
                captureDevice.Open();

                RawCapture rawCapture = null;
                do
                {
                    rawCapture = captureDevice.GetNextPacket();
                    packetsRead++;
                }
                while(rawCapture != null);

                captureDevice.Close();
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("{0}", rate.ToString());
        }

        [Test]
        public unsafe void BenchmarkICaptureDeviceUnsafe()
        {
            int packetsRead = 0;
            var startTime = DateTime.Now;
            while(packetsRead < packetsToRead)
            {
                ICaptureDevice captureDevice = new SharpPcap.LibPcap.CaptureFileReaderDevice("../../capture_files/10k_packets.pcap");
                captureDevice.Open();

                RawCapture rawCapture = null;
                do
                {
                    rawCapture = captureDevice.GetNextPacket();
                    packetsRead++;
                }
                while(rawCapture != null);

                captureDevice.Close();
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("{0}", rate.ToString());
        }
    }
}

