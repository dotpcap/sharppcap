using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap;
using SharpPcap.AirPcap;

namespace AirPcapBasicCapture
{
    class Program
    {
        static void Main(string[] args)
        {
            var devices = AirPcapDeviceList.Instance;

            if (devices.Count == 0)
            {
                Console.WriteLine("No devices found, are you running as admin(if in Windows), or root(if in Linux/Mac)?");
                return;
            }

            Console.WriteLine("Available AirPcap devices:");
            for (var i = 0; i < devices.Count; i++)
            {
                Console.WriteLine("[{0}] - {1}", i, devices[i].Name);
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            var devIndex = int.Parse(Console.ReadLine());

            var device = devices[devIndex];

            device.Open();

            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

            device.StartCapture();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            device.StopCapture();

            Console.WriteLine("-- Capture stopped.");

            // Print out the device statistics
            Console.WriteLine(device.Statistics.ToString());

            // Close the pcap device
            device.Close();
        }

        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var len = e.Packet.Data.Length;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(e.Packet.ToString());

            var p = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            Console.WriteLine(p.ToString(PacketDotNet.StringOutputType.VerboseColored));
        }
    }
}
