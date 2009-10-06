using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpPcap;

namespace Test
{
    [TestFixture]
    public class SetFilterTest
    {
        [Test]
        public void SimpleFilter()
        {
            List<PcapDevice> devices = Pcap.GetAllDevices();

            if(devices.Count == 0)
            {
                Console.WriteLine("No pcap supported devices found, are you running" +
                                  " as a user with access to adapters (root on Linux)?");
                return;
            }

            devices[0].SetFilter("tcp port 80");
            devices[0].Open();
        }
    }
}
