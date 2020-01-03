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
    public class SendQueueTest
    {
        private const string Filter = "ether proto 0x1234";
        private const int PacketCount = 8;
        private static readonly long DeltaTicks = FromMilliseconds(1).Ticks;

        [Test]
        public void TestNativeTransmitNormal()
        {
            var received = RunCapture(Filter, (device) =>
            {
                GetSendQueue().NativeTransmit(device, false);
            });
            AssertGoodTransmitNormal(received);
        }

        [Test]
        public void TestNativeTransmitSync()
        {
            var received = RunCapture(Filter, (device) =>
            {
                GetSendQueue().NativeTransmit(device, true);
            });
            AssertGoodTransmitSync(received);
        }

        [Test]
        public void TestManagedTransmitNormal()
        {
            var received = RunCapture(Filter, (device) =>
            {
                GetSendQueue().ManagedTransmit(device, false);
            });
            AssertGoodTransmitNormal(received);
        }

        [Test]
        public void TestManagedTransmitSync()
        {
            var received = RunCapture(Filter, (device) =>
            {
                GetSendQueue().ManagedTransmit(device, true);
            });
            AssertGoodTransmitSync(received);
        }

        private static void AssertGoodTransmitNormal(List<RawCapture> received)
        {
            Assert.That(received, Has.Count.EqualTo(PacketCount));
            var times = received.Select(r => r.Timeval.Date);
            Assert.That(times.Max() - times.Min(), Is.LessThan(FromMilliseconds(1)));
        }

        private static void AssertGoodTransmitSync(List<RawCapture> received)
        {
            Assert.That(received, Has.Count.EqualTo(PacketCount));
            var times = received.Select(r => r.Timeval.Date).ToArray();
            for (int i = 1; i < PacketCount; i++)
            {
                var delta = (times[i] - times[i - 1]).Ticks;
                Assert.That(delta, Is.LessThan(DeltaTicks * 1.1));
                Assert.That(delta, Is.GreaterThan(DeltaTicks * 0.9));
            }
        }

        [Test]
        public void TestReturnValue()
        {
            var device = GetPcapDevice();
            var queue = GetSendQueue();
            var managed = queue.ManagedTransmit(device, false);
            var native = queue.NativeTransmit(device, false);
            Assert.AreEqual(managed, native);
        }

        [Test]
        public void TestIsHardwareAccelerated()
        {
            Assert.AreEqual(
                SendQueue.IsHardwareAccelerated,
                Environment.OSVersion.Platform == PlatformID.Win32NT
            );
        }

        /// <summary>
        /// Helper method
        /// </summary>
        /// <returns></returns>
        private static SendQueueWrapper GetSendQueue()
        {
            var queue = new SendQueueWrapper(1024);
            var packet = EthernetPacket.RandomPacket();
            packet.Type = (EthernetType)0x1234;
            for (var i = 0; i < PacketCount; i++)
            {
                var time = i * DeltaTicks * 1e6 / TicksPerSecond;
                Assert.IsTrue(queue.Add(packet.Bytes, 123456, (int)time));
            }
            return queue;
        }

        class SendQueueWrapper : SendQueue
        {
            public SendQueueWrapper(int memSize) : base(memSize)
            {
            }

            internal new int NativeTransmit(PcapDevice device, bool synchronized)
            {
                return base.NativeTransmit(device, synchronized);
            }

            internal new int ManagedTransmit(PcapDevice device, bool synchronized)
            {
                return base.ManagedTransmit(device, synchronized);
            }
        }
    }


}
