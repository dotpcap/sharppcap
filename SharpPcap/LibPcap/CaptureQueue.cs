using System;
using System.Collections.Generic;
using SharpPcap;
using SharpPcap.LibPcap;

namespace SharpPcap.LibPcap
{
    public class CaptureQueue
    {
        /// Object that is used to prevent concurrent writting or partial reading of PacketQueue
        private static readonly object QueueLock = new object();

        /// The queue that the callback thread puts packets in
        private static List<RawCapture> PacketQueue = new List<RawCapture>();

        /// Creation of the capture queue
        public static void CreateCaptureQueue(LibPcapLiveDevice device)
        {
            // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
        }

        /// The famous OnPacketArrival callback
        private static void device_OnPacketArrival(object sender, PacketCapture e)
        {
            // lock PacketQueue
            lock (QueueLock)
            {
                PacketQueue.Add(e.GetPacket());
            }
        }

        /// Checks for queued packets. If any exist it locks the QueueLock, saves a
        /// reference of the current queue for itself, puts a new queue back into
        /// place into PacketQueue and unlocks QueueLock. This is a minimal amount of
        /// work done while the queue is locked. The caller can then process queue that it saved without holding
        /// the queue lock.
        public static void FlushCaptureQueue(out List<RawCapture> CaptureQueue)
        {
            CaptureQueue = new List<RawCapture>();
            
            // lock PacketQueue
            lock (QueueLock)
            {
                if (PacketQueue.Count != 0)
                {
                    // swap queues, giving the capture callback a new one
                    CaptureQueue = PacketQueue;
                    PacketQueue = new List<RawCapture>();
                }
            }
        }

        /// Destroy the capture queue
        public static void DestroyCaptureQueue()
        {
            lock (QueueLock)
            {
                // Clear the queue
                PacketQueue.Clear();
            }
        }
    }
}