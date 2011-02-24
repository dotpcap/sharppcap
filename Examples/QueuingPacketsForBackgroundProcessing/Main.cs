using System;
using System.Collections.Generic;
using SharpPcap;

namespace QueuingPacketsForBackgroundProcessing
{
    /// <summary>
    /// Basic capture example showing simple queueing for background processing
    /// </summary>
    public class MainClass
    {
        /// <summary>
        /// When true the background thread will terminate
        /// </summary>
        /// <param name="args">
        /// A <see cref="System.String"/>
        /// </param>
        private static bool BackgroundThreadStop = false;

        /// <summary>
        /// Object that is used to prevent two threads from accessing
        /// PacketQueue at the same time
        /// </summary>
        /// <param name="args">
        /// A <see cref="System.String"/>
        /// </param>
        private static object QueueLock = new object();

        /// <summary>
        /// The queue that the callback thread puts packets in. Accessed by
        /// the background thread when QueueLock is held
        /// </summary>
        private static List<RawCapture> PacketQueue = new List<RawCapture>();

        /// <summary>
        /// The last time PcapDevice.Statistics() was called on the active device.
        /// Allow periodic display of device statistics
        /// </summary>
        /// <param name="args">
        /// A <see cref="System.String"/>
        /// </param>
        private static DateTime LastStatisticsOutput = DateTime.Now;

        /// <summary>
        /// Interval between PcapDevice.Statistics() output
        /// </summary>
        /// <param name="args">
        /// A <see cref="System.String"/>
        /// </param>
        private static TimeSpan LastStatisticsInterval = new TimeSpan(0, 0, 2);

        /// <summary>
        /// Basic capture example
        /// </summary>
        public static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}", ver);

            // If no device exists, print error
            if(CaptureDeviceList.Instance.Count < 1)
            {
                Console.WriteLine("No device found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i=0;

            // Print out all devices
            foreach(var dev in CaptureDeviceList.Instance)
            {
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse( Console.ReadLine() );

            // start the background thread
            var backgroundThread = new System.Threading.Thread(BackgroundThread);
            backgroundThread.Start();

            var device = CaptureDeviceList.Instance[i];

            // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += 
                new PacketArrivalEventHandler( device_OnPacketArrival );

            // Open the device for capturing
            device.Open();

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0}, hit 'Enter' to stop...",
                device.Description);

            // Start the capturing process
            device.StartCapture();

            // Wait for 'Enter' from the user. We pause here until being asked to
            // be terminated
            Console.ReadLine();

            // Stop the capturing process
            device.StopCapture();

            Console.WriteLine("-- Capture stopped.");

            // ask the background thread to shut down
            BackgroundThreadStop = true;

            // wait for the background thread to terminate
            backgroundThread.Join();

            // Print out the device statistics
            Console.WriteLine(device.Statistics.ToString());

            // Close the pcap device
            device.Close();
        }

        /// <summary>
        /// Prints the time and length of each received packet
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            // print out periodic statistics about this device
            var Now = DateTime.Now; // cache 'DateTime.Now' for minor reduction in cpu overhead
            var interval = Now - LastStatisticsOutput;
            if(interval > LastStatisticsInterval)
            {
                Console.WriteLine("device_OnPacketArrival: " + e.Device.Statistics);
                LastStatisticsOutput = Now;
            }

            // lock QueueLock to prevent multiple threads accessing PacketQueue at
            // the same time
            lock(QueueLock)
            {
                PacketQueue.Add(e.Packet);
            }
        }

        /// <summary>
        /// Checks for queued packets. If any exist it locks the QueueLock, saves a
        /// reference of the current queue for itself, puts a new queue back into
        /// place into PacketQueue and unlocks QueueLock. This is a minimal amount of
        /// work done while the queue is locked.
        ///
        /// The background thread can then process queue that it saved without holding
        /// the queue lock.
        /// </summary>
        private static void BackgroundThread()
        {
            while(!BackgroundThreadStop)
            {
                bool shouldSleep = true;

                lock(QueueLock)
                {
                    if(PacketQueue.Count != 0)
                    {
                        shouldSleep = false;
                    }
                }

                if(shouldSleep)
                {
                    System.Threading.Thread.Sleep(250);
                } else // should process the queue
                {
                    List<RawCapture> ourQueue;
                    lock(QueueLock)
                    {
                        // swap queues, giving the capture callback a new one
                        ourQueue = PacketQueue;
                        PacketQueue = new List<RawCapture>();
                    }

                    Console.WriteLine("BackgroundThread: ourQueue.Count is {0}", ourQueue.Count);

                    foreach(var packet in ourQueue)
                    {
                        var time = packet.Timeval.Date;
                        var len = packet.Data.Length;
                        Console.WriteLine("BackgroundThread: {0}:{1}:{2},{3} Len={4}", 
                            time.Hour, time.Minute, time.Second, time.Millisecond, len);
                    }

                    // Here is where we can process our packets freely without
                    // holding off packet capture.
                    //
                    // NOTE: If the incoming packet rate is greater than
                    //       the packet processing rate these queues will grow
                    //       to enormous sizes. Packets should be dropped in these
                    //       cases
                }
            }
        }
    }
}
