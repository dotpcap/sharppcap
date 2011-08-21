using System;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace ReadingCaptureFile
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            string ver = SharpPcap.Version.VersionString;

            /* Print SharpPcap version */
            Console.WriteLine("SharpPcap {0}, ReadingCaptureFile", ver);
            Console.WriteLine();

            Console.WriteLine();

            // read the file from stdin or from the command line arguments
            string capFile;
            if(args.Length == 0)
            {
                Console.Write("-- Please enter an input capture file name: ");
                capFile = Console.ReadLine();
            } else
            {
                // use the first argument as the filename
                capFile = args[0];
            }

            Console.WriteLine("opening '{0}'", capFile);

            ICaptureDevice device;

            try
            {
                // Get an offline device
                device = new CaptureFileReaderDevice( capFile );

                // Open the device
                device.Open();
            }
            catch(Exception e)
            {
                Console.WriteLine("Caught exception when opening file" + e.ToString());
                return;
            }

            // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival +=
                new PacketArrivalEventHandler( device_OnPacketArrival );

            Console.WriteLine();
            Console.WriteLine
                ("-- Capturing from '{0}', hit 'Ctrl-C' to exit...",
                capFile);

            // Start capture 'INFINTE' number of packets
            // This method will return when EOF reached.
            device.Capture();

            // Close the pcap device
            device.Close();
            Console.WriteLine("-- End of file reached.");
            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }

        private static int packetIndex = 0;

        /// <summary>
        /// Prints the source and dest MAC addresses of each received Ethernet frame
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            if(e.Packet.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                var ethernetPacket = (PacketDotNet.EthernetPacket)packet;

                Console.WriteLine("{0} At: {1}:{2}: MAC:{3} -> MAC:{4}",
                                  packetIndex,
                                  e.Packet.Timeval.Date.ToString(),
                                  e.Packet.Timeval.Date.Millisecond,
                                  ethernetPacket.SourceHwAddress,
                                  ethernetPacket.DestinationHwAddress);
                packetIndex++;
            }
        }
    }
}

