using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SharpPcap.LibPcap
{
    public class CaptureQueue : IDisposable
    {
        /// The queue that the callback thread puts packets in
        private BlockingCollection<RawCapture> PacketQueue = [];

        /// The timeout if the queue is full
        private readonly Int32 MillisecondsTimeout = 0;

        /// The bounded capacity of the queue
        private readonly int BoundedCapacity = 100;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="boundedCapacity"></param>
        public CaptureQueue(Int32 millisecondsTimeout, int boundedCapacity)
        {
            MillisecondsTimeout = millisecondsTimeout;
            BoundedCapacity = boundedCapacity;
            PacketQueue = new BlockingCollection<RawCapture>(BoundedCapacity);
        }

        /// Multiple device can register to the callback
        public void RegisterDevice(ICaptureDevice device)
        {
            // For this device, register the handler function to the 'packet arrival' event
            device.OnPacketArrival += device_OnPacketArrival;
        }

        /// The famous OnPacketArrival callback
        private void device_OnPacketArrival(object sender, PacketCapture e)
        {
            PacketQueue.TryAdd(e.GetPacket(), MillisecondsTimeout);
        }

        /// Checks for queued packets. If any exist it saves a
        /// reference of the current queue for itself and puts a new queue back into
        /// place into PacketQueue. The caller can then process queue that it saved without holding
        /// the queue lock.
        public void FlushCaptureQueue(out List<RawCapture> CaptureQueue)
        {
            CaptureQueue = [];
            
            if (PacketQueue.Count > 0)
            {
                // swap queues, giving the capture callback a new one
                CaptureQueue = [.. PacketQueue];
                PacketQueue = new BlockingCollection<RawCapture>(BoundedCapacity);
            }
        }

        /// Destroy the capture queue
        public void Dispose()
        {
            // Clear the queue
            PacketQueue.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}