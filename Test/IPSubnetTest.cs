using System;
using System.Collections;
using SharpPcap.Util;

namespace Test
{
    /// <summary>
    /// Summary description for IPSubnetTest.
    /// </summary>
    public class IPSubnetTest
    {
        public static void Test()
        {
            IPSubnet ipRange = new IPSubnet("10.10.10.0", 24);
            Console.WriteLine(ipRange.NetworkAddress);
            Console.WriteLine(ipRange.BroadcastAddress);
            ipRange.setRandom(false);

            foreach(System.Net.IPAddress ip in ipRange)
            {
                System.Console.Out.WriteLine(ip);                
            }
        }
    }
}
