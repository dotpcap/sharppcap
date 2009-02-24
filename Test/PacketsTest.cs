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
            IPPacket.IPVersions[] versions = { IPPacket.IPVersions.IPv4, IPPacket.IPVersions.IPv6 };

            for(int i=0; i<10000; i++)
            {
                int len;
                //choose random version
                IPPacket.IPVersions ipver = versions[Rand.Instance.GetInt(0, 1)];
                //choose random len based on version
                if (ipver == IPPacket.IPVersions.IPv4)
                    len = Rand.Instance.GetInt(54, 1500);
                else
                    len = Rand.Instance.GetInt(74, 1500);

                TCPPacket tcp = TCPPacket.RandomPacket(len, ipver);
                //TODO: this test should use a known quantity, a packet recorded and loaded from a file
                Assert.AreEqual(len, tcp.Bytes.Length);
                Assert.IsTrue(tcp.ValidIPChecksum);
                Assert.IsTrue(tcp.ValidTCPChecksum);
            }
        }
    }

}
