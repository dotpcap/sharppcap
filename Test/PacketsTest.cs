using System;
using SharpPcap.Util;
using SharpPcap.Packets;
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
//				TCPPacket tcp = TCPPacket.RandomPacket();
                //TODO: TCPPacket to be fixed after the ipv4/ipv6 changes as ValidIPChecksum doesn't work now
                throw new System.NotImplementedException();
//				Assert.IsTrue(tcp.ValidIPChecksum);
//				Assert.IsTrue(tcp.ValidTCPChecksum);
			}
		}
	}

}
