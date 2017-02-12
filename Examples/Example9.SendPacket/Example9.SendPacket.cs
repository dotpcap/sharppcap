using System;
using System.Collections.Generic;
using SharpPcap;

namespace Example9
{
    public class DumpTCP
    {
        public static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example9.SendPacket.cs\n", ver);

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
                Console.WriteLine("{0}) {1}",i,dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to send a packet on: ");
            i = int.Parse( Console.ReadLine() );

            var device = devices[i];

            Console.Write("-- This will send a random packet out this interface, "+
                "continue? [YES|no]");
            string resp = Console.ReadLine().ToLower();

            // If user refused, exit program
            if((resp!="") && ( !resp.StartsWith("y")))
            {
                Console.WriteLine("Cancelled by user!");
                return;
            }

            //Open the device
            device.Open();
            
            //Generate a random packet
            byte[] bytes = GetRandomPacket();

            try
            {
                //Send the packet out the network device
                device.SendPacket( bytes );
                Console.WriteLine("-- Packet sent successfuly.");
            }
            catch(Exception e)
            {
                Console.WriteLine("-- "+ e.Message );
            }

            //Close the pcap device
            device.Close();
            Console.WriteLine("-- Device closed.");
            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }
        
        /// <summary>
        /// Generates a random packet of size 200
        /// </summary>
        private static byte[] GetRandomPacket()
        {
            byte[] packet = new byte[200];
            Random rand = new Random();
            rand.NextBytes( packet );
            return packet;
        }
    }
}
