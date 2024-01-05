// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
// Copyright 2008-2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SharpPcap.LibPcap
{
    public partial class PcapDevice
    {
        /// <summary>
        /// Thread that is performing the background packet capture
        /// </summary>
        protected Task captureThread;

        /// <summary>
        /// Flag that indicates that a capture thread should stop
        /// </summary>
        protected CancellationTokenSource threadCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Return a value indicating if the capturing process of this adapter is started
        /// </summary>
        public virtual bool Started
        {
            get { return (captureThread != null); }
        }

        /// <summary>
        /// Maximum time within which the capture thread must join the main thread (on 
        /// <see cref="StopCapture"/>) or else the thread is aborted and an exception thrown.
        /// </summary>
        public TimeSpan StopCaptureTimeout { get; set; } = new TimeSpan(0, 0, 1);

        /// <summary>
        /// Starts the capturing process via a background thread
        /// OnPacketArrival() will be called for each captured packet
        /// </summary>
        public virtual void StartCapture()
        {
            if (!Started)
            {
                if (!Opened)
                    throw new DeviceNotReadyException("Can't start capture, the pcap device is not opened.");

                if (OnPacketArrival == null)
                    throw new DeviceNotReadyException("No delegates assigned to OnPacketArrival, no where for captured packets to go.");

                var cancellationToken = threadCancellationTokenSource.Token;
                captureThread = Task.Run(() => CaptureThread(cancellationToken), cancellationToken);
            }
        }

        /// <summary>
        /// Stops the capture process
        ///
        /// Throws an exception if the stop capture timeout is exceeded and the
        /// capture thread was aborted
        /// </summary>
        public virtual void StopCapture()
        {
            if (Started)
            {
                threadCancellationTokenSource.Cancel();
                threadCancellationTokenSource = new CancellationTokenSource();
                LibPcapSafeNativeMethods.pcap_breakloop(Handle);
                Task.WaitAny(new[] { captureThread }, StopCaptureTimeout);
                captureThread = null;
            }
        }

        /// <summary>
        /// Synchronously capture packets on this device. Method blocks forever.
        /// </summary>
        public void Capture()
        {
            Capture(Pcap.InfinitePacketCount);
        }

        /// <summary>
        /// Synchronously captures packets on this network device. This method will block
        /// until capturing is finished.
        /// </summary>
        /// <param name="packetCount">The number of packets to be captured.
        /// -1 means capture indefiniately</param>
        public void Capture(int packetCount)
        {
            m_pcapPacketCount = packetCount;
            CaptureThread(threadCancellationTokenSource.Token);

            // restore the capture count incase the user Starts
            m_pcapPacketCount = Pcap.InfinitePacketCount;
        }

        /// <summary>
        /// unix specific code, we want to poll for packets
        /// otherwise if we call pcap_dispatch() the read() will block
        /// and won't resume until a packet arrives OR until a signal
        /// occurs
        /// </summary>
        /// <param name="timeout">
        /// Timeout chosen to allow the capture thread to loop frequently enough
        /// to enable it to properly exit when the user requests it to but
        /// infrequently enough to cause any noticable performance overhead
        /// </param>
        /// <returns>true if poll was successfull and we have data to read, false otherwise</returns>
        protected internal bool PollFileDescriptor(int timeout = 500)
        {
            if (FileDescriptor < 0)
            {
                // Either this is a File Capture, or Windows
                // Assume we have data to read
                return true;
            }
            var pollFds = new Posix.Pollfd[1];
            pollFds[0].fd = FileDescriptor;
            pollFds[0].events = Posix.PollEvents.POLLPRI | Posix.PollEvents.POLLIN;

            var result = Posix.Poll(pollFds, (uint)pollFds.Length, timeout);

            // if we have no poll results, we don't have anything to read
            // -1 means error
            // 0 means timeout
            // non-negative means we got something
            return result != 0;
        }

        /// <summary>
        /// The capture thread
        /// </summary>
        protected virtual void CaptureThread(CancellationToken cancellationToken)
        {
            if (!Opened)
                throw new DeviceNotReadyException("Capture called before PcapDevice.Open()");

            var Callback = new LibPcapSafeNativeMethods.pcap_handler(PacketHandler);
            var handle = Handle;
            var gotRef = false;
            try
            {
                // Make sure that handle does not get closed until this function is done
                handle.DangerousAddRef(ref gotRef);
                if (!gotRef)
                {
                    return;
                }
                while (!cancellationToken.IsCancellationRequested)
                {
                    // TODO: This check can be removed once libpcap versions >= 1.10 has become in widespread use.
                    // libpcap 1.10 improves pcap_dispatch() to break out when pcap_breakloop() across threads
                    if (!PollFileDescriptor())
                    {
                        // We don't have data to read, don't call pcap_dispatch() yet
                        continue;
                    }

                    int res = LibPcapSafeNativeMethods.pcap_dispatch(handle, m_pcapPacketCount, Callback, handle.DangerousGetHandle());

                    // pcap_dispatch() returns the number of packets read or, a status value if the value
                    // is negative
                    if (res <= 0)
                    {
                        switch (res)    // Check pcap loop status results and notify upstream.
                        {
                            case Pcap.LOOP_USER_TERMINATED:     // User requsted loop termination with StopCapture()
                                SendCaptureStoppedEvent(CaptureStoppedEventStatus.CompletedWithoutError);
                                return;
                            case Pcap.LOOP_COUNT_EXHAUSTED:     // m_pcapPacketCount exceeded (successful exit)
                                {
                                    // NOTE: pcap_dispatch() returns 0 when a timeout occurrs so to prevent timeouts
                                    //       from causing premature exiting from the capture loop we only consider
                                    //       exhausted events to cause an escape from the loop when they are from
                                    //       offline devices, ie. files read from disk
                                    if (this is CaptureReaderDevice)
                                    {
                                        SendCaptureStoppedEvent(CaptureStoppedEventStatus.CompletedWithoutError);
                                        return;
                                    }
                                    break;
                                }
                            case Pcap.LOOP_EXIT_WITH_ERROR:     // An error occurred whilst capturing.
                                SendCaptureStoppedEvent(CaptureStoppedEventStatus.ErrorWhileCapturing);
                                return;
                            default:
                                // This can only be triggered by a bug in libpcap.
                                // We can't throw here, sicne that would crash the application
                                Trace.TraceError($"SharpPcap: Unknown pcap_loop exit status: {res}");
                                SendCaptureStoppedEvent(CaptureStoppedEventStatus.ErrorWhileCapturing);
                                return;
                        }
                    }
                    else // res > 0
                    {
                        // if we aren't capturing infinitely we need to account for
                        // the packets that we read
                        if (m_pcapPacketCount != Pcap.InfinitePacketCount)
                        {
                            // take away for the packets read
                            if (m_pcapPacketCount >= res)
                                m_pcapPacketCount -= res;
                            else
                                m_pcapPacketCount = 0;

                            // no more packets to capture, we are finished capturing
                            if (m_pcapPacketCount == 0)
                            {
                                SendCaptureStoppedEvent(CaptureStoppedEventStatus.CompletedWithoutError);
                                return;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (gotRef)
                {
                    handle.DangerousRelease();
                }
            }
            SendCaptureStoppedEvent(CaptureStoppedEventStatus.CompletedWithoutError);
        }
    }
}
