using System;
using Tamir.IPLib;
using Tamir.IPLib.Protocols;
using Tamir.IPLib.Util;

/*
Copyright (c) 2006 Tamir Gal, http://www.tamirgal.com, All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

	1. Redistributions of source code must retain the above copyright notice,
		this list of conditions and the following disclaimer.

	2. Redistributions in binary form must reproduce the above copyright 
		notice, this list of conditions and the following disclaimer in 
		the documentation and/or other materials provided with the distribution.

	3. The names of the authors may not be used to endorse or promote products
		derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace Test
{
	/// <summary>
	/// A sample showing how to use the Address Resolution Protocol (ARP)
	/// with the SharpPcap library.
	/// </summary>
	public class ArpTest
	{

		public static void Main3(string[] args)
		{
			string ver = Tamir.IPLib.Version.GetVersionString();
			/* Print SharpPcap version */
			Console.WriteLine("SharpPcap {0}, ArpTest.cs", ver);
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
			Console.Write("-- Please choose a device for sending the ARP request: ");
			i = int.Parse( Console.ReadLine() );

			string device = devices[i].PcapName;

			String ip = "";

			while(true)
			{
				Console.Write("-- Please enter IP address to be resolved by ARP: ");
				ip = Console.ReadLine();
				if(IPUtil.IsIP(ip))
					break;
				Console.WriteLine("Bad IP address format, please try again");
			}

			//Create a new ARP resolver
			//(for more info, see:
			//http://www.tamirgal.com/home/SourceView.aspx?Item=SharpPcap&File=ARP.cs)
			ARP arper = new ARP(device);
			
			//print the resolved address
			Console.WriteLine(ip+" is at: "+arper.Resolve(ip));
		}
	}
}
