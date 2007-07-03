using System;
using Tamir.IPLib;

namespace Tamir.IPLib.Test.Example5
{
	/// <summary>
	/// Obtaining the device list
	/// </summary>
	public class PcapFilter
	{
		/// <summary>
		/// Obtaining the device list
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
			string ver = Tamir.IPLib.Version.GetVersionString();
			/* Print SharpPcap version */
			Console.WriteLine("SharpPcap {0}, Example4.IfListAdv.cs", ver);

			/* Retrieve the device list */
			PcapDeviceList devices = SharpPcap.GetAllDevices();

			if(devices.Count<1)
			{
				Console.WriteLine("No device found on this machine");
				return;
			}
			
			Console.WriteLine();
			Console.WriteLine("The following devices are available on this machine:");
			Console.WriteLine("----------------------------------------------------");
			Console.WriteLine();

			int i = 0;

			/* Scan the list printing every entry */
			foreach(PcapDevice dev in devices)
			{
				/* Description */
				Console.WriteLine("{0}) {1}",i,dev.PcapDescription);
				Console.WriteLine();
				/* Name */
				Console.WriteLine("\tName:\t\t{0}",dev.PcapName);
				/* Is Loopback */
				Console.WriteLine("\tLoopback:\t\t{0}",dev.PcapLoopback);

				/* 
					If the device is a physical network device,
					lets print some advanced info
				 */
				if(dev is NetworkDevice)
				{//Then..

					/* Cast to NetworkDevice */
					NetworkDevice netDev = (NetworkDevice)dev;
					/* Print advanced info */
					Console.WriteLine("\tIP Address:\t\t{0}",netDev.IpAddress);
					Console.WriteLine("\tSubnet Mask:\t\t{0}",netDev.SubnetMask);
					Console.WriteLine("\tMAC Address:\t\t{0}",netDev.MacAddress);
					Console.WriteLine("\tDefault Gateway:\t{0}",netDev.DefaultGateway);
					Console.WriteLine("\tPrimary WINS:\t\t{0}",netDev.WinsServerPrimary);
					Console.WriteLine("\tSecondary WINS:\t\t{0}",netDev.WinsServerSecondary);
					Console.WriteLine("\tDHCP Enabled:\t\t{0}",netDev.DhcpEnabled);
					Console.WriteLine("\tDHCP Server:\t\t{0}",netDev.DhcpServer);
					Console.WriteLine("\tDHCP Lease Obtained:\t{0}",netDev.DhcpLeaseObtained);
					Console.WriteLine("\tDHCP Lease Expires:\t{0}",netDev.DhcpLeaseExpires);
					Console.WriteLine("\tAdmin Status:\t{0}",netDev.AdminStatus);
					Console.WriteLine("\tMedia State:\t{0}",netDev.MediaState);
				}
				Console.WriteLine();
				i++;
			}
			Console.Write("Hit 'Enter' to exit...");
			Console.ReadLine();
		}
	}
}
