using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Test.TestHelper;

namespace Test
{
    /// <summary>
    /// Due to race conditions, it could happen that a device handle gets closed while being used by another thread
    /// We need to be able to at least prevent memory corruption
    /// If this test did what it should, you will see in the logs:
    ///     "The active test run was aborted. Reason: Test host process crashed" 
    /// We run each test 10 times in parallel, to make libpcap reuse freed memory and crash
    /// </summary>
    [TestFixture]
    [Category("MemorySafetry")]
    public class MemorySafetyTests
    {

        private readonly PcapInterface TestInterface = GetPcapDevice().Interface;

        [Test]
        [Parallelizable(ParallelScope.Children)]
        public void DisposeDuringCapture([Range(0, 9)] int _)
        {
            using var waitHandle = new AutoResetEvent(false);

            // We can't use the same device for async capturing and sending

            using var receiver = new LibPcapLiveDevice(TestInterface);
            using var sender = new LibPcapLiveDevice(TestInterface);

            // Configure sender
            sender.Open();

            // Configure receiver
            receiver.Open(DeviceModes.Promiscuous);
            receiver.Filter = "ether proto 0xDEAD";
            receiver.OnPacketArrival += (d, e) =>
            {
                try
                {
                    // Most simple form, dipose device while capture is running
                    Task.Run(receiver.Dispose).Wait();
                    // Now try to access the memory of the packet
                    Packet.ParsePacket(LinkLayers.Ethernet, e.Data.ToArray());
                }
                finally
                {
                    // Let test close
                    waitHandle.Set();
                }
            };
            receiver.StartCapture();

            // Send the packets
            var packet = EthernetPacket.RandomPacket();
            packet.Type = (EthernetType)0xDEAD;
            sender.SendPacket(packet);

            // Wait for packets to arrive
            Assert.IsTrue(waitHandle.WaitOne(5000));
        }


        [Test]
        [Parallelizable(ParallelScope.Children)]
        public void DisposeDuringTransmit([Range(0, 9)] int _)
        {
            using var device = new LibPcapLiveDevice(TestInterface);
            device.Open();
            var queue = SendQueueTest.GetSendQueue(0xDEAD);
            Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds((double)SendQueueTest.DeltaTime));
                device.Dispose();
            });
            try
            {
                queue.Transmit(device, SendQueueTransmitModes.Synchronized);
            }
            catch (Exception ex)
            when (ex is DeviceNotReadyException || ex is ObjectDisposedException)
            {
                // We are good, we disposed mid sending and we deserve the exception
            }
        }
    }
}
