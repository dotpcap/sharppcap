using System;
using NUnit.Framework;

namespace Tamir.IPLib.Util
{
	[TestFixture]
	public class TCPPacketTest
	{
				
		[TestFixtureSetUp]
		public virtual void  SetUp()
		{
		}
		
		[Test]
		public virtual void  testSubnet()
		{
			try
			{

			}
			catch (System.Exception e)
			{
				SupportClass.WriteStackTrace(e, Console.Error);				
				Assert.Fail(e.ToString());
			}
		}
	}
}