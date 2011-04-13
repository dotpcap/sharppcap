using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap.AirPcap;

namespace AirPcapDeviceListExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("AirPcap version {0}", AirPcapVersion.VersionString());

            var devices = AirPcapDeviceList.Instance;

            Console.WriteLine("AirPcap devices found:");
            for(var i = 0; i < devices.Count; i++)
            {
                Console.WriteLine("[{0}] {1}", i, devices[i].ToString());
            }

            if (devices.Count == 0)
            {
                Console.WriteLine("No devices found");
            }
        }
    }
}
