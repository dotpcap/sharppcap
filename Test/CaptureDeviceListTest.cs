// Copyright 2021 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

using System;
using SharpPcap;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class CaptureDeviceListTest
    {
        /// <summary>
        /// Test that we can create a new device list
        /// </summary>
        [Test]
        public void CaptureDeviceListNew()
        {
            var deviceList = CaptureDeviceList.New();
            Assert.That(deviceList, Is.Not.Null);
            Assert.That(deviceList, Is.Not.Empty);
        }
    }
}
