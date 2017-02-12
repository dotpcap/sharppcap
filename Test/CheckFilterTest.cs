using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;

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
            Assert.IsFalse(LibPcapLiveDevice.CheckFilter("some bogus filter", out errorString));

            // test a known working filter
            Assert.IsTrue(LibPcapLiveDevice.CheckFilter("port 23", out errorString));
        }
    }
}
