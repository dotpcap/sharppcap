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
    [Category("SendPacket")]
    public class SendQueueTest
    {
        private const string Filter = "ether proto 0x1234";
        private const int PacketCount = 80;
        // Windows is usually able to simulate inter packet gaps down to 20µs
        // We test with 100µs to avoid flaky tests
        internal static readonly decimal DeltaTime = 100E-6M;

        /// <summary>
        /// Transmit with normal works correctly
        /// </summary>
        [Test]
        public void TestTransmitNormalSyncFalse()
        {
            if (SendQueue.IsHardwareAccelerated)
            {
                var received = RunCapture(Filter, (device) =>
                {
                    GetSendQueue().Transmit(device, false);
                });
                AssertGoodTransmitNormal(received);
            }
            else
            {
                Assert.Ignore("Skipping test as no hardware acceleration is present");
            }
        }

        /// <summary>
        /// Transmit with Normal works as expected
        /// </summary>
        [Test]
        public void TestTransmitNormal()
        {
            if (SendQueue.IsHardwareAccelerated)
            {
                var received = RunCapture(Filter, (device) =>
                {
                    GetSendQueue().Transmit(device, SendQueueTransmitModes.Normal);
                });
                AssertGoodTransmitNormal(received);
            }
            else
            {
                Assert.Ignore("Skipping test as no hardware acceleration is present");
            }
        }

        /// <summary>
        /// Transmit with an empty queue returns zero
        /// </summary>
        [Test]
        public void TestTransmitEmpty()
        {
            using var device = GetPcapDevice();
            device.Open();
            var queue = new SendQueueWrapper(1024);
            Assert.AreEqual(0, queue.Transmit(device, SendQueueTransmitModes.Normal));
        }

        [Test]
        public void TestAddByteArray()
        {
            var queue = new SendQueueWrapper(1024);
            var bytes = new Byte[] { 0x1, 0x2, 0x3 };
            queue.Add(bytes);
            Assert.AreEqual(PcapHeader.MemorySize + bytes.Length, queue.CurrentLength);
        }

        [Test]
        public void TestAddPacket()
        {
            var queue = new SendQueueWrapper(1024);
            var packet = EthernetPacket.RandomPacket();
            queue.Add(packet);
            Assert.AreEqual(PcapHeader.MemorySize + packet.TotalPacketLength, queue.CurrentLength);
        }

        [Test]
        public void TestAddRawCapture()
        {
            var queue = new SendQueueWrapper(1024);
            var bytes = new Byte[] { 0x1, 0x2, 0x3 };
            var rawCapture = new RawCapture(LinkLayers.Ethernet, new PosixTimeval(10, 20), bytes);
            queue.Add(rawCapture);
            Assert.AreEqual(PcapHeader.MemorySize + rawCapture.PacketLength, queue.CurrentLength);
        }

        [Test]
        public void TestNativeTransmitNormal()
        {
            if (SendQueue.IsHardwareAccelerated)
            {
                var received = RunCapture(Filter, (device) =>
                {
                    GetSendQueue().NativeTransmit(device, SendQueueTransmitModes.Normal);
                });
                AssertGoodTransmitNormal(received);
            }
            else
            {
                Assert.Ignore("Skipping test as no hardware acceleration is present");
            }
        }

        [Test]
        public void TestNativeTransmitSync()
        {
            if (SendQueue.IsHardwareAccelerated)
            {
                var received = RunCapture(Filter, (device) =>
                {
                    GetSendQueue().NativeTransmit(device, SendQueueTransmitModes.Synchronized);
                });
                AssertGoodTransmitSync(received);
            }
            else
            {
                Assert.Ignore("Skipping test as no hardware acceleration is present");
            }
        }

        [Test]
        public void TestManagedTransmitNormal()
        {
            var received = RunCapture(Filter, (device) =>
            {
                GetSendQueue().ManagedTransmit(device, SendQueueTransmitModes.Normal);
            });
            AssertGoodTransmitNormal(received);
        }

        [Test]
        public void TestManagedTransmitSync()
        {
            var received = RunCapture(Filter, (device) =>
            {
                GetSendQueue().ManagedTransmit(device, SendQueueTransmitModes.Synchronized);
            });
            AssertGoodTransmitSync(received);
        }

        private static decimal[] GetDeltaTimes(List<RawCapture> received)
        {
            // Skip the first two, those get penalized due to JIT
            var times = received.Skip(2)
                .Select(r => r.Timeval.Value).ToArray();
            var times1 = times.Skip(1);
            var times2 = times.Take(times.Length - 1);
            return times1.Zip(times2, (t1, t2) => t1 - t2).ToArray();
        }

        private static void AssertGoodTransmitNormal(List<RawCapture> received)
        {
            Assert.That(received, Has.Count.EqualTo(PacketCount));
            var deltas = GetDeltaTimes(received);
            // Windows usually can not put a delta smaller than 20µs
            // We tolorate up to 100µs to avoid flaky tests
            // Ensure 95% of packets have delta < 100µs
            Assert.That(Percentile(deltas, 0.95M), Is.LessThan(100e-6M));
        }

        private static void AssertGoodTransmitSync(List<RawCapture> received)
        {
            Assert.That(received, Has.Count.EqualTo(PacketCount));
            var deltas = GetDeltaTimes(received);
            // Ensure 90% of packets have delta = DeltaTime +/- 10%
            Assert.That(Percentile(deltas, 0.1M), Is.GreaterThan(DeltaTime * 0.9M));
            Assert.That(Percentile(deltas, 0.9M), Is.LessThan(DeltaTime * 1.1M));
            // Ensure all packets have delta = DeltaTime +/- 50%
            Assert.That(Percentile(deltas, 0), Is.GreaterThan(DeltaTime * 0.5M));
            Assert.That(Percentile(deltas, 1), Is.LessThan(DeltaTime * 1.5M));
        }

        /// <summary>
        /// Statistical percentile
        /// From https://stackoverflow.com/a/8137455
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="percentile"></param>
        /// <returns></returns>
        private static decimal Percentile(decimal[] sequence, decimal percentile)
        {
            Array.Sort(sequence);
            var N = sequence.Length;
            var n = (N - 1) * percentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1)
            {
                return sequence[0];
            }
            else if (n == N)
            {
                return sequence[N - 1];
            }
            else
            {
                var k = (int)n;
                var d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }

        [Test]
        public void TestReturnValue()
        {
            if (SendQueue.IsHardwareAccelerated)
            {
                using var device = GetPcapDevice();
                device.Open();
                var queue = GetSendQueue();
                var managed = queue.ManagedTransmit(device, SendQueueTransmitModes.Normal);
                var native = queue.NativeTransmit(device, SendQueueTransmitModes.Normal);
                Assert.AreEqual(managed, native);
            }
            else
            {
                Assert.Ignore("Skipping test as no hardware acceleration is present");
            }
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
        internal static SendQueueWrapper GetSendQueue(ushort ethertype = 0x1234)
        {
            var queue = new SendQueueWrapper((14 + PcapHeader.MemorySize) * PacketCount);
            var packet = EthernetPacket.RandomPacket();
            packet.Type = (EthernetType)ethertype;
            var time = new PosixTimeval();
            for (var i = 0; i < PacketCount; i++)
            {
                time.Value = 123456 + i * DeltaTime;
                Assert.IsTrue(queue.Add(packet.Bytes, (int)time.Seconds, (int)time.MicroSeconds));
            }
            return queue;
        }

        internal class SendQueueWrapper : SendQueue
        {
            public SendQueueWrapper(int memSize) : base(memSize)
            {
            }

            internal new int NativeTransmit(PcapDevice device, SendQueueTransmitModes transmitMode)
            {
                return base.NativeTransmit(device, transmitMode);
            }

            internal new int ManagedTransmit(PcapDevice device, SendQueueTransmitModes transmitMode)
            {
                return base.ManagedTransmit(device, transmitMode);
            }
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
