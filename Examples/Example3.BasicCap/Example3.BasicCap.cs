using System;
using System.Collections.Generic;

namespace SharpPcap.Test.Example3
{
    /// <summary>
    /// Basic capture example
    /// </summary>
    public class BasicCap
    {
        /// <summary>
        /// Basic capture example
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            string ver = SharpPcap.Version.VersionString;
            /* Print SharpPcap version */
            Console.WriteLine("SharpPcap {0}, Example3.BasicCap.cs", ver);

            /* Retrieve the device list */
            List<PcapDevice> devices = SharpPcap.Pcap.GetAllDevices();

            /*If no device exists, print error */
            if(devices.Count<1)
            {
                Console.WriteLine("No device found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i=0;

            /* Scan the list printing every entry */
            foreach(PcapDevice dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse( Console.ReadLine() );

            PcapDevice device = devices[i];

            //Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += 
                new SharpPcap.Pcap.PacketArrivalEvent( device_PcapOnPacketArrival );

            //Open the device for capturing
            //true -- means promiscuous mode
            //1000 -- means a read wait of 1000ms
            device.Open(true, 1000);

            Console.WriteLine();
            Console.WriteLine("-- Listenning on {0}, hit 'Enter' to stop...",
                device.Description);

            //Start the capturing process
            device.StartCapture();

            //Wait for 'Enter' from the user.
            Console.ReadLine();

            //Stop the capturing process
            device.StopCapture();

            Console.WriteLine("-- Capture stopped.");

            // print out the device statistics
            Console.WriteLine(device.Statistics().ToString());

            //Close the pcap device
            device.Close();
        }

        /// <summary>
        /// Prints the time and length of each received packet
        /// </summary>
        private static void device_PcapOnPacketArrival(object sender, PcapCaptureEventArgs e)
        {
            DateTime time = e.Packet.PcapHeader.Date;
            uint len = e.Packet.PcapHeader.PacketLength;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}", 
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(e.Packet.ToColoredString(true));
        }
    }
}
