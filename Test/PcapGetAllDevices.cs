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
            List<PcapDevice> devices = Pcap.GetAllDevices();

            if(devices.Count == 0)
            {
                Console.WriteLine("No pcap supported devices found, are you running" +
                                  " as a user with access to adapters (root on Linux)?");
            } else
            {
                Console.WriteLine("Found {0} devices", devices.Count);
            }

            foreach(PcapDevice d in devices)
            {
                Console.WriteLine(d.ToString());
            }
        }
    }
}
