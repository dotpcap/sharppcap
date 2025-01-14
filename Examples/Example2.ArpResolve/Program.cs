using System;
using System.Net;
using SharpPcap;
using SharpPcap.LibPcap;

// Copyright (c) 2006 Tamir Gal, http://www.tamirgal.com, All rights reserved.
//
// SPDX-License-Identifier: MIT

namespace Example2
{
    /// <summary>
    /// A sample showing how to use the Address Resolution Protocol (ARP)
    /// with the SharpPcap library.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            // Print SharpPcap version
            var ver = Pcap.SharpPcapVersion;
            Console.WriteLine("SharpPcap {0}, Example2.ArpResolve.cs\n", ver);

            // Retrieve the device list
            var devices = LibPcapLiveDeviceList.Instance;

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            // Print out the available devices
            foreach (var dev in devices)
            {
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device for sending the ARP request: ");
            i = int.Parse(Console.ReadLine());

            var device = devices[i];

            System.Net.IPAddress ip;

            // loop until a valid ip address is parsed
            while (true)
            {
                Console.Write("-- Please enter IP address to be resolved by ARP: ");
                if (IPAddress.TryParse(Console.ReadLine(), out ip))
                    break;
                Console.WriteLine("Bad IP address format, please try again");
            }

            // Create a new ARP resolver
            ARP arper = new ARP(device);

            // print the resolved address or indicate that none was found
            var resolvedMacAddress = arper.Resolve(ip);
            if (resolvedMacAddress == null)
            {
                Console.WriteLine("Timeout, no mac address found for ip of " + ip);
            }
            else
            {
                Console.WriteLine(ip + " is at: " + arper.Resolve(ip));
            }
        }
    }
}
