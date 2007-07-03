using System;
using Tamir.IPLib;

namespace Tamir.IPLib.Test.Example1
{
	/// <summary>
	/// Obtaining the device list
	/// </summary>
	public class IfListAdv
	{
		/// <summary>
		/// Obtaining the device list
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
			string ver = Tamir.IPLib.Version.GetVersionString();
			/* Print SharpPcap version */
			Console.WriteLine("SharpPcap {0}, Example4.IfList.cs", ver);

			/* Retrieve the device list */
			PcapDeviceList devices = SharpPcap.GetAllDevices();

			/*If no device exists, print error */
			if(devices.Count<1)
			{
				Console.WriteLine("No device found on this machine");
				return;
			}
			
			Console.WriteLine();
			Console.WriteLine("The following devices are available on this machine:");
			Console.WriteLine("----------------------------------------------------");
			Console.WriteLine();

			int i=0;

			/* Scan the list printing every entry */
			foreach(PcapDevice dev in devices)
			{
				/* Description */
				Console.WriteLine("{0}) {1}",i,dev.PcapDescription);
				Console.WriteLine();
				/* Name */
				Console.WriteLine("\tName:\t{0}",dev.PcapName);
				/* IP Address */
				Console.WriteLine("\tIP Address: \t\t{0}",dev.PcapIpAddress);
				/* Is Loopback */
				Console.WriteLine("\tLoopback: \t\t{0}",dev.PcapLoopback);

				Console.WriteLine();
				i++;
			}
			Console.Write("Hit 'Enter' to exit...");
			Console.ReadLine();
		}
	}
}
