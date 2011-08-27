using System;
using SharpPcap;
using PacketDotNet;
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
                var captureDevice = new SharpPcap.LibPcap.CaptureFileReaderDevice("../../capture_files/10k_packets.pcap");
                captureDevice.Open();

                RawCapture rawCapture = null;
                do
                {
                    rawCapture = captureDevice.GetNextPacket();

                    // Parse the packet using PacketDotNet
                    if(rawCapture != null)
                        Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

                    packetsRead++;
                }
                while(rawCapture != null);

                captureDevice.Close();
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets parsed");

            Console.WriteLine("{0}", rate.ToString());
        }
    }
}

