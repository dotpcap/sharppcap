using System;
using Tamir.IPLib;
using Tamir.IPLib.Packets;

namespace Tamir.IPLib.Test.Example7
{
	/// <summary>
	/// Basic capture example
	/// </summary>
	public class DumpToFile
	{
		/// <summary>
		/// Basic capture example
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
			string ver = Tamir.IPLib.Version.GetVersionString();
			/* Print SharpPcap version */
			Console.WriteLine("SharpPcap {0}, Example7.DumpToFile.cs", ver);
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

			Console.WriteLine();
			Console.Write("-- Please choose a device to capture: ");
			i = int.Parse( Console.ReadLine() );
			Console.Write("-- Please enter the output file name: ");
			string capFile = Console.ReadLine();

			PcapDevice device = devices[i];

			//Register our handler function to the 'packet arrival' event
			device.PcapOnPacketArrival += 
				new SharpPcap.PacketArrivalEvent( device_PcapOnPacketArrival );

			//Open the device for capturing
			//true -- means promiscuous mode
			//1000 -- means a read wait of 1000ms
			device.PcapOpen(true, 1000);

			//Open or create a capture output file
			device.PcapDumpOpen( capFile );

			Console.WriteLine();
			Console.WriteLine
				("-- Listenning on {0}, hit 'Ctrl-C' to exit...",
				device.PcapDescription);

			//Start capture 'INFINTE' number of packets
			device.PcapCapture( SharpPcap.INFINITE );

			//Close the pcap device
			//(Note: these lines will never be called since
			// we're capturing infinite number of packets
			device.PcapDumpFlush();
			device.PcapDumpClose();
			device.PcapClose();
		}

		/// <summary>
		/// Dumps each received packet to a pcap file
		/// </summary>
		private static void device_PcapOnPacketArrival(object sender, Packet packet)
		{						
			PcapDevice device = (PcapDevice)sender;
			//if device has a dump file opened
			if( device.PcapDumpOpened )
			{
				//dump the packet to the file
				device.PcapDump( packet );
				Console.WriteLine("Packet dumped to file.");
			}
		}
	}
}
