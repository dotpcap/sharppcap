﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Statistics;

namespace Test.Statistics
{
    [TestFixture]
    public class StatisticsDeviceTest
    {
        private const string Filter = "ether proto 0x1234";

        [Test]
        public void Test()
        {
            using var sender = TestHelper.GetPcapDevice();
            using var device = new StatisticsDevice(sender.Interface);
            var config = new DeviceConfiguration
            {
                // for stats device on Windows, this is the interval between Statistics
                ReadTimeout = 10,
            };
            sender.Open();
            device.Open(config);

            Assert.AreEqual(sender.LinkType, device.LinkType);

            Assert.IsNotEmpty(device.Name);
            Assert.AreEqual(device.Name, sender.Name);
            Assert.AreEqual(device.Description, sender.Description);
            Assert.AreEqual(device.MacAddress, sender.MacAddress);

            Assert.IsEmpty(device.LastError);

            var stats = new List<StatisticsEventArgs>();
            device.OnPcapStatistics += (s, e) =>
            {
                stats.Add(e);
            };

            device.Filter = Filter;
            Assert.AreEqual(Filter, device.Filter);
            device.StartCapture();

            var packet = EthernetPacket.RandomPacket();
            packet.Type = (EthernetType)0x1234;
            packet.PayloadData = new byte[60];

            var packetLength = packet.TotalPacketLength;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows count Ethernet Preamble, SFD and CRC
                packetLength += 12;
            }

            var count = 7;

            Thread.Sleep(100);
            for (int i = 0; i < count; i++)
            {
                sender.SendPacket(packet);
                // We do this to make sure we receive multiple stats
                Thread.Sleep(100);
            }

            device.Close();

            Assert.That(stats, Is.Not.Empty);
            var receivedPackets = stats.Select(s => s.ReceivedPackets);
            var receivedBytes = stats.Select(s => s.ReceivedBytes);

            Assert.That(receivedPackets, Is.Ordered);
            Assert.That(receivedBytes, Is.Ordered);

            foreach(var s in stats)
            {
                Assert.AreEqual(device, s.Device);
                Assert.GreaterOrEqual(DateTime.UtcNow, s.Timeval.Date);
            }

            Assert.That(receivedPackets.Last(), Is.EqualTo(count));
            Assert.That(receivedBytes.Last(), Is.EqualTo(count * packetLength));
        }

        /// <summary>
        /// Test that we get the appropriate exception from StartCapture() if
        /// there hasn't been any delegates assigned to OnPacketArrival or
        /// OnPcapStatistics
        /// </summary>
        [Test]
        public void DeviceNotReadyExceptionWhenStartingACaptureWithoutAddingDelegateToOnPcapStatistics()
        {
            using var device = new StatisticsDevice(TestHelper.GetPcapDevice().Interface);

            device.Open();

            Assert.Throws<DeviceNotReadyException>(() => device.StartCapture());
        }
    }
}
