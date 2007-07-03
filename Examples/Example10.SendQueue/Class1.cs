using System;
using Tamir.IPLib;

namespace Example11.SendQueue
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main1(string[] args)
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

			Console.Write("-- This will send a random packet out this interface,"+
				"continue? [YES|no]");
			string resp = Console.ReadLine().ToLower();
			
			//If user refused, exit program
			if(resp=="n"||resp=="no")
				return;

			PcapSendQueue queue = new PcapSendQueue( 10000 );
			byte[] bytes;
			bytes=GetRandomPacket();
			queue.Add( GetRandomPacket(), 0, 0 );
			Console.WriteLine( queue.CurrentLength );
			queue.Add( GetRandomPacket(), 0, 10 );
			Console.WriteLine( queue.CurrentLength );
			queue.Add( GetRandomPacket(), 2, 0 );
			Console.WriteLine( queue.CurrentLength );
			queue.Add( GetRandomPacket(), 2, 1 );
			Console.WriteLine( queue.CurrentLength );

			//Open the device
			device.PcapOpen();
			Console.WriteLine( device.PcapSendQueue( queue, true ) );
			Console.WriteLine( device.PcapLastError );
			device.PcapClose();
			queue.Dispose();
            Console.WriteLine(" device closed");
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
