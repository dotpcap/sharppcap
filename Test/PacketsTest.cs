using System;
using Tamir.IPLib.Util;
using Tamir.IPLib.Packets;
using NUnit.Framework;

namespace Test
{
	[TestFixture]
	public class PacketsTest
	{
		[Test]
		public void TestChecksums()
		{
			for(int i=0; i<10000; i++)
			{
				TCPPacket tcp = TCPPacket.RandomPacket();
				Assert.IsTrue(tcp.ValidIPChecksum);
				Assert.IsTrue(tcp.ValidTCPChecksum);
			}
		}
	}

}
