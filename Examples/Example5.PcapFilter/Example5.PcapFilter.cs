using System;
using System.Collections.Generic;
using SharpPcap;
using SharpPcap.Packets;

namespace SharpPcap.Test.Example5
{
    /// <summary>
    /// Basic capture example
    /// </summary>
    public class PcapFilter
    {
        /// <summary>
        /// Basic capture example
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            string ver = SharpPcap.Version.GetVersionString();
            /* Print SharpPcap version */
            Console.WriteLine("SharpPcap {0}, Example5.PcapFilter.cs", ver);
            Console.WriteLine();

            /* Retrieve the device list */
            List<PcapDevice> devices = SharpPcap.Pcap.GetAllDevices();

            /*If no device exists, print error */
            if(devices.Count<1)
            {
                Console.WriteLine("No device found on this machine");
                return;
            }
            
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i=0;

            /* Scan the list printing every entry */
            foreach(PcapDevice dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1}",i,dev.Description);
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

            //tcpdump filter to capture only TCP/IP packets         
            string filter = "ip and tcp";
            //Associate the filter with this capture
            device.SetFilter( filter );

            Console.WriteLine();
            Console.WriteLine
                ("-- The following tcpdump filter will be applied: \"{0}\"", 
                filter);
            Console.WriteLine
                ("-- Listenning on {0}, hit 'Ctrl-C' to exit...",
                device.Description);

            //Start capture packets
            device.Capture( SharpPcap.Pcap.INFINITE );

            //Close the pcap device
            //(Note: this line will never be called since
            // we're capturing infinite number of packets
            device.Close();
        }

        /// <summary>
        /// Prints the time and length of each received packet
        /// </summary>
        private static void device_PcapOnPacketArrival(object sender, Packet packet)
        {
            DateTime time = packet.PcapHeader.Date;
            uint len = packet.PcapHeader.PacketLength;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}", 
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
        }
    }
}
