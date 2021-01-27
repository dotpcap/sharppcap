using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test
{
    [TestFixture]
    public class ArpTest
    {
        [Test]
        public void TestArp()
        {
            var d = TestHelper.GetPcapDevice();
            var arp = new ARP(d);

            // timeout should not be null
            Assert.IsNotNull(arp.Timeout);

            // and we can set a timeout
            arp.Timeout = new TimeSpan(0, 0, 2);

            var destinationIP = new System.Net.IPAddress(new byte[] { 8, 8, 8, 8 });

            // Note: We don't care about the success or failure here
            arp.Resolve(destinationIP);
        }
    }
}
