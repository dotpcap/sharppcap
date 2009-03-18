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
            // test a known failing filter
            string errorString;
            Assert.IsFalse(PcapDevice.CheckFilter("some bogus filter", out errorString));

            // test a known working filter
            Assert.IsTrue(PcapDevice.CheckFilter("port 23", out errorString));
        }
    }
}
