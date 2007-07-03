using System;
using Tamir.IPLib;
using Tamir.IPLib.Packets;

namespace Tamir.IPLib.Test.Example8
{
	/// <summary>
	/// Basic capture example
	/// </summary>
	public class DumpTCP
	{
		/// <summary>
		/// Basic capture example
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
			string ver = Tamir.IPLib.Version.GetVersionString();
			/* Print SharpPcap version */
			Console.WriteLine("SharpPcap {0}, Example8.ReadFile.cs", ver);
			Console.WriteLine();

			Console.WriteLine();
			Console.Write("-- Please enter an input capture file name: ");
			string capFile = Console.ReadLine();

			PcapDevice device;
			
			try
			{
				//Get an offline file pcap device
				device = SharpPcap.GetPcapOfflineDevice( capFile );
				//Open the device for capturing
				device.PcapOpen();
			} 
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return;
			}

			//Register our handler function to the 'packet arrival' event
			device.PcapOnPacketArrival += 
				new SharpPcap.PacketArrivalEvent( device_PcapOnPacketArrival );

			Console.WriteLine();
			Console.WriteLine
				("-- Capturing from '{0}', hit 'Ctrl-C' to exit...",
				capFile);

			//Start capture 'INFINTE' number of packets
			//This method will return when EOF reached.
			device.PcapCapture( SharpPcap.INFINITE );

			//Close the pcap device
			device.PcapClose();
			Console.WriteLine("-- End of file reached.");
			Console.Write("Hit 'Enter' to exit...");
			Console.ReadLine();
		}

		/// <summary>
		/// Prints the source and dest MAC addresses of each received Ethernet frame
		/// </summary>
		private static void device_PcapOnPacketArrival(object sender, Packet packet)
		{		
			if( packet is EthernetPacket )
			{
				EthernetPacket etherFrame = (EthernetPacket)packet;
				Console.WriteLine("At: {0}:{1}: MAC:{2} -> MAC:{3}",
					etherFrame.PcapHeader.Date.ToString(),
					etherFrame.PcapHeader.Date.Millisecond,
					etherFrame.SourceHwAddress, 
					etherFrame.DestinationHwAddress);
			}
		}
	}
}
