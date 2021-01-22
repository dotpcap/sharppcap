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
        [Test]
        public void DeviceProperties([PcapDevices] DeviceFixture fixture)
        {
            var device = (PcapDevice)fixture.GetDevice();

            device.Open();
            var pcapIf = device.Interface;

            Assert.IsTrue(device.Opened);

            Assert.IsNotEmpty(device.Name);
            Assert.AreEqual(device.Name, pcapIf.Name);
            Assert.AreEqual(device.Description, pcapIf.Description);

            Assert.IsNotNull(pcapIf.GatewayAddresses);
            Assert.IsNotNull(pcapIf.Addresses);

            if (pcapIf.MacAddress != null)
            {
                Assert.That(pcapIf.MacAddress.GetAddressBytes(), Has.Length.EqualTo(6));
            }

            device.Close();
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
            var device = fixture.GetDevice();

            Assert.IsFalse(device.Started, "Expected device not to be Started");

            device.Open();
            device.OnPacketArrival += HandleOnPacketArrival;

            // start background capture
            device.StartCapture();

            Assert.IsTrue(device.Started, "Expected device to be Started");

            // attempt to get the next packet via GetNextPacket()
            // to ensure that we get the exception we expect
            Assert.Throws<InvalidOperationDuringBackgroundCaptureException>(
                () => device.GetNextPacket()
            );

            device.Close();
        }

        /// <summary>
        /// Test that we get the appropriate exception from PcapDevice.StartCapture() if
        /// there hasn't been any delegates assigned to PcapDevice.OnPacketArrival
        /// </summary>
        [Test]
        public void DeviceNotReadyExceptionWhenStartingACaptureWithoutAddingDelegateToOnPacketArrival(
           [CaptureDevices] DeviceFixture fixture
        )
        {
            var device = fixture.GetDevice();
            device.Open();

            Assert.Throws<DeviceNotReadyException>(
                () => device.StartCapture()
            );

            device.Close();
        }

        void HandleOnPacketArrival(object sender, CaptureEventArgs e)
        {

        }

        [Test]
        public void ReceivePacketsWithStartCapture()
        {
            const int PacketsCount = 10;
            var packets = new List<RawCapture>();
            var statuses = new List<CaptureStoppedEventStatus>();
            void Receiver_OnPacketArrival(object s, CaptureEventArgs e)
            {
                packets.Add(e.Packet);
            }
            void Receiver_OnCaptureStopped(object s, CaptureStoppedEventStatus status)
            {
                statuses.Add(status);
            }
            // We can't use the same device for async capturing and sending
            var device = GetPcapDevice();
            var receiver = new LibPcapLiveDevice(device.Interface);
            var sender = new LibPcapLiveDevice(receiver.Interface);
            try
            {
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
            }
            finally
            {
                receiver.OnPacketArrival -= Receiver_OnPacketArrival;
                sender.Close();
                receiver.Close();
            }
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

