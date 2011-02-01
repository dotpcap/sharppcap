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

namespace Test
{
    [TestFixture]
    public class WinPcapDeviceTest
    {
        /// <summary>
        /// Test that no exceptions are thrown from WinPcapDevice.StartCapture() if
        /// a statistics event handler is attached but no packet capture handler
        /// </summary>
        [Test]
        public void NoExceptionsWithJustStatisticsHandler ()
        {
            var devices = SharpPcap.WinPcap.WinPcapDeviceList.Instance;
            if(devices.Count == 0)
            {
                throw new System.InvalidOperationException("No winpcap devices found, are you running" +
                                                           " on windows?");
            }

            devices[0].Open();

            bool caughtException = false;

            try
            {
                // start background capture
                devices[0].StartCapture();
            } catch(DeviceNotReadyException)
            {
                caughtException = true;
            }

            Assert.IsFalse(caughtException);

            devices[0].Close();
        }

        /// <summary>
        /// Test that we get the appropriate exception from StartCapture() if
        /// there hasn't been any delegates assigned to OnPacketArrival or
        /// OnPcapStatistics
        /// </summary>
        [Test]
        public void DeviceNotReadyExceptionWhenStartingACaptureWithoutAddingDelegateToOnPacketArrivalAndOnPcapStatistics ()
        {
            var devices = SharpPcap.WinPcap.WinPcapDeviceList.Instance;
            if(devices.Count == 0)
            {
                throw new System.InvalidOperationException("No winpcap devices found, are you running" +
                                                           " on windows?");
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
    }
}

