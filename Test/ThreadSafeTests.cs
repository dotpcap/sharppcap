// Copyright 2022 Ayoub Kaanich <kayoub5@live.com>
// SPDX-License-Identifier: MIT

using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    /// <summary>
    /// This is not about making PcapDevice 100% thread safe
    /// this is about prevent memory access violation
    /// </summary>
    [TestFixture]
    public class ThreadSafeTests
    {

        [Test]
        public void TestThreadSafety()
        {
            var pcapDevice = TestHelper.GetPcapDevice();
            var devices = Enumerable.Range(0, Environment.ProcessorCount)
                .Select(_ => new LibPcapLiveDevice(pcapDevice.Interface))
                .ToArray();

            foreach (var device in devices)
            {
                Initialize(device);
            }
            Thread.Sleep(TimeSpan.FromMinutes(1));
            foreach (var device in devices)
            {
                device.Dispose();
            }
        }

        private static void Initialize(LibPcapLiveDevice device)
        {
            var retry = 0;
            while (retry <= 7)
            {
                try
                {
                    device.Open(DeviceModes.Promiscuous);
                    device.OnPacketArrival += Device_OnPacketArrival;
                    device.StartCapture();
                    return;
                }
                catch (PcapException)
                {
                    // Race condition while closing/opening device, retry
                    retry++;
                }
            }
        }

        private static void Device_OnPacketArrival(object sender, PacketCapture e)
        {
            try
            {
                var device = (LibPcapLiveDevice)sender;
                Task.Run(() =>
                {
                    try
                    {
                        device.Dispose();
                        Thread.Sleep(1);
                        Initialize(device);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception happened in thread:{ex}");
                    }
                });
                // Trigger the data to be read from the pcap_t memory
                e.GetPacket();
            }
            catch (DeviceNotReadyException)
            {
                // Pass, normal
            }
            catch (ObjectDisposedException)
            {
                // Pass, normal
            }
        }
    }
}
