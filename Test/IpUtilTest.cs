using System;
using NUnit.Framework;
using Tamir.IPLib.Util;


namespace Test
{
	[TestFixture]
	public class IpUtilTest
	{
		[TestFixtureSetUp]
		public void Setup()
		{
		}

		[Test]
		public void TestIp()
		{
			String ipaddr = "192.168.55.80";
			long longIP = IPUtil.IpToLong(ipaddr);
			Assert.AreEqual(ipaddr, IPUtil.IpToString(longIP));
		}

		[Test]
		public void testExtractIp() 
		{
			Assert.AreEqual("1.2.3.0", IPUtil.ExtractIp("1.2.3.0/24"));
			Assert.AreEqual("1.2.3.0", IPUtil.ExtractIp("1.2.3.0 255.255.255.0"));
		}

		[Test]
		public void testExtractMaskBits() 
		{
			Assert.AreEqual(24, IPUtil.ExtractMaskBits("1.2.3.0/24"));
			Assert.AreEqual(24, IPUtil.ExtractMaskBits("1.2.3.0 255.255.255.0"));
		}

		[Test]
		public void testIsRange()
		{
			Assert.IsTrue(IPUtil.IsRange("1.2.3.0/24"));
			Assert.IsTrue(IPUtil.IsRange("1.2.3.0 255.255.255.0"));
			Assert.IsTrue(!IPUtil.IsRange("1.2.3.0"));
			Assert.IsTrue(!IPUtil.IsRange("1.2.3.0/"));
			Assert.IsTrue(!IPUtil.IsRange("1.2.3.0 255.255.0"));
		}

		[Test]
		public void testIsRangeWithMaskBits()
		{
			Assert.IsTrue(IPUtil.IsRangeWithMaskBits("1.2.3.0/24"));
			Assert.IsTrue(!IPUtil.IsRangeWithMaskBits("1.2.3.0 255.255.255.0"));
			Assert.IsTrue(!IPUtil.IsRangeWithMaskBits("1.2.3.0"));
			Assert.IsTrue(!IPUtil.IsRangeWithMaskBits("1.2.3.0/"));
			Assert.IsTrue(!IPUtil.IsRangeWithMaskBits("1.2.3.0 255.255.0"));
		}

		[Test]
		public void testIsRangeWithDottedMask()
		{
			Assert.IsTrue(!IPUtil.IsRangeWithDottedMask("1.2.3.0/24"));
			Assert.IsTrue(IPUtil.IsRangeWithDottedMask("1.2.3.0 255.255.255.0"));
			Assert.IsTrue(!IPUtil.IsRangeWithDottedMask("1.2.3.0"));
			Assert.IsTrue(!IPUtil.IsRangeWithDottedMask("1.2.3.0/"));
			Assert.IsTrue(!IPUtil.IsRangeWithDottedMask("1.2.3.0 255.255.0"));
		}
	}
}
