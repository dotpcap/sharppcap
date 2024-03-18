// SPDX-License-Identifier: MIT

using System;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace ReadingCaptureFile
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ver = Pcap.SharpPcapVersion;

            /* Print SharpPcap version */
            Console.WriteLine("SharpPcap {0}, ReadingCaptureFile", ver);
            Console.WriteLine();

            Console.WriteLine();

            // read the file from stdin or from the command line arguments
            string capFile;
            if (args.Length == 0)
            {
                Console.Write("-- Please enter an input capture file name: ");
                capFile = Console.ReadLine();
            }
            else
            {
                // use the first argument as the filename
                capFile = args[0];
            }

            Console.WriteLine("opening '{0}'", capFile);

            ICaptureDevice device;

            try
            {
                // Get an offline device
                device = new CaptureFileReaderDevice(capFile);

                // Open the device
                device.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception when opening file" + e.ToString());
                return;
            }

            // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival +=
                new PacketArrivalEventHandler(device_OnPacketArrival);

            Console.WriteLine();
            Console.WriteLine
                ("-- Capturing from '{0}', hit 'Ctrl-C' to exit...",
                capFile);

            var startTime = DateTime.Now;

            // Start capture 'INFINTE' number of packets
            // This method will return when EOF reached.
            device.Capture();

            // Close the pcap device
            device.Close();
            var endTime = DateTime.Now;
            Console.WriteLine("-- End of file reached.");

            var duration = endTime - startTime;
            Console.WriteLine("Read {0} packets in {1}s", packetIndex, duration.TotalSeconds);

            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }

        private static int packetIndex = 0;

        /// <summary>
        /// Prints the source and dest MAC addresses of each received Ethernet frame
        /// </summary>
        private static void device_OnPacketArrival(object sender, PacketCapture e)
        {
            packetIndex++;

            var rawPacket = e.GetPacket();
            var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

            var ethernetPacket = packet.Extract<EthernetPacket>();
            if (ethernetPacket != null)
            {
                Console.WriteLine("{0} At: {1}:{2}: MAC:{3} -> MAC:{4}",
                                  packetIndex,
                                  e.Header.Timeval.Date.ToString(),
                                  e.Header.Timeval.Date.Millisecond,
                                  ethernetPacket.SourceHardwareAddress,
                                  ethernetPacket.DestinationHardwareAddress);
            }
        }
    }
}

