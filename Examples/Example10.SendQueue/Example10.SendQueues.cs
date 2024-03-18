// SPDX-License-Identifier: MIT

using System;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Example10
{
    /// <summary>
    /// Example using send queues
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Basic capture example
        /// </summary>
        public static void Main(string[] args)
        {
            // Print SharpPcap version
            var ver = Pcap.SharpPcapVersion;
            Console.WriteLine("SharpPcap {0}, Example10.SendQueues.cs\n", ver);

            Console.Write("-- Please enter an input capture file name: ");
            string capFile = Console.ReadLine();

            ICaptureDevice device;

            try
            {
                // Get an offline file pcap device
                device = new CaptureFileReaderDevice(capFile);

                // Open the device for capturing
                device.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.Write("Queueing packets...");

            //Allocate a new send queue
            var squeue = new SharpPcap.LibPcap.SendQueue
                ((int)((CaptureFileReaderDevice)device).FileSize);
            RawCapture packet;
            PacketCapture e;
            GetPacketStatus retval;

            try
            {
                //Go through all packets in the file and add to the queue
                while ((retval = device.GetNextPacket(out e)) == GetPacketStatus.PacketRead)
                {
                    packet = e.GetPacket();
                    if (!squeue.Add(packet))
                    {
                        Console.WriteLine("Warning: packet buffer too small, " +
                            "not all the packets will be sent.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("OK");

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            var devices = LibPcapLiveDeviceList.Instance;
            /* Scan the list printing every entry */
            foreach (var dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to transmit on: ");
            i = int.Parse(Console.ReadLine());
            devices[i].Open();
            string resp;

            if (devices[i].LinkType != device.LinkType)
            {
                Console.Write("Warning: the datalink of the capture" +
                    " differs from the one of the selected interface, continue? [YES|no]");
                resp = Console.ReadLine().ToLower();

                if ((resp != "") && (!resp.StartsWith("y")))
                {
                    Console.WriteLine("Cancelled by user!");
                    devices[i].Close();
                    return;
                }
            }

            // close the offline device
            device.Close();

            // find the network device for sending the packets we read
            device = devices[i];

            Console.Write("This will transmit all queued packets through" +
                " this device, continue? [YES|no]");
            resp = Console.ReadLine().ToLower();

            if ((resp != "") && (!resp.StartsWith("y")))
            {
                Console.WriteLine("Cancelled by user!");
                return;
            }

            try
            {
                var liveDevice = device as LibPcapLiveDevice;

                Console.Write("Sending packets...");
                int sent = squeue.Transmit(liveDevice, SendQueueTransmitModes.Synchronized);
                Console.WriteLine("Done!");
                if (sent < squeue.CurrentLength)
                {
                    Console.WriteLine("An error occurred sending the packets: {0}. " +
                        "Only {1} bytes were sent\n", device.LastError, sent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            //Free the queue
            squeue.Dispose();
            Console.WriteLine("-- Queue is disposed.");
            //Close the pcap device
            device.Close();
            Console.WriteLine("-- Device closed.");
            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }
    }
}
