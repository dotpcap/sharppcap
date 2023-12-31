// SPDX-License-Identifier: MIT

using System;
using System.Net;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NpcapRemoteCapture
{
    /// <summary>
    /// Example showing how to use the remote capture feature of Npcap
    /// </summary>
    public class Program
    {
        private static readonly int rpcapDefaultPort = 2002;

        static void PrintUsage(string programName)
        {
            Console.WriteLine("{0} ipAddress [port] - port defaults to {1} (rpcapd default port)", programName, rpcapDefaultPort);
        }

        static void Main(string[] args)
        {
            if ((args.Length < 1) || (args.Length > 2))
            {
                PrintUsage("NpcapRemoteCapture");
                return;
            }

            // ensure that a remote capture daemon has been started by running
            // 'rpcapd.exe' on the remote server. By default port 2003 is used for the server

            var ipAddress = System.Net.IPAddress.Parse(args[0]);
            var port = rpcapDefaultPort;
            if (args.Length == 2)
                port = Int32.Parse(args[1]);

            var remoteInterfaces = PcapInterface.GetAllPcapInterfaces(new IPEndPoint(ipAddress, port), null);
            foreach (var dev in remoteInterfaces)
            {
                Console.WriteLine("device: {0}", dev.ToString());
            }

            // open the device for capture
            using var device = new LibPcapLiveDevice(remoteInterfaces[0]);

            device.OnPacketArrival += new PacketArrivalEventHandler(dev_OnPacketArrival);

            device.Open(new DeviceConfiguration { ReadTimeout = 500 });

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0}, hit 'Enter' to stop...",
                device.Description);

            // Start the capturing process
            device.StartCapture();

            // Wait for 'Enter' from the user.
            Console.ReadLine();

            // Stop the capturing process
            device.StopCapture();

            Console.WriteLine("-- Capture stopped.");

            // Print out the device statistics
            Console.WriteLine(device.Statistics.ToString());
        }

        static void dev_OnPacketArrival(object sender, SharpPcap.PacketCapture e)
        {
            var time = e.Header.Timeval.Date;
            var len = e.Data.Length;
            var packet = e.GetPacket();
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(packet.ToString());
        }
    }
}
