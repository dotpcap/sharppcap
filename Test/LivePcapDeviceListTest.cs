using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpPcap;

namespace Test
{
    [TestFixture]
    public class LivePcapDeviceListTest
    {
        [Test]
        public void AllDevicesTest()
        {
            if(LivePcapDeviceList.Instance.Count == 0)
            {
                throw new System.InvalidOperationException("No pcap supported devices found, are you running" +
                                                           " as a user with access to adapters (root on Linux)?");
            } else
            {
                Console.WriteLine("Found {0} devices", LivePcapDeviceList.Instance.Count);
            }

            foreach(LivePcapDevice d in LivePcapDeviceList.Instance)
            {
                Console.WriteLine(d.ToString());
            }
        }
    }
}
