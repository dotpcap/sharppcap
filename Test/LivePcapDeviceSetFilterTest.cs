// Copyright 2009-2021 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

using System;
using System.Linq;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap;

namespace Test
{
    [TestFixture]
    public class LivePcapDeviceSetFilterTest
    {
        [Test]
        public void SimpleFilter([CaptureDevices] DeviceFixture fixture)
        {
            // BPF is known to support those link layers, 
            // support for other link layers such as NFLOG and USB is unknown
            var supportedLinks = new[]
            {
                LinkLayers.Ethernet,
                LinkLayers.Raw,
                LinkLayers.Null
            };
            using var device = fixture.GetDevice();
            device.Open();
            if (!supportedLinks.Contains(device.LinkType))
            {
                Assert.Inconclusive("NFLOG link-layer not supported");
            }
            var filter = "tcp port 80";
            device.Filter = filter;
            Assert.That(device.Filter, Is.EqualTo(filter));
        }

        /// <summary>
        /// Test that we get the expected exception if PcapDevice.SetFilter()
        /// is called on a PcapDevice that has not been opened
        /// </summary>
        [Test]
        public void SetFilterExceptionIfDeviceIsClosed([CaptureDevices] DeviceFixture fixture)
        {
            var device = fixture.GetDevice();
            Assert.Throws<DeviceNotReadyException>(
                () => device.Filter = "tcp port 80",
                "Did not catch the expected DeviceNotReadyException"
            );
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
