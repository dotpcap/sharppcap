// SPDX-License-Identifier: MIT

using System;
using SharpPcap;
using PacketDotNet;

namespace MultipleFiltersOnDevice
{
    /// <summary>
    /// Example that shows how to apply multiple filters to the same device
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            // Print SharpPcap version
            var ver = Pcap.SharpPcapVersion;
            Console.WriteLine("SharpPcap {0}, MultipleFiltersOnDevice", ver);

            // If no devices were found print an error
            if (CaptureDeviceList.Instance.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            // Print out the devices
            foreach (var dev in CaptureDeviceList.Instance)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse(Console.ReadLine());

            int readTimeoutMilliseconds = 1000;

            using var device1 = CaptureDeviceList.Instance[i];
            using var device2 = CaptureDeviceList.New()[i]; // NOTE: the call to New()

            // Register our handler function to the 'packet arrival' event
            device1.OnPacketArrival +=
                        new PacketArrivalEventHandler(device_OnPacketArrival);
            device2.OnPacketArrival +=
                        new PacketArrivalEventHandler(device_OnPacketArrival);

            // Open the devices for capturing
            device1.Open(DeviceModes.Promiscuous, readTimeoutMilliseconds);
            device2.Open(DeviceModes.Promiscuous, readTimeoutMilliseconds);

            // set the filters
            device1.Filter = "tcp port 80"; // http
            device2.Filter = "udp port 53"; // dns

            Console.WriteLine("device1.Filter {0}, device2.Filter {1}",
                              device1.Filter, device2.Filter);

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0} {1}, hit 'Enter' to stop...",
                device1.Name, device1.Description);

            // Start the capturing process
            device1.StartCapture();
            device2.StartCapture();

            // Wait for 'Enter' from the user.
            Console.ReadLine();

            // Stop the capturing process
            device1.StopCapture();
            device2.StopCapture();

            Console.WriteLine("-- Capture stopped.");

            // Print out the device statistics
            Console.WriteLine("device1 {0}", device1.Statistics.ToString());
            Console.WriteLine("device2 {0}", device2.Statistics.ToString());
        }

        /// <summary>
        /// Prints the time and length of each received packet
        /// </summary>
        private static void device_OnPacketArrival(object sender, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            var time = rawPacket.Timeval.Date;
            var len = rawPacket.Data.Length;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            var p = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            Console.WriteLine(p.ToString());
        }
    }
}
