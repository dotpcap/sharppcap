// Copyright 2021 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test
{
    [TestFixture]
    public class LibPcapLiveDeviceTest
    {
        [Test]
        public void LibPcapLiveDeviceProperties()
        {
            var d = LibPcapLiveDeviceList.Instance[0];
            Assert.That(d, Is.Not.Null);

            Assert.That(d.Addresses, Is.Not.Null);
            Console.WriteLine("Flags: {0}", d.Flags);
            Console.WriteLine("Loopback: {0}", d.Loopback);
        }

        /// <summary>
        /// Test specifying the timestamp type when opening devices
        /// </summary>
        /// <param name="fixture"></param>
        [Category("Timestamp")]
        [Test]
        public void DeviceOpenWithTimestampType([CaptureDevices] DeviceFixture fixture)
        {
            using var device = (PcapDevice)fixture.GetDevice();
            if (!(device is LibPcapLiveDevice))
                return;

            var liveDevice = device as LibPcapLiveDevice;

            var timestampTypes = liveDevice.Interface.TimestampsSupported;

            Assert.That(timestampTypes, Is.Not.Empty);

            // open the device with each of its supported timestamp types
            foreach (var pcapClock in timestampTypes)
            {
                var configuration = new DeviceConfiguration();
                configuration.TimestampType = pcapClock.TimestampType;
                liveDevice.Open(configuration);

                Assert.That(liveDevice.Interface.TimestampsSupported, Is.Not.Null);

                liveDevice.Close();
            }
        }

        [Test]
        public void NonBlockingMode()
        {
            using var d = LibPcapLiveDeviceList.Instance[0];
            Assert.That(d, Is.Not.Null);

            // assert that setting and getting non blocking mode
            // with a closed device
            Assert.Throws<DeviceNotReadyException>(() => d.NonBlockingMode = true);
            Assert.Throws<DeviceNotReadyException>(() => { var x = d.NonBlockingMode; });

            // test that we can set the blocking mode on an open device
            d.Open();
            d.NonBlockingMode = true;
            Assert.That(d.NonBlockingMode, Is.True);
            d.NonBlockingMode = false;
            Assert.That(d.NonBlockingMode, Is.False);
        }
    }
}
