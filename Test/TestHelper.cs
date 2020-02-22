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
        internal static PcapDevice GetPcapDevice()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var device in LibPcapLiveDeviceList.Instance)
            {
                var friendlyName = device.Interface.FriendlyName ?? string.Empty;
                if (friendlyName.ToLower().Contains("loopback") || friendlyName == "any")
                {
                    continue;
                }
                var nic = nics.FirstOrDefault(ni => ni.Name == friendlyName);
                if (nic?.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }
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
                finally
                {
                    if (device.Opened) device.Close();
                }

                if (link == LinkLayers.Ethernet)
                {
                    return device;
                }
            }
            return null;
        }

        /// <summary>
        /// Run a test routine, and report what packets were captured during that routine
        /// </summary>
        /// <param name="filter">to avoid noise from OS affecting test result, a filter is needed</param>
        /// <param name="routine">the routine to run</param>
        /// <returns></returns>
        internal static List<RawCapture> RunCapture(string filter, Action<PcapDevice> routine)
        {
            var device = GetPcapDevice();
            if (device == null)
            {
                throw new InvalidOperationException("No ethernet pcap supported devices found, are you running" +
                                                           " as a user with access to adapters (root on Linux)?");
            }
            Console.WriteLine($"Using device {device}");
            var received = new List<RawCapture>();
            device.Open();
            device.Filter = filter;
            void OnPacketArrival(object sender, CaptureEventArgs e)
            {
                received.Add(e.Packet);
            }
            device.OnPacketArrival += OnPacketArrival;
            device.StartCapture();
            try
            {
                routine(device);
            }
            finally
            {
                device.StopCapture();
                device.OnPacketArrival -= OnPacketArrival;
                device.Close();
            }
            return received;
        }

        internal static void ConfirmIdleState()
        {
            var devices = LibPcapLiveDeviceList.Instance;
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
    }
}
