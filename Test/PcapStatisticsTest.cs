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
            var devices = Pcap.GetAllDevices();

            if(devices.Count == 0)
            {
                var error = "No pcap supported devices found, are you running" +
                            " as a user with access to adapters (root on Linux)?";
                Console.WriteLine(error);
                throw new System.InvalidOperationException(error);
            } else
            {
                Console.WriteLine("Found {0} devices", devices.Count);
            }

            PcapDevice dev = null;
            foreach(var d in devices)
            {
                Console.WriteLine(d.ToString());

                if(d.Name == "any")
                {
                    dev = d;
                }
            }

            // open a device for capture
            dev.Open();

            // wait a little while so maybe packets will pass by
            System.Threading.Thread.Sleep(500);

            // retrieve the statistics
            var statistics = dev.Statistics();

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
            var devices = Pcap.GetAllDevices();

            if(devices.Count == 0)
            {
                var error = "No pcap supported devices found, are you running" +
                            " as a user with access to adapters (root on Linux)?";
                Console.WriteLine(error);
                throw new System.InvalidOperationException(error);
            } else
            {
                Console.WriteLine("Found {0} devices", devices.Count);
            }

            var caughtException = false;
            try
            {
                // attempt to retrieve statistics from a closed device
                devices[0].Statistics();
            } catch(PcapDeviceNotReadyException)
            {
                caughtException = true;
            }

            // ensure that we caught an exception
            Assert.IsTrue(caughtException, "Did not catch PcapDeviceNotReadyException");
        }
    }
}
