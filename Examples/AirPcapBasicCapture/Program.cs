using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap.AirPcap;

namespace AirPcapBasicCapture
{
    class Program
    {
        static void Main(string[] args)
        {
            var devices = AirPcapDeviceList.GetDevices();

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

            ///FIXME: maybe create a separate project that uses the generic packet arrival
//            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            device.OnAirPcapPacketArrival += new AirPcapDevice.AirPcapPacketArrivalEventHandler(device_OnAirPcapPacketArrival);

            device.StartCapture();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            device.StopCapture();

            Console.WriteLine("-- Capture stopped.");

            // Print out the device statistics
//            Console.WriteLine(device.Statistics().ToString());

            // Close the pcap device
            device.Close();
        }

        static void device_OnAirPcapPacketArrival(object sender, AirPcapCaptureEventArgs e)
        {
            Console.WriteLine("AirPcap specific packet arrived {0}", e.Packet.ToString());
            var fields = e.Packet.RadioHeader.DecodeRadioTapFields();
            foreach (var field in fields)
            {
                Console.WriteLine(field.ToString());
            }

            var p = PacketDotNet.Packet.ParsePacket(e.Packet);
            Console.WriteLine(p.ToString());

#if false
            foreach(var b in e.Packet.Data)
            {
                Console.Write(" {0:x}", b);
            }
            Console.WriteLine();
#endif
        }

        static void device_OnPacketArrival(object sender, SharpPcap.CaptureEventArgs e)
        {
            Console.WriteLine("SharpPcap generic packet arrived, len={0}", e.Packet.Data.Length);
        }
    }
}
