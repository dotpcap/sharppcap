using System;
using NUnit.Framework;
using Tamir.IPLib;

namespace Test
{
	[TestFixture]
	public class IPHelperTest
	{
		[Test]
		public void AdminStatusTest()
		{
			NetworkDeviceList devs = IPHelper.GetAllDevices();
			NetworkDevice myDev = null;
			foreach(NetworkDevice dev in devs)
			{
				Console.WriteLine(dev.ToStringDetailed());
				if(dev.Name.EndsWith("{2E39C390-6D40-412E-ADF4-8270E3782F74}"))
				{
					myDev=dev;
				}
			}
			
			if(myDev!=null)
			{
				Console.WriteLine("Bringing down '{0}'...", myDev.Description);
				myDev.AdminStatus = false;
				Assert.IsTrue(!myDev.AdminStatus, "Admin status should be: False");
				//System.Threading.Thread.Sleep(10000);
				Console.WriteLine("Bringing up '{0}'...", myDev.Description);
				myDev.AdminStatus = true;
				Assert.IsTrue(myDev.AdminStatus, "Admin status should be: True");
			}
		}
	}
}
