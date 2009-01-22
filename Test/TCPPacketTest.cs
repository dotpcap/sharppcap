using System;
using NUnit.Framework;

namespace SharpPcap.Util
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
				Console.Error.WriteLine(e.StackTrace);				
				Assert.Fail(e.ToString());
			}
		}
	}
}