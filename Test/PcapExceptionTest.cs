using SharpPcap;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class PcapExceptionTest
    {
        [Test]
        public void Exception()
        {
            var ex = new PcapException();
        }
    }
}
