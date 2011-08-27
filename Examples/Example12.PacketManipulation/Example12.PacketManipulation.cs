using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Example12.PacketManipulation
{
    /// <summary>
    /// Example showing packet manipulation
    /// </summary>
    class PacketManipulation
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example12.PacketManipulation.cs", ver);
            Console.WriteLine();

            // Retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error
            if(devices.Count<1)
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
                Console.WriteLine("{0}) {1}", i, dev.Description);
                i++;
            }
            Console.WriteLine("{0}) {1}", i, "Read packets from offline pcap file");

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            var choice = int.Parse( Console.ReadLine() );

            ICaptureDevice device = null;
            if(choice==i)
            {
                Console.Write("-- Please enter an input capture file name: ");
                string capFile = Console.ReadLine();
                device = new CaptureFileReaderDevice(capFile);
            }
            else
            {
                device = devices[choice];
            }

            //Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += 
                new PacketArrivalEventHandler(device_OnPacketArrival);

            // Open the device for capturing
            device.Open();

            Console.WriteLine();
            Console.WriteLine
                ("-- Listening on {0}, hit 'Ctrl-C' to exit...",
                device.Description);

            // Start capture 'INFINTE' number of packets
            device.Capture();

            // Close the pcap device
            // (Note: this line will never be called since
            //  we're capturing infinite number of packets
            device.Close();
        }

        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            if(packet is PacketDotNet.EthernetPacket)
            {
                var eth = ((PacketDotNet.EthernetPacket)packet);
                Console.WriteLine("Original Eth packet: " + eth.ToString());

                //Manipulate ethernet parameters
                eth.SourceHwAddress = PhysicalAddress.Parse("00-11-22-33-44-55");
                eth.DestinationHwAddress = PhysicalAddress.Parse("00-99-88-77-66-55");

                var ip = PacketDotNet.IpPacket.GetEncapsulated(packet);
                if(ip != null)
                {
                    Console.WriteLine("Original IP packet: " + ip.ToString());

                    //manipulate IP parameters
                    ip.SourceAddress = System.Net.IPAddress.Parse("1.2.3.4");
                    ip.DestinationAddress = System.Net.IPAddress.Parse("44.33.22.11");
                    ip.TimeToLive = 11;

                    var tcp = PacketDotNet.TcpPacket.GetEncapsulated(packet);
                    if (tcp != null)
                    {
                        Console.WriteLine("Original TCP packet: " + tcp.ToString());

                        //manipulate TCP parameters
                        tcp.SourcePort = 9999;
                        tcp.DestinationPort = 8888;
                        tcp.Syn = !tcp.Syn;
                        tcp.Fin = !tcp.Fin;
                        tcp.Ack = !tcp.Ack;
                        tcp.WindowSize = 500;
                        tcp.AcknowledgmentNumber = 800;
                        tcp.SequenceNumber = 800;
                    }

                    var udp = PacketDotNet.UdpPacket.GetEncapsulated(packet);
                    if (udp != null)
                    {
                        Console.WriteLine("Original UDP packet: " + udp.ToString());

                        //manipulate UDP parameters
                        udp.SourcePort = 9999;
                        udp.DestinationPort = 8888;
                    }
                }

                Console.WriteLine("Manipulated Eth packet: " + eth.ToString());
            }
        }
    }
}
