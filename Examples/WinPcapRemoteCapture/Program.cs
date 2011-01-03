using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap.WinPcap;

namespace WinPcapRemoteCapture
{
    /// <summary>
    /// Example showing how to use the remote capture feature of WinPcap
    /// </summary>
    class Program
    {
        private static int rpcapDefaultPort = 2002;

        static void PrintUsage(string programName)
        {
            Console.WriteLine("{0} ipAddress [port] - port defaults to {1} (rpcapd default port)", programName, rpcapDefaultPort);
        }

        static void Main(string[] args)
        {
            if ((args.Length < 1) || (args.Length > 2))
            {
                PrintUsage("WinPcapRemoteCapture");
                return;
            }

            // ensure that a remote capture daemon has been started by running
            // 'rpcapd.exe' on the remote server. By default port 2003 is used for the server

            var ipAddress = System.Net.IPAddress.Parse(args[0]);
            var port = rpcapDefaultPort;
            if(args.Length == 2)
                port = Int32.Parse(args[1]);

            var remoteDevices = WinPcapDeviceList.Devices(ipAddress, port, null);
            foreach (var dev in remoteDevices)
            {
                Console.WriteLine("device: {0}", dev.ToString());
            }

            // open the device for capture
            var device = remoteDevices[0];

            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(dev_OnPacketArrival);

            device.Open(0, 500, null);

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

            // Close the pcap device
            device.Close();
        }

        static void dev_OnPacketArrival(object sender, SharpPcap.CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var len = e.Packet.Data.Length;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(e.Packet.ToString());
        }
    }
}
