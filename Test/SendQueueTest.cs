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
        private const int PacketCount = 8;
        internal static readonly int DeltaMs = 10;

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

        private static void AssertGoodTransmitNormal(List<RawCapture> received)
        {
            Assert.That(received, Has.Count.EqualTo(PacketCount));
            var times = received.Select(r => r.Timeval.Date);
            Assert.That(times.Max() - times.Min(), Is.LessThan(FromMilliseconds(10)));
        }

        private static void AssertGoodTransmitSync(List<RawCapture> received)
        {
            Assert.That(received, Has.Count.EqualTo(PacketCount));
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
            var queue = new SendQueueWrapper(1024);
            var packet = EthernetPacket.RandomPacket();
            packet.Type = (EthernetType)ethertype;
            for (var i = 0; i < PacketCount; i++)
            {
                Assert.IsTrue(queue.Add(packet.Bytes, 123456, i * DeltaMs * 1000));
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
