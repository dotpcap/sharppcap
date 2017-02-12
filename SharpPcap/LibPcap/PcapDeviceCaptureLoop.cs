// Define that lets us test behavior using Mono.Unix.Native directly avoiding
// reflection. Useful for debugging but we don't want to ship an assembly that
// depends on Mono.Unix.Native because many users are on MS .Net which lacks this
// assembly
//#define UseMonoUnixNativeDirectly

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
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2008-2010 Chris Morgan <chmorgan@gmail.com>
 * Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
 */

using System;
using System.Threading;
using System.Reflection;
#if UseMonoUnixNativeDirectly
using Mono.Unix.Native;
#endif

// NOTE: The reflection code in here may seem complex but it serves an important
//       purpose.
//
//       Under Unix, pcap_loop(), pcap_dispatch(), pcap_next() and pcap_next_ex()
//       all perform blocking read() calls at the os level that have NO timeout.
//       If the user wishes to stop capturing on an adapter they will need to wait
//       until the next packet arrives for the capture loop to wake up and look to see
//       if it has been asked to shut down. This may be never in the case of inactive
//       adapters or far longer than what the user desires.
//
//       So, to avoid the issue we use reflection to load up Mono.Posix and invoke
//       the poll() system call. The thread sleeps on the poll() and only when woken
//       up and indicating that data is available do we call one of the pcap
//       data retrieval routines. This is how we avoid blocking for long periods
//       or forever.
//
//       Poll enables us to set a timeout. The timeout is chosen to be long
//       enough to avoid a noticable performance impact from frequent looping
//       but short enough to satisify the timing constraints of the Thread.Join() in
//       the stop capture code.
//

namespace SharpPcap.LibPcap
{
    public partial class PcapDevice
    {
        /// <summary>
        /// Thread that is performing the background packet capture
        /// </summary>
        protected Thread captureThread;

        /// <summary>
        /// Flag that indicates that a capture thread should stop
        /// </summary>
        protected bool shouldCaptureThreadStop;

        /// <summary>
        /// If Environment.OSVersion.Platform is unix and MonoUnixFound is true
        /// then we can support proper termination of the capture loop
        /// </summary>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        private static bool MonoUnixFound = false;

        // if true then we are running under a unix platform so we must be using libpcap
        private static bool isLibPcap = (Environment.OSVersion.Platform == PlatformID.Unix);

#if !UseMonoUnixNativeDirectly
        // variables for unix dynamic invocation of Mono.Unix.Native.Syscall.poll()
        private static Assembly MonoUnixNativeAssembly;
        private static Type SyscallType;
        private static Type PollfdType;
        private static Type PollEventsType;

        // values of the PollEvents.POLLPRI and POLLIN enum
        private static short POLLPRI;
        private static short POLLIN;

        /// <summary>
        /// Setup the reflection type and methodinfo for invocation of
        /// Mono.Unix.Native.Syscall.poll() to avoid timeouts when
        /// stopping the capture thread
        /// </summary>
        private static bool UnixSetupMonoUnixNative()
        {
            // load the assemly
            var AssemblyName = "Mono.Posix, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756";

            try
            {
                MonoUnixNativeAssembly = Assembly.Load(AssemblyName);
            } catch(System.Exception)
            {
                // unable to load the Mono.Posix assembly so we can't
                // avoid blocking forever in the capture loop
                return false;
            }

            SyscallType = MonoUnixNativeAssembly.GetType("Mono.Unix.Native.Syscall");
            if(SyscallType == null)
            {
                throw new System.InvalidOperationException("SyscallType is null");
            }

            PollfdType = MonoUnixNativeAssembly.GetType("Mono.Unix.Native.Pollfd");
            if(PollfdType == null)
            {
                throw new System.InvalidOperationException("PollfdType is null");
            }

            PollEventsType = MonoUnixNativeAssembly.GetType("Mono.Unix.Native.PollEvents");
            if(PollEventsType == null)
            {
                throw new System.InvalidOperationException("PollEventsType is null");
            }

            // retrieve the pollpri and pollin values
            FieldInfo field;
            field = PollEventsType.GetField("POLLPRI");
            POLLPRI = (short)field.GetValue(PollEventsType);

            field = PollEventsType.GetField("POLLIN");
            POLLIN = (short)field.GetRawConstantValue();

            return true;
        }

        static PcapDevice()
        {
            // if we are running under libpcap and mono then we should
            // setup the Mono.Unix.Native methods
            if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                MonoUnixFound = UnixSetupMonoUnixNative();
            }
        }
#endif

        /// <summary>
        /// Return a value indicating if the capturing process of this adapter is started
        /// </summary>
        public virtual bool Started
        {
            get { return (captureThread != null); }
        }

        // time we give the capture thread to stop before we assume that
        // there is an error
        private TimeSpan stopCaptureTimeout = new TimeSpan(0, 0, 1);

        /// <summary>
        /// Maximum time within which the capture thread must join the main thread (on 
        /// <see cref="StopCapture"/>) or else the thread is aborted and an exception thrown.
        /// </summary>
        public TimeSpan StopCaptureTimeout
        {
            get { return stopCaptureTimeout; }
            set { stopCaptureTimeout = value; }
        }

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

                if ( OnPacketArrival == null)
                    throw new DeviceNotReadyException("No delegates assigned to OnPacketArrival, no where for captured packets to go.");

                shouldCaptureThreadStop = false;
                captureThread = new Thread(new ThreadStart(this.CaptureThread));
                captureThread.Start();
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
                shouldCaptureThreadStop = true;
                if(!captureThread.Join(StopCaptureTimeout))
                {
                    captureThread.Abort();
                    captureThread = null;
                    string error;

                    if(isLibPcap && !MonoUnixFound)
                    {
                        error = string.Format("captureThread was aborted after {0}. Using a Mono" +
                                              " version >= 2.4 and installing Mono.Posix should" +
                                              " enable smooth thread shutdown",
                                              StopCaptureTimeout.ToString());
                    } else
                    {
                        error = string.Format("captureThread was aborted after {0}",
                                              StopCaptureTimeout.ToString());
                    }

                    throw new PcapException(error);
                }

                captureThread = null; // otherwise we will always return true from PcapDevice.Started
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
            CaptureThread();

            // restore the capture count incase the user Starts
            m_pcapPacketCount = Pcap.InfinitePacketCount;
        }

        /// <summary>
        /// The capture thread
        /// </summary>
        protected virtual void CaptureThread()
        {
            if (!Opened)
                throw new DeviceNotReadyException("Capture called before PcapDevice.Open()");

            var usePoll = (this is LibPcapLiveDevice) &&
                          isLibPcap && MonoUnixFound;

            // unix specific code
            int captureFileDescriptor = 0;
            if(usePoll)
            {
                // retrieve the file descriptor of the adapter for use with poll()
                captureFileDescriptor = LibPcapSafeNativeMethods.pcap_fileno(PcapHandle);
                if(captureFileDescriptor == -1)
                {
                    SendCaptureStoppedEvent(CaptureStoppedEventStatus.ErrorWhileCapturing);
                    return;
                }
            }

            LibPcapSafeNativeMethods.pcap_handler Callback = new LibPcapSafeNativeMethods.pcap_handler(PacketHandler);

            // unix specific code
#if UseMonoUnixNativeDirectly
            Pollfd[] pollFds = new Pollfd[1];
#else
            System.Array pollFds = null;
            object[] PollParameters = null;
#endif

            // Timeout chosen to allow the capture thread to loop frequently enough
            // to enable it to properly exit when the user requests it to but
            // infrequently enough to cause any noticable performance overhead
            int millisecondTimeout = 500;

            if(usePoll)
            {
#if UseMonoUnixNativeDirectly
                pollFds[0].fd = captureFileDescriptor;
                pollFds[0].events = PollEvents.POLLPRI | Mono.Unix.Native.PollEvents.POLLIN;
#else
                FieldInfo field;
                pollFds = Array.CreateInstance(PollfdType, 1);

                // create a PollFd struct instance
                var pollFd = Activator.CreateInstance(PollfdType);

                // set the descriptor field
                field = PollfdType.GetField("fd");
                field.SetValue(pollFd, captureFileDescriptor);

                // set the events field
                short eventValue = (short)(POLLIN | POLLPRI); // mask the two together
                field = PollfdType.GetField("events");
                field.SetValue(pollFd, eventValue);

                // set the Pollfd entry
                pollFds.SetValue(pollFd, 0);

                // setup the parameters we will pass to the poll() method
                PollParameters = new object[2];
                PollParameters[0] = pollFds;
                PollParameters[1] = millisecondTimeout;
#endif
            }

            while(!shouldCaptureThreadStop)
            {
                // unix specific code, we want to poll for packets
                // otherwise if we call pcap_dispatch() the read() will block
                // and won't resume until a packet arrives OR until a signal
                // occurs
                if(usePoll)
                {
                    // block here
#if UseMonoUnixNativeDirectly
                    var result = Mono.Unix.Native.Syscall.poll(pollFds, millisecondTimeout);
#else
                    object o = SyscallType.InvokeMember("poll",
                                                        BindingFlags.InvokeMethod,
                                                        Type.DefaultBinder,
                                                        null,
                                                        PollParameters);
                    int result = (int)o;
#endif

                    // if we have no poll results, just loop
                    if(result <= 0)
                    {
                        continue;
                    }

                    // fall through here to the pcap_dispatch() call
                }

                int res = LibPcapSafeNativeMethods.pcap_dispatch(PcapHandle, m_pcapPacketCount, Callback, IntPtr.Zero);

                // pcap_dispatch() returns the number of packets read or, a status value if the value
                // is negative
                if(res <= 0)
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
                            if(this is CaptureFileReaderDevice)
                            {
                                SendCaptureStoppedEvent(CaptureStoppedEventStatus.CompletedWithoutError);
                                return;
                            }
                            break;
                        }
                        case Pcap.LOOP_EXIT_WITH_ERROR:     // An error occurred whilst capturing.
                            SendCaptureStoppedEvent(CaptureStoppedEventStatus.CompletedWithoutError);
                            return;
                        default:    // This can only be triggered by a bug in libpcap.
                            throw new PcapException("Unknown pcap_loop exit status.");
                    }
                } else // res > 0
                {
                    // if we aren't capturing infinitely we need to account for
                    // the packets that we read
                    if(m_pcapPacketCount != Pcap.InfinitePacketCount)
                    {
                        // take away for the packets read
                        if(m_pcapPacketCount >= res)
                            m_pcapPacketCount -= res;
                        else
                            m_pcapPacketCount = 0;

                        // no more packets to capture, we are finished capturing
                        if(m_pcapPacketCount == 0)
                        {
                            SendCaptureStoppedEvent(CaptureStoppedEventStatus.CompletedWithoutError);
                            return;
                        }
                    }
                }
            }

            SendCaptureStoppedEvent(CaptureStoppedEventStatus.CompletedWithoutError);
        }
    }
}
