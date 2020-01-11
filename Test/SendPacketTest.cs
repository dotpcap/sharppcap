using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using static Test.TestHelper;
using static System.TimeSpan;

namespace Test
{
    [TestFixture]
    [NonParallelizable]
    public class SendPacketTest
    {
        private const string Filter = "ether proto 0x1234";

        [Test]
        [Ignore("Not sure why test fails")]
        public void TestSendPacketTest()
        {
            var packet = EthernetPacket.RandomPacket();
            packet.Type = (EthernetType)0x1234;
            var received = RunCapture(Filter, (device) =>
            {
                device.SendPacket(packet);
                // Test hack: MacOS 10.15, delay of 1000ms causes this test to pass on cmorgan's 2019 macbook pro,
                // 500ms does not
                System.Threading.Thread.Sleep(1000);
            });
            Assert.That(received, Has.Count.EqualTo(1));
            CollectionAssert.AreEquivalent(packet.Bytes, received[0].Data);
        }

        [SetUp]
        public void SetUp()
        {
            TestHelper.ConfirmIdleState();
        }

        [TearDown]
        public void Cleanup()
        {
            TestHelper.ConfirmIdleState();
        }
    }
}
