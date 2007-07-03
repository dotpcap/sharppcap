using System;
using System.Collections;
using Tamir.IPLib.Util;

namespace Test
{
	/// <summary>
	/// Summary description for IPSubnetTest.
	/// </summary>
	public class IPSubnetTest
	{
		public static void Test()
		{
			IPSubnet ipRange = new IPSubnet("10.10.10.0", 24);
			Console.WriteLine(ipRange.NetworkAddress);
			Console.WriteLine(ipRange.BroadcastAddress);
			ipRange.setRandom(false);
			int count = (int)ipRange.size();
			
			ArrayList list = new ArrayList();

			
			foreach(String ip in ipRange)
			{
				//System.Console.Out.WriteLine(ip);
				list.Add(ip);
				if (--count == 0)
					break;
			}

			IPAddressList ipList = new IPAddressList(list);
			ipList.setRandom(false);
			count = (int)ipList.size();
			foreach(String ip in ipList)
			{
				System.Console.Out.WriteLine(ip);
				if (--count == 0)
					break;
			}
		}
	}
}
