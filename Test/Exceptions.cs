﻿using System;
using SharpPcap;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class Exceptions
    {
        [Test]
        public void ExceptionTest()
        {
            var deviceNotReadyException = new DeviceNotReadyException();
            var statistics = new StatisticsException("test description");
        }
    }
}
