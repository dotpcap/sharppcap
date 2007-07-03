using System;
using Tamir.IPLib;
using Tamir.IPLib.Packets;

namespace Tamir.IPLib.Test.Example10
{
	/// <summary>
	/// Basic capture example with no callback
	/// </summary>
	public class SendQueues
	{
		/// <summary>
		/// Basic capture example
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
			string ver = Tamir.IPLib.Version.GetVersionString();
			/* Print SharpPcap version */
			Console.WriteLine("SharpPcap {0}, Example10.SendQueues.cs", ver);

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

			Console.Write("Queueing packets...");

			//Allocate a new send queue
			PcapSendQueue squeue = new PcapSendQueue
				( (int)((PcapOfflineDevice)device).PcapFileSize );
			Packet packet;
			
			try
			{
				//Go through all packets in the file and add to the queue
				while( (packet=device.PcapGetNextPacket()) != null )
				{
					if( !squeue.Add( packet ) )
					{
						Console.WriteLine("Warning: packet buffer too small, "+
							"not all the packets will be sent.");
						break;
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return;
			}
			
			Console.WriteLine("OK");
			
			Console.WriteLine();
			Console.WriteLine("The following devices are available on this machine:");
			Console.WriteLine("----------------------------------------------------");
			Console.WriteLine();

			int i=0;

			PcapDeviceList devices = SharpPcap.GetAllDevices();
			/* Scan the list printing every entry */
			foreach(PcapDevice dev in devices)
			{
				/* Description */
				Console.WriteLine("{0}) {1}",i,dev.PcapDescription);
				i++;
			}

			Console.WriteLine();
			Console.Write("-- Please choose a device to transmit on: ");
			i = int.Parse( Console.ReadLine() );
			devices[i].PcapOpen();
			string resp;

			if(devices[i].PcapDataLink != device.PcapDataLink)
			{
				Console.Write("Warning: the datalink of the capture"+
					" differs from the one of the selected interface, continue? [YES|no]");
				resp = Console.ReadLine().ToLower();

				if((resp!="")&&( !resp.StartsWith("y")))
				{
					Console.WriteLine("Cancelled by user!");
					devices[i].PcapClose();
					return;
				}
			}
			device.PcapClose();
			device = devices[i];

			Console.Write("This will transmit all queued packets through"+
				" this device, continue? [YES|no]");
			resp = Console.ReadLine().ToLower();

			if((resp!="")&&( !resp.StartsWith("y")))
			{
				Console.WriteLine("Cancelled by user!");
				return;
			}

			try
			{
				Console.Write("Sending packets...");
				int sent = device.PcapSendQueue( squeue, true );
				Console.WriteLine("Done!");
				if( sent < squeue.CurrentLength )
				{
					Console.WriteLine("An error occurred sending the packets: {0}. "+
						"Only {1} bytes were sent\n", device.PcapLastError, sent);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine( "Error: "+e.Message );
			}
			//Free the queue
			squeue.Dispose();
			Console.WriteLine("-- Queue is disposed.");
			//Close the pcap device
			device.PcapClose();
			Console.WriteLine("-- Device closed.");
			Console.Write("Hit 'Enter' to exit...");
			Console.ReadLine();
		}
	}
}
