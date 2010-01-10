using System;
using System.Collections.Generic;

namespace SharpPcap.Test.Example7
{
    public class DumpToFile
    {
        public static void Main(string[] args)
        {
            string ver = SharpPcap.Version.VersionString;
            /* Print SharpPcap version */
            Console.WriteLine("SharpPcap {0}, Example7.DumpToFile.cs", ver);
            Console.WriteLine();

            /* Retrieve the device list */
            var devices = PcapDeviceList.Instance;

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
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse( Console.ReadLine() );
            Console.Write("-- Please enter the output file name: ");
            string capFile = Console.ReadLine();

            PcapDevice device = devices[i];

            //Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += 
                new SharpPcap.Pcap.PacketArrivalEvent( device_PcapOnPacketArrival );

            //Open the device for capturing
            //true -- means promiscuous mode
            //1000 -- means a read wait of 1000ms
            device.Open(true, 1000);

            //Open or create a capture output file
            device.DumpOpen( capFile );

            Console.WriteLine();
            Console.WriteLine
                ("-- Listenning on {0}, hit 'Ctrl-C' to exit...",
                device.Description);

            //Start capture 'INFINTE' number of packets
            device.Capture( SharpPcap.Pcap.INFINITE );

            //Close the pcap device
            //(Note: these lines will never be called since
            // we're capturing infinite number of packets
            device.DumpFlush();
            device.DumpClose();
            device.Close();
        }

        /// <summary>
        /// Dumps each received packet to a pcap file
        /// </summary>
        private static void device_PcapOnPacketArrival(object sender, PcapCaptureEventArgs e)
        {                       
            PcapDevice device = (PcapDevice)sender;
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
