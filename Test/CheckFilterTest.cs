using NUnit.Framework;
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
            Assert.IsFalse(LibPcapLiveDevice.CheckFilter("some bogus filter", out string errorString));

            // test a known working filter
            Assert.IsTrue(LibPcapLiveDevice.CheckFilter("port 23", out errorString));
        }
    }
}
