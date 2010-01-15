using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpPcap;

namespace Test
{
    [TestFixture]
    public class PcapGetAllDevices
    {
        [Test]
        public void PcapGetAllDevicesTest()
        {
            List<LivePcapDevice> devices = Pcap.GetAllDevices();

            if(devices.Count == 0)
            {
                throw new System.InvalidOperationException("No pcap supported devices found, are you running" +
                                                           " as a user with access to adapters (root on Linux)?");
            } else
            {
                Console.WriteLine("Found {0} devices", devices.Count);
            }

            foreach(LivePcapDevice d in devices)
            {
                Console.WriteLine(d.ToString());
            }
        }
    }
}
