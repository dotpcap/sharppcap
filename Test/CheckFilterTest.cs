using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpPcap;

namespace Test
{
    [TestFixture]
    public class CheckFilterTest
    {
        [Test]
        public void TestFilters()
        {
            // find a device
            List<PcapDevice> devices = Pcap.GetAllDevices();

            Assert.IsFalse(devices.Count == 0, "No devices found, cannot perform test. Try running as root");

            PcapDevice d = devices[0];
            d.Open();

            // test a known failing filter
            string errorString;
            Assert.IsFalse(d.CheckFilter("some bogus filter", out errorString));

            // test a known working filter
            Assert.IsTrue(d.CheckFilter("port 23", out errorString));
        }
    }
}
