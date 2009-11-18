using System;
using NUnit.Framework;
using SharpPcap.Util;

namespace Test
{
    [TestFixture]
    public class ChecksumUtils
    {
        [Test]
        public void OnesSum()
        {
            var bytes = new Byte[4096];
            for(int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 0x7f;
            }

            var result = SharpPcap.ChecksumUtils.OnesSum(bytes);
            Console.WriteLine("result: {0}", result);
        }
    }
}
