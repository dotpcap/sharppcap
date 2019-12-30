using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;

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
                try
                {
                    device.Open();
                    if (device.LinkType == LinkLayers.Ethernet)
                    {
                        var nic = nics.FirstOrDefault(ni => ni.Name == device.Interface.FriendlyName);
                        if (nic.OperationalStatus == OperationalStatus.Up)
                        {
                            return device;
                        }
                    }
                    device.Close();
                }
                catch (Exception)
                {
                    continue;
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
    }
}
