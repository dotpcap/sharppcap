using System;
using SharpPcap;
using NUnit.Framework;

namespace Test.Performance
{
    [TestFixture]
    public class PacketParsing
    {
        [Test]
        public void Benchmark()
        {
            int packetsToRead = 50000000;
            int packetsRead = 0;
            var startTime = DateTime.Now;
            while(packetsRead < packetsToRead)
            {
                var offlineDevice = new OfflineCaptureDevice("../../capture_files/10k_packets.pcap");
                offlineDevice.Open();

                RawCapture rawCapture = null;
                do
                {
                    rawCapture = offlineDevice.GetNextPacket();
                    packetsRead++;
                }
                while(rawCapture != null);

                offlineDevice.Close();
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets captured");

            Console.WriteLine("{0}", rate.ToString());
        }
    }
}

