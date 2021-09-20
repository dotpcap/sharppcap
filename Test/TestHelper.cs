using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Test
{
    internal static class TestHelper
    {
        public static string GetFile(string name)
        {
            var assembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(assembly, "capture_files", name);
        }


        /// <summary>
        /// Find the first Ethernet adapter that is actually connected to something
        /// </summary>
        /// <returns></returns>
        internal static LibPcapLiveDevice GetPcapDevice()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var inf in PcapInterface.GetAllPcapInterfaces())
            {
                var friendlyName = inf.FriendlyName ?? string.Empty;
                if (friendlyName.ToLower().Contains("loopback") || friendlyName == "any")
                {
                    continue;
                }
                if (friendlyName == "virbr0-nic")
                {
                    // Semaphore CI have this interface, and it's always down
                    // OperationalStatus does not detect it correctly
                    continue;
                }
                var nic = nics.FirstOrDefault(ni => ni.Name == friendlyName);
                if (nic?.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }
                using var device = new LibPcapLiveDevice(inf);
                LinkLayers link;
                try
                {
                    device.Open();
                    link = device.LinkType;
                }
                catch (PcapException ex)
                {
                    Console.WriteLine(ex);
                    continue;
                }

                if (link == LinkLayers.Ethernet)
                {
                    return device;
                }
            }
            throw new InvalidOperationException("No ethernet pcap supported devices found, are you running" +
                                           " as a user with access to adapters (root on Linux)?");
        }

        /// <summary>
        /// Run a test routine, and report what packets were captured during that routine
        /// </summary>
        /// <param name="filter">to avoid noise from OS affecting test result, a filter is needed</param>
        /// <param name="routine">the routine to run</param>
        /// <returns></returns>
        internal static List<RawCapture> RunCapture(string filter, Action<LibPcapLiveDevice> routine)
        {
            using var device = GetPcapDevice();
            Console.WriteLine($"Using device {device}");
            var received = new List<RawCapture>();
            var mode = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ?
                DeviceModes.Promiscuous :
                DeviceModes.None;

            device.Open(mode, 1);
            device.Filter = filter;
            // We can't use the same device for capturing and sending in Linux
            // The device simply does not receive its own sent traffic in Linux, not sure what the reason is.
            // Tested using Linux on Windows WSL
            var sender = device;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                sender = new LibPcapLiveDevice(device.Interface);
                sender.Open(mode, 1);
            }
            using(sender)
            {
                routine(sender);
                // waiting for any queued packets to be sent
                Thread.Sleep(10);
                var sw = Stopwatch.StartNew();
                while (true)
                {
                    var retval = device.GetNextPacket(out PacketCapture e);
                    if (retval == GetPacketStatus.PacketRead)
                    {
                        var packet = e.GetPacket();
                        Console.WriteLine($"Received: {packet} after {sw.Elapsed} (at {packet.Timeval})");
                        received.Add(packet);
                    }
                    else
                    {
                        Console.WriteLine($"Received: null packet after {sw.Elapsed})");
                        if (sw.ElapsedMilliseconds > 2000)
                        {
                            // No more packets in queue, and 2 second has passed
                            break;
                        }
                    }

                }
            }
            return received;
        }

        internal static void ConfirmIdleState()
        {
            var devices = DeviceFixture.GetDevices().OfType<PcapDevice>();
            foreach (var d in devices)
            {
                var isOpened = d.Opened;
                var isStarted = d.Started;
                if (isStarted) d.StopCapture();
                if (isOpened) d.Close();
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                // If test already failed, no point asserting here
                if (status != TestStatus.Failed)
                {
                    Assert.IsFalse(isOpened, "Expected device to not to be Opened");
                    Assert.IsFalse(isStarted, "Expected device to not be Started");
                }
            }
        }

        public static DeviceConfiguration StrictConfig(DeviceConfiguration configuration)
        {
            configuration.ConfigurationFailed += Configuration_ConfigurationFailed;
            return configuration;
        }

        private static void Configuration_ConfigurationFailed(object sender, ConfigurationFailedEventArgs e)
        {
            Assert.Fail(e.Message);
        }
    }
}
