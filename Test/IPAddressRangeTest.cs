using System;
using NUnit.Framework;

namespace Tamir.IPLib.Util
{
	[TestFixture]
	public class IPAddressRangeTest
	{
		internal IPAddressRange ipRange;
				
		[TestFixtureSetUp]
		public virtual void  SetUp()
		{
			ipRange = new IPAddressRange();
			//ipRange.setRandom(false);
		}
		
		[Test]
		public virtual void  testGeneral()
		{
			int count = 200;
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			foreach(String ip in ipRange)
			{
				//UPGRADE_TODO: Method 'java.io.PrintStream.println' was converted to 'System.Console.Out.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintStreamprintln_javalangObject'"
				System.Console.Out.WriteLine(ip);
				if (--count == 0)
					break;
			}
		}
		
		[Test]
		public virtual void  testSubnet()
		{
			try
			{
				IPSubnet subnet = new IPSubnet("22.33.44.0/16");
				int count = 200;
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				foreach(String ip in subnet)
				{
					//UPGRADE_TODO: Method 'java.io.PrintStream.println' was converted to 'System.Console.Out.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintStreamprintln_javalangObject'"
					System.Console.Out.WriteLine(ip);
					if (--count == 0)
						break;
				}
			}
			catch (System.Exception e)
			{
				// TODO Auto-generated catch block
				SupportClass.WriteStackTrace(e, Console.Error);
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				Assert.Fail(e.ToString());
			}
		}

		[Test]
		public virtual void  testSubnet2()
		{
			try
			{
				IPSubnet subnet = new IPSubnet("10.10.10.0 255.255.255.0");
				int count = 200;
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				foreach(String ip in subnet)
				{
					//UPGRADE_TODO: Method 'java.io.PrintStream.println' was converted to 'System.Console.Out.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintStreamprintln_javalangObject'"
					System.Console.Out.WriteLine(ip);
					if (--count == 0)
						break;
				}
			}
			catch (System.Exception e)
			{
				// TODO Auto-generated catch block
				SupportClass.WriteStackTrace(e, Console.Error);
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				Assert.Fail(e.ToString());
			}
		}
	}
}