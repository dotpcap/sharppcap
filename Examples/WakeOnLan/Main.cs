/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

using System;
using PacketDotNet;
using SharpPcap;

namespace SharpPcap.Examples
{
    class WakeOnLanCapture
    {
        /// <summary>
        /// A basic Wake-On-LAN capture example
        /// </summary>
        public static void Main (string[] args)
        {
            // print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}", ver);

            // retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // if no devices were found print an error
            if(devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            // scan the list printing every entry
            foreach(var dev in devices)
            {
                Console.WriteLine("{0}) {1} {2}",i,dev.Name,dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse(Console.ReadLine());

            var device1 = devices[i];
            var device2 = CaptureDeviceList.New()[i];

            // register our handler function to the 'packet arrival' event
            device1.OnPacketArrival +=
                new PacketArrivalEventHandler(device_OnPacketArrival);
            device2.OnPacketArrival +=
                new PacketArrivalEventHandler(device_OnPacketArrival);

            // open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device1.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device2.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            // tcpdump filter to capture only TCP/IP packets
            device1.Filter = "ether dst FF:FF:FF:FF:FF:FF and udp";
            device2.Filter = "ether dst FF:FF:FF:FF:FF:FF and ether proto 0x0842";

            Console.WriteLine();
            Console.WriteLine("-- Listening for packets... Hit 'Ctrl-C' to exit --");

            // start capture packets
            device1.Capture();
            device2.Capture();

            // close the pcap device
            // note: this line will never be called since
            //  we're capturing infinite number of packets
            device1.Close();
            device2.Close();
        }

        /// <summary>
        /// Handle incoming packets
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var len = e.Packet.Data.Length;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                time.Hour, time.Minute, time.Second, time.Millisecond, len);

            // parse the incoming packet
            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            if(packet == null)
                return;

            var wol = PacketDotNet.WakeOnLanPacket.GetEncapsulated(packet);
            if(wol.PayloadData != null)
            {
                PrintHex(wol.DestinationMAC.GetAddressBytes());
            }
        }

        /// <summary>
        /// Prints the MAC of the WOL packet
        /// </summary>
        private static void PrintHex(byte[] hexArr)
        {
            Console.Write("WOL Packet detected for: ");
            for(int i = 0; i<hexArr.Length; i++)
            {
                // display the physical address in hexadecimal
                Console.Write("{0}", hexArr[i].ToString("X2"));
                // trim the last hyphen from the MAC
                if (i != hexArr.Length -1)
                {
                    Console.Write("-");
                }
            }
            Console.WriteLine();
        }
    }
}