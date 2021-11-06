using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Linq;
using System.Threading;

namespace Test
{
    [TestFixture]
    public class ThreadSafeTests
    {

        [Test]
        public void TestThreadSafety()
        {
            var pcapDevice = TestHelper.GetPcapDevice();
            var devices = Enumerable.Range(0, 1000)
                .Select(_ => new LibPcapLiveDevice(pcapDevice.Interface))
                .ToArray();

            foreach (var device in devices)
            {
                Initialize(device, 0);
            }
            Thread.Sleep(5000);
            foreach (var device in devices)
            {
                device.Dispose();
            }
        }

        private static void Initialize(LibPcapLiveDevice device, int nr)
        {
            if (nr == 5)
            {
                return;
            }
            try
            {
                device.Open(DeviceModes.Promiscuous);
                device.OnPacketArrival += Device_OnPacketArrival;
                device.StartCapture();
            }
            catch (Exception)
            {
                Initialize(device, nr + 1);
            }
        }

        private static void Device_OnPacketArrival(object sender, PacketCapture e)
        {
            try
            {
                var device = (LibPcapLiveDevice)sender;
                var t = new Thread(new ParameterizedThreadStart((obj) =>
                {
                    var device = (LibPcapLiveDevice)obj;
                    try
                    {
                        device.Dispose();
                        Thread.Sleep(10);
                        Initialize(device, 0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception happened in thread:{ex}");
                    }
                }));
                t.Start(e.Device);
                // Trigger the data to be read from the pcap_t memory
                e.GetPacket();
            }
            catch (DeviceNotReadyException)
            {
                // Pass, normal
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception happened:{ex}");
            }
        }
    }
}
