using System;
using NUnit.Framework;
using SharpPcap;

namespace Test
{
    [TestFixture]
    public class PcapDeviceTest
    {
        /// <summary>
        /// Calling PcapDevice.GetNextPacket() while a capture loop is running
        /// in another thread causes errors inside of libpcap where at some point the
        /// capture will simply stop even though the capture thread appears to be running
        /// normally. This appears to be due to corruption of the pcap device structure
        /// allocated by libpcap.
        /// 
        /// Discovering the cause of this bug took me (Chris M.) two and half days of
        /// poking around and looking at strace output.
        /// 
        /// Test that the proper exception is thrown if a user tries to
        /// call GetNextPacket() while a capture loop is running.
        /// </summary>
        [Test]
        public void GetNextPacketExceptionIfCaptureLoopRunning ()
        {
            var devices = LivePcapDeviceList.Instance;
            if(devices.Count == 0)
            {
                throw new System.InvalidOperationException("No pcap supported devices found, are you running" +
                                                           " as a user with access to adapters (root on Linux)?");
            }

            devices[0].Open();
            devices[0].OnPacketArrival += HandleOnPacketArrival;

            // start background capture
            devices[0].StartCapture();

            // attempt to get the next packet via GetNextPacket()
            // to ensure that we get the exception we expect
            bool caughtExpectedException = false;
            try
            {
                devices[0].GetNextPacket();
            } catch(InvalidOperationDuringBackgroundCaptureException)
            {
                caughtExpectedException = true;
            }

            Assert.IsTrue(caughtExpectedException);

            devices[0].Close();
        }

        /// <summary>
        /// Test that we get the appropriate exception from PcapDevice.StartCapture() if
        /// there hasn't been any delegates assigned to PcapDevice.OnPacketArrival
        /// </summary>
        [Test]
        public void DeviceNotReadyExceptionWhenStartingACaptureWithoutAddingDelegateToOnPacketArrival ()
        {
            var devices = LivePcapDeviceList.Instance;
            if(devices.Count == 0)
            {
                throw new System.InvalidOperationException("No pcap supported devices found, are you running" +
                                                           " as a user with access to adapters (root on Linux)?");
            }

            devices[0].Open();

            bool caughtExpectedException = false;

            try
            {
                // start background capture
                devices[0].StartCapture();
            } catch(DeviceNotReadyException)
            {
                caughtExpectedException = true;
            }

            Assert.IsTrue(caughtExpectedException);

            devices[0].Close();
        }

        void HandleOnPacketArrival (object sender, CaptureEventArgs e)
        {
            
        }
    }
}

