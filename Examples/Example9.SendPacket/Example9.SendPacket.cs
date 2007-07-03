using System;
using Tamir.IPLib;
using Tamir.IPLib.Packets;

namespace Tamir.IPLib.Test.Example9
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
			Console.WriteLine("SharpPcap {0}, Example9.SendPacket.cs", ver);
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
			Console.Write("-- Please choose a device to send a packet on: ");
			i = int.Parse( Console.ReadLine() );

			PcapDevice device = devices[i];

			Console.Write("-- This will send a random packet out this interface, "+
				"continue? [YES|no]");
			string resp = Console.ReadLine().ToLower();
			
			//If user refused, exit program
			if((resp!="")&&( !resp.StartsWith("y")))
			{
				Console.WriteLine("Cancelled by user!");
				return;
			}

			//Open the device
			device.PcapOpen();
			
			//Generate a random packet
			byte[] bytes = GetRandomPacket();

			try
			{
				//Send the packet out the network device
				device.PcapSendPacket( bytes );
				Console.WriteLine("-- Packet sent successfuly.");
			}
			catch(Exception e)
			{
				Console.WriteLine("-- "+ e.Message );
			}

			//Close the pcap device
			device.PcapClose();
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
