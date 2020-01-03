using System;
using NUnit.Framework;
using SharpPcap.LibPcap;

namespace Test
{
    [TestFixture]
    public class LivePcapDeviceListTest
    {
        [Test]
        public void AllDevicesTest()
        {
            if(LibPcapLiveDeviceList.Instance.Count == 0)
            {
                throw new InvalidOperationException("No pcap supported devices found, are you running" +
                                                           " as a user with access to adapters (root on Linux)?");
            } else
            {
                Console.WriteLine("Found {0} devices", LibPcapLiveDeviceList.Instance.Count);
            }

            foreach(var d in LibPcapLiveDeviceList.Instance)
            {
                Console.WriteLine(d.ToString());
            }
        }
    }
}
