using System;
using System.Collections.Generic;

namespace SharpPcap.Test.Example7
{
    public class DumpToFile
    {
        public static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example7.DumpToFile.cs\n", ver);

            // Retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error
            if(devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            // Print out the available devices
            foreach(var dev in devices)
            {
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse( Console.ReadLine() );
            Console.Write("-- Please enter the output file name: ");
            string capFile = Console.ReadLine();

            var device = devices[i];

            // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += 
                new PacketArrivalEventHandler( device_OnPacketArrival );

            // Open the device for capturing
            device.Open();

            // Open or create a capture output file
            device.DumpOpen( capFile );

            Console.WriteLine();
            Console.WriteLine
                ("-- Listening on {0}, hit 'Ctrl-C' to exit...",
                device.Description);

            // Start capture 'INFINTE' number of packets
            device.Capture();

            // Close the pcap device
            // (Note: these lines will never be called since
            //  we're capturing infinite number of packets
            device.DumpFlush();
            device.DumpClose();
            device.Close();
        }

        /// <summary>
        /// Dumps each received packet to a pcap file
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {                       
            var device = (ICaptureDevice)sender;

            //if device has a dump file opened
            if( device.DumpOpened )
            {
                //dump the packet to the file
                device.Dump( e.Packet );
                Console.WriteLine("Packet dumped to file.");
            }
        }
    }
}
