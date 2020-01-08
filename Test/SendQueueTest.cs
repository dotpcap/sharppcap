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
    public class SendQueueTest
    {
        private const string Filter = "ether proto 0x1234";
        private const int PacketCount = 8;
        private static readonly int DeltaMs = 10;

        [Test]
        public void TestNativeTransmitNormal()
        {
            if (SendQueue.IsHardwareAccelerated)
            {
                var received = RunCapture(Filter, (device) =>
                {
                    GetSendQueue().NativeTransmit(device, false);
                });
                AssertGoodTransmitNormal(received);
            } else
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
                    GetSendQueue().NativeTransmit(device, true);
                });
                AssertGoodTransmitSync(received);
            } else
            {
                Assert.Ignore("Skipping test as no hardware acceleration is present");
            }
        }

        [Test]
        public void TestManagedTransmitNormal()
        {
            var received = RunCapture(Filter, (device) =>
            {
                GetSendQueue().ManagedTransmit(device, false);

                // Test hack: MacOS 10.15, delay of 1000ms causes this test to pass on cmorgan's 2019 macbook pro,
                // delay of 500ms does not
                System.Threading.Thread.Sleep(1000);
            });
            AssertGoodTransmitNormal(received);
        }

        [Test]
        public void TestManagedTransmitSync()
        {
            var received = RunCapture(Filter, (device) =>
            {
                GetSendQueue().ManagedTransmit(device, true);

                // Test hack: MacOS 10.15, delay of 2000ms causes this test to pass on cmorgan's 2019 macbook pro,
                // delay of 1500ms does not
                System.Threading.Thread.Sleep(2000);
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
                var device = GetPcapDevice();
                device.Open();
                try
                {
                    var queue = GetSendQueue();
                    var managed = queue.ManagedTransmit(device, false);
                    var native = queue.NativeTransmit(device, false);
                    Assert.AreEqual(managed, native);
                }
                finally
                {
                    device.Close();
                }
            } else
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
        private static SendQueueWrapper GetSendQueue()
        {
            var queue = new SendQueueWrapper(1024);
            var packet = EthernetPacket.RandomPacket();
            packet.Type = (EthernetType)0x1234;
            for (var i = 0; i < PacketCount; i++)
            {
                Assert.IsTrue(queue.Add(packet.Bytes, 123456, i * DeltaMs * 1000));
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
