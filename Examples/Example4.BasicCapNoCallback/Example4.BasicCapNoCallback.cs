using System;
using System.Collections.Generic;
using SharpPcap;
using SharpPcap.Packets;

namespace SharpPcap.Test.Example4
{
    /// <summary>
    /// Basic capture example with no callback
    /// </summary>
    public class BasicCapNoCallback
    {
        /// <summary>
        /// Basic capture example
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            string ver = SharpPcap.Version.GetVersionString();
            /* Print SharpPcap version */
            Console.WriteLine("SharpPcap {0}, Example4.BasicCapNoCallback.cs", ver);

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
                Console.WriteLine("{0}) {1}",i,dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse( Console.ReadLine() );

            PcapDevice device = devices[i];

            //Open the device for capturing
            //true -- means promiscuous mode
            //1000 -- means a read wait of 1000ms
            device.Open(true, 1000);

            Console.WriteLine();
            Console.WriteLine("-- Listenning on {0}...",
                device.Description);

            Packet packet;

            //Keep capture packets using PcapGetNextPacket()
            while( (packet=device.GetNextPacket()) != null )
            {
                // Prints the time and length of each received packet
                DateTime time = packet.PcapHeader.Date;
                uint len = packet.PcapHeader.PacketLength;
                Console.WriteLine("{0}:{1}:{2},{3} Len={4}", 
                    time.Hour, time.Minute, time.Second, time.Millisecond, len);
            }

            //Close the pcap device
            device.Close();
            Console.WriteLine("-- Timeout (1000ms) elapsed, capture stopped, device closed.");
            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }
    }
}
