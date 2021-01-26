using System;
using SharpPcap;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class Exceptions
    {
        public Exceptions()
        {
            var deviceNotReadyException = new DeviceNotReadyException();
            var statistics = new StatisticsException("test description");
            var npcapRequiredException = new NpcapRequiredException("test message");
        }
    }
}
