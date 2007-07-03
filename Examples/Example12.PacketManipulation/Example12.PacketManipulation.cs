using System;
using Tamir.IPLib;
using Tamir.IPLib.Packets;

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
		[STAThread]
		static void Main(string[] args)
		{
			string ver = Tamir.IPLib.Version.GetVersionString();
			/* Print SharpPcap version */
			Console.WriteLine("SharpPcap {0}, Example12.PacketManipulation.cs", ver);
			Console.WriteLine();

			/* Retrieve the device list */
			PcapDeviceList devices = SharpPcap.GetAllDevices();

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
				Console.WriteLine("{0}) {1}",i,dev.PcapDescription);
				i++;
			}
			Console.WriteLine("{0}) {1}",i,"Read packets from offline pcap file");

			Console.WriteLine();
			Console.Write("-- Please choose a device to capture: ");
			int choice = int.Parse( Console.ReadLine() );
			
			PcapDevice device =null;
			if(choice==i)
			{
				Console.Write("-- Please enter an input capture file name: ");
				string capFile = Console.ReadLine();
				device = SharpPcap.GetPcapOfflineDevice(capFile);
			}
			else
			{
				device = devices[choice];
			}
			

			//Register our handler function to the 'packet arrival' event
			device.PcapOnPacketArrival += 
				new Tamir.IPLib.SharpPcap.PacketArrivalEvent(device_PcapOnPacketArrival);

			//Open the device for capturing
			//true -- means promiscuous mode
			//1000 -- means a read wait of 1000ms
			device.PcapOpen(true, 1000);

			Console.WriteLine();
			Console.WriteLine
				("-- Listenning on {0}, hit 'Ctrl-C' to exit...",
				device.PcapDescription);

			//Start capture 'INFINTE' number of packets
			device.PcapCapture( SharpPcap.INFINITE );

			//Close the pcap device
			//(Note: this line will never be called since
			// we're capturing infinite number of packets
			device.PcapClose();
		}

		private static void device_PcapOnPacketArrival(object sender, Tamir.IPLib.Packets.Packet packet)
		{
			if(packet is EthernetPacket)
			{				
				EthernetPacket eth = ((EthernetPacket)packet);
				Console.WriteLine("Original packet: "+eth.ToColoredString(false));

				//Manipulate ethernet parameters
				eth.SourceHwAddress = "00:11:22:33:44:55";
				eth.DestinationHwAddress = "00:99:88:77:66:55";

				if (packet is IPPacket)
				{
					IPPacket ip = ((IPPacket)packet);
					
					//manipulate IP parameters
					ip.SourceAddress = "1.2.3.4";
					ip.DestinationAddress = "44.33.22.11";
					ip.TimeToLive = 11;

					//Recalculate the IP checksum
					ip.ComputeIPChecksum();

					if (ip is TCPPacket)
					{
						TCPPacket tcp = ((TCPPacket)ip);

						//manipulate TCP parameters
						tcp.SourcePort = 9999;
						tcp.DestinationPort = 8888;
						tcp.Syn = !tcp.Syn;
						tcp.Fin = !tcp.Fin;
						tcp.Ack = !tcp.Ack;
						tcp.WindowSize = 500;
						tcp.AcknowledgementNumber = 800;
						tcp.SequenceNumber = 800;

						//Recalculate the TCP checksum
						tcp.ComputeTCPChecksum();
					}

					if (ip is UDPPacket)
					{
						UDPPacket udp = ((UDPPacket)ip);

						//manipulate UDP parameters
						udp.SourcePort = 9999;
						udp.DestinationPort = 8888;

						//Recalculate the UDP checksum
						udp.ComputeUDPChecksum();
					}
				}
				Console.WriteLine("Manipulated packet: "+eth.ToColoredString(false));
			}
		}
	}
}
