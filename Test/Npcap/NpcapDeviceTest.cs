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
 * Copyright 2011 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using NUnit.Framework;
using SharpPcap;

namespace Test.Npcap
{
    [TestFixture]
    [NonParallelizable]
    public class NpcapDeviceTest
    {
        /// <summary>
        /// Test that no exceptions are thrown from NpcapDevice.StartCapture() if
        /// a statistics event handler is attached but no packet capture handler
        /// </summary>
        [Test]
        public void NoExceptionsWithJustStatisticsHandler()
        {
            var devices = SharpPcap.Npcap.NpcapDeviceList.Instance;
            if (devices.Count == 0)
            {
                throw new InvalidOperationException("No npcap devices found, are you running" +
                                                           " on windows?");
            }

            using var device = devices[0];
            device.Open();
            device.OnPcapStatistics += (sender, args) => { };

            Assert.DoesNotThrow(() => device.StartCapture());
        }

        /// <summary>
        /// Test that we get the appropriate exception from StartCapture() if
        /// there hasn't been any delegates assigned to OnPacketArrival or
        /// OnPcapStatistics
        /// </summary>
        [Test]
        public void DeviceNotReadyExceptionWhenStartingACaptureWithoutAddingDelegateToOnPacketArrivalAndOnPcapStatistics()
        {
            var devices = SharpPcap.Npcap.NpcapDeviceList.Instance;
            if (devices.Count == 0)
            {
                throw new InvalidOperationException("No npcap devices found, are you running" +
                                                           " on windows?");
            }

            using var device = devices[0];
            device.Open();

            Assert.Throws<DeviceNotReadyException>(() => device.StartCapture());
        }
    }
}

