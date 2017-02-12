using System;
using System.Collections.Generic;
using SharpPcap;

namespace Example4
{
    /// <summary>
    /// Basic capture example with no callback
    /// </summary>
    public class BasicCapNoCallback
    {
        public static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example4.BasicCapNoCallback.cs", ver);

            // Retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error
            if(devices.Count < 1)
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
            foreach(var dev in devices)
            {
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse( Console.ReadLine() );

            var device = devices[i];

            // Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0}...",
                device.Description);

            RawCapture packet;

            // Capture packets using GetNextPacket()
            while( (packet = device.GetNextPacket()) != null )
            {
                // Prints the time and length of each received packet
                var time = packet.Timeval.Date;
                var len = packet.Data.Length;
                Console.WriteLine("{0}:{1}:{2},{3} Len={4}", 
                    time.Hour, time.Minute, time.Second, time.Millisecond, len);
            }

            // Print out the device statistics
            Console.WriteLine(device.Statistics.ToString());

            //Close the pcap device
            device.Close();
            Console.WriteLine("-- Timeout elapsed, capture stopped, device closed.");
            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }
    }
}
