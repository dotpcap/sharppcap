// Copyright 2011-2021 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

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
            PacketCapture e;
            GetPacketStatus retval;
            while (packetsRead < packetsToRead)
            {
                using var captureDevice = new CaptureFileReaderDevice(TestHelper.GetFile("10k_packets.pcap"));
                captureDevice.Open();

                do
                {
                    retval = captureDevice.GetNextPacket(out e);

                    // Parse the packet using PacketDotNet
                    if (retval == GetPacketStatus.PacketRead)
                    {
                        var rawCapture = e.GetPacket();
                        Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                        packetsRead++;
                    }
                }
                while (retval == GetPacketStatus.PacketRead);

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
            PacketCapture e;
            GetPacketStatus retval;
            while (packetsRead < packetsToRead)
            {
                using var captureDevice = new CaptureFileReaderDevice(TestHelper.GetFile("10k_packets.pcap"));
                captureDevice.Open();

                do
                {
                    retval = captureDevice.GetNextPacket(out e);

                    // Parse the packet using PacketDotNet
                    if (retval == GetPacketStatus.PacketRead)
                    {
                        var rawCapture = e.GetPacket();
                        Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                    }

                    packetsRead++;
                }
                while (retval == GetPacketStatus.PacketRead);

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
            PacketCapture e;
            GetPacketStatus retval;
            while (packetsRead < packetsToRead)
            {
                using var captureDevice = new CaptureFileReaderDevice(TestHelper.GetFile("10k_packets.pcap"));
                captureDevice.Open();

                do
                {
                    retval = captureDevice.GetNextPacket(out e);

                    // Parse the packet using PacketDotNet
                    if (retval == GetPacketStatus.PacketRead)
                    {
                        var rawCapture = e.GetPacket();
                        Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                    }

                    packetsRead++;
                }
                while (retval == GetPacketStatus.PacketRead);

            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsRead, "packets parsed");

            Console.WriteLine("{0}", rate.ToString());
        }
    }
}

