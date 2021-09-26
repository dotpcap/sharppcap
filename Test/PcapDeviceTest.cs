/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using static Test.TestHelper;

namespace Test
{
    [TestFixture]
    [NonParallelizable]
    public class PcapDeviceTest
    {
        /// <summary>
        /// Test that we can convert each TimestampType
        /// </summary>
        [Category("Timestamp")]
        [Test]
        public void TimestampConversions()
        {
            foreach (TimestampType timestampType in Enum.GetValues(typeof(TimestampType)))
            {
                var pcapClock = new PcapClock(timestampType);
                Assert.IsNotNull(pcapClock);
                Assert.IsNotEmpty(pcapClock.Name);
                Assert.IsNotEmpty(pcapClock.Description);
            }
        }

        public void DeviceTest(PcapDevice device, TimestampResolution? targetResolution)
        {
            var pcapIf = device.Interface;

            Assert.IsTrue(device.Opened);

            Assert.IsNotEmpty(device.Name);
            Assert.AreEqual(device.Name, pcapIf.Name);
            Assert.AreEqual(device.Description, pcapIf.Description);

            Assert.IsEmpty(device.LastError);

            Assert.IsNotNull(pcapIf.GatewayAddresses);
            Assert.IsNotNull(pcapIf.Addresses);

            var resolution = device.TimestampResolution;

            var expectedResolution = targetResolution ?? TimestampResolution.Microsecond;

            // confirm the resolution was set
            Assert.AreEqual(expectedResolution, resolution);
        }

        /// <summary>
        /// Note: This tests the default timestamp precision as set by libpcap internally
        /// </summary>
        /// <param name="fixture"></param>
        [Test]
        public void DeviceOpen([CaptureDevices] DeviceFixture fixture)
        {
            using var device = (PcapDevice)fixture.GetDevice();

            device.Open();
            DeviceTest(device, null);
        }

        [Test]
        [Category("Timestamp")]
        [LibpcapVersion(">=1.5.0")]
        public void DeviceOpenWithTimestampPrecision(
            [CaptureDevices] DeviceFixture fixture,
            [Values] TimestampResolution resolution
        )
        {
            using var device = (PcapDevice)fixture.GetDevice();
            try
            {
                var configuration = new DeviceConfiguration();
                configuration.TimestampResolution = resolution;
                device.Open(configuration);
                DeviceTest(device, resolution);
            }
            catch (PcapException ex)
            {
                // its ok if the device does not support setting the precision, all other PcapError
                // types are considered test failures
                Assert.AreEqual(PcapError.TimestampPrecisionNotSupported, ex.Error);
                Assert.Ignore("Device does not support this timestamp precision");
            }
        }

        [Test]
        public void BufferSize()
        {
            using var device = GetPcapDevice();
            var size_64mb = 64 * 1024 * 1024;

            device.Open(StrictConfig(new DeviceConfiguration
            {
                BufferSize = size_64mb,
            }));
        }

        /// <summary>
        /// Non compatible modes should raise ConfigurationFailed
        /// </summary>
        [Test]
        public void NonCompatibleModes()
        {
            using var device = GetPcapDevice();

            var config = new DeviceConfiguration
            {
                Mode = DeviceModes.NoCaptureLocal,
                BufferSize = 128 * 1024,
            };

            var ex = Assert.Throws<PcapException>(() => device.Open(config));
            if (ex.Error != PcapError.PlatformNotSupported)
            {
                StringAssert.Contains(nameof(DeviceConfiguration.BufferSize), ex.Message);
            }
        }

        /// <summary>
        /// It shall be possible to set Immediate mode (aka Max Responsiveness) even in Libpcap that do not have pcap_open
        /// by using pcap_set_immediate_mode
        /// </summary>
        [Test]
        [LibpcapVersion(">=1.5.0")]
        public void MaxResponsivenessIsSameAsImmediate()
        {
            using var device = GetPcapDevice();
            // We don't have much to assert on, but we shall see the path covered in codecov
            device.Open(DeviceModes.Promiscuous | DeviceModes.MaxResponsiveness);
        }

        [Test]
        [Platform("Win")]
        [TestCase("KernelBufferSize", 64 * 1000 * 1000)]
        [TestCase("MinToCopy", 10000)]
        public void Configuration_Windows(string property, int value)
        {
            using var device = GetPcapDevice();

            var config = new DeviceConfiguration();
            config.GetType().GetProperty(property).SetValue(config, value);

            device.Open(StrictConfig(config));
        }

        [Test]
        [Platform(Exclude = "Win")]
        [TestCase("KernelBufferSize", 64 * 1000 * 1000)]
        [TestCase("MinToCopy", 10000)]
        public void Configuration_NotWindows(string property, int value)
        {
            using var device = GetPcapDevice();
            var config = new DeviceConfiguration();
            config.GetType().GetProperty(property).SetValue(config, value);
            var failures = new List<ConfigurationFailedEventArgs>();
            config.ConfigurationFailed += (s, e) =>
            {
                failures.Add(e);
            };
            device.Open(config);
            Assert.That(failures, Has.Count.EqualTo(1));
            var fail = failures[0];
            Assert.AreEqual(property, fail.Property);
            Assert.AreEqual(PcapError.PlatformNotSupported, fail.Error);
        }

        [Test]
        public void Immediate([Values] bool? immediate)
        {
            using var device = GetPcapDevice();
            device.Open(StrictConfig(new DeviceConfiguration
            {
                Immediate = immediate,
            }));
        }

        /// <summary>
        /// Test that we can retrieve packet information via GetNextPacketPointers
        /// </summary>
        [Test]
        public void GetNextPacketPointers()
        {
            using var device = TestHelper.GetPcapDevice();
            device.Open();

            // confirm that we can retrieve the next packet pointers
            var header = IntPtr.Zero;
            var data = IntPtr.Zero;
            device.GetNextPacketPointers(ref header, ref data);
            Assert.AreNotEqual(IntPtr.Zero, header);
        }

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
        public void GetNextPacketExceptionIfCaptureLoopRunning(
            [CaptureDevices] DeviceFixture fixture
        )
        {
            using var device = fixture.GetDevice();

            Assert.IsFalse(device.Started, "Expected device not to be Started");

            device.Open();
            device.OnPacketArrival += HandleOnPacketArrival;

            // start background capture
            device.StartCapture();

            Assert.IsTrue(device.Started, "Expected device to be Started");

            // attempt to get the next packet via GetNextPacket()
            // to ensure that we get the exception we expect
            Assert.Throws<InvalidOperationDuringBackgroundCaptureException>(
                () => device.GetNextPacket(out var _)
            );
        }

        /// <summary>
        /// Test that we get the appropriate exception from PcapDevice.StartCapture() if
        /// there hasn't been any delegates assigned to PcapDevice.OnPacketArrival
        /// </summary>
        [Test]
        public void DeviceNotReadyExceptionWhenStartingACaptureWithoutAddingDelegateToOnPacketArrival()
        {
            using var device = TestHelper.GetPcapDevice();
            device.Open();

            Assert.Throws<DeviceNotReadyException>(
                () => device.StartCapture()
            );
        }

        /// <summary>
        /// Test that we get the appropriate exception when starting a capture on a closed device
        /// </summary>
        [Test]
        public void DeviceNotReadyExceptionWhenStartingACaptureOnAClosedDevice()
        {
            using var device = TestHelper.GetPcapDevice();
            Assert.Throws<DeviceNotReadyException>(
                () => device.StartCapture()
            );
        }

        void HandleOnPacketArrival(object sender, PacketCapture e)
        {

        }

        [Test]
        public void ReceivePacketsWithStartCapture()
        {
            const int PacketsCount = 10;
            var packets = new List<RawCapture>();
            var statuses = new List<CaptureStoppedEventStatus>();
            void Receiver_OnPacketArrival(object s, PacketCapture e)
            {
                packets.Add(e.GetPacket());
            }
            void Receiver_OnCaptureStopped(object s, CaptureStoppedEventStatus status)
            {
                statuses.Add(status);
            }
            // We can't use the same device for async capturing and sending
            var device = GetPcapDevice();
            using var receiver = new LibPcapLiveDevice(device.Interface);
            using var sender = new LibPcapLiveDevice(receiver.Interface);

            // Configure sender
            sender.Open();

            // Configure receiver
            receiver.Open(DeviceModes.Promiscuous);
            receiver.Filter = "ether proto 0x1234";
            receiver.OnPacketArrival += Receiver_OnPacketArrival;
            receiver.OnCaptureStopped += Receiver_OnCaptureStopped;
            receiver.StartCapture();

            // Send the packets
            var packet = EthernetPacket.RandomPacket();
            packet.Type = (EthernetType)0x1234;
            for (var i = 0; i < PacketsCount; i++)
            {
                sender.SendPacket(packet);
            }
            // Wait for packets to arrive
            Thread.Sleep(2000);
            receiver.StopCapture();

            // Checks
            Assert.That(packets, Has.Count.EqualTo(PacketsCount));
            Assert.That(statuses, Has.Count.EqualTo(1));
            Assert.AreEqual(statuses[0], CaptureStoppedEventStatus.CompletedWithoutError);
        }

        [SetUp]
        public void SetUp()
        {
            ConfirmIdleState();
        }

        [TearDown]
        public void Cleanup()
        {
            ConfirmIdleState();
        }

    }
}

