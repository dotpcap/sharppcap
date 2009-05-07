using System;
using NUnit.Framework;
using SharpPcap;

namespace Test
{
    [TestFixture]
    public class PcapTest
    {
        // Test that we can retrieve the Pcap version without any errors
        [Test]
        public void Version()
        {
            Console.WriteLine(Pcap.Version);
        }
    }
}
