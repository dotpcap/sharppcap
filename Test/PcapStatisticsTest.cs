using System;
using NUnit.Framework;
using SharpPcap;

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
            var devices = LivePcapDeviceList.Instance;

            if(devices.Count == 0)
            {
                var error = "No pcap supported devices found, are you running" +
                            " as a user with access to adapters (root on Linux)?";
                throw new System.InvalidOperationException(error);
            } else
            {
                Console.WriteLine("Found {0} devices", devices.Count);
            }

            LivePcapDevice dev = null;
            foreach(var d in devices)
            {
                Console.WriteLine(d.ToString());

                if(d.Name == "any")
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

            Assert.IsNotNull(dev, "Unable to find a capture device");

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
            var devices = LivePcapDeviceList.Instance;

            if(devices.Count == 0)
            {
                var error = "No pcap supported devices found, are you running" +
                            " as a user with access to adapters (root on Linux)?";
                throw new System.InvalidOperationException(error);
            } else
            {
                Console.WriteLine("Found {0} devices", devices.Count);
            }

            var caughtException = false;
            try
            {
#pragma warning disable 0168
                // attempt to retrieve statistics from a closed device
                var stats = devices[0].Statistics;
#pragma warning restore 0168
            } catch(DeviceNotReadyException)
            {
                caughtException = true;
            }

            // ensure that we caught an exception
            Assert.IsTrue(caughtException, "Did not catch PcapDeviceNotReadyException");
        }
    }
}
