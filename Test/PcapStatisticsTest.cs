// Copyright 2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test
{
    [TestFixture]
    public class PcapStatisticsTest
    {
        /// <summary>
        /// Test retrieving statistics from the 'any' device, if one is found
        /// </summary>
        [Test]
        public void TestStatistics()
        {
            var devices = LibPcapLiveDeviceList.Instance;

            if (devices.Count == 0)
            {
                var error = "No pcap supported devices found, are you running" +
                            " as a user with access to adapters (root on Linux)?";
                throw new InvalidOperationException(error);
            }
            else
            {
                Console.WriteLine("Found {0} devices", devices.Count);
            }

            LibPcapLiveDevice dev = null;
            foreach (var d in devices)
            {
                Console.WriteLine(d.ToString());

                if (d.Name == "any")
                {
                    dev = d;
                }
            }

            // if we couldn't find the 'any' device (maybe we are running on Windows)
            // then just use the first device we can find, if there are any devices
            if ((dev == null) && devices.Count != 0)
            {
                dev = devices[0];
            }

            Assert.That(dev, Is.Not.Null, "Unable to find a capture device");

            // open a device for capture
            dev.Open();

            // wait a little while so maybe packets will pass by
            System.Threading.Thread.Sleep(500);

            // retrieve the statistics
            var statistics = dev.Statistics;

            // output the statistics
            Console.WriteLine("statistics: {0}", statistics.ToString());

            dev.Close();
        }

        /// <summary>
        /// Ensure that we get an exception if we call PcapDevice.Statistics()
        /// on a closed device
        /// </summary>
        [Test]
        public void TestStatisticsException()
        {
            var devices = LibPcapLiveDeviceList.Instance;

            if (devices.Count == 0)
            {
                var error = "No pcap supported devices found, are you running" +
                            " as a user with access to adapters (root on Linux)?";
                throw new InvalidOperationException(error);
            }
            else
            {
                Console.WriteLine("Found {0} devices", devices.Count);
            }

            // ensure that we caught an exception
            Assert.Throws<DeviceNotReadyException>(
                () => devices[0].Statistics.ToString(),
                "Did not catch PcapDeviceNotReadyException"
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
