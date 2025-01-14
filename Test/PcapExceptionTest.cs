// Copyright 2021 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

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
