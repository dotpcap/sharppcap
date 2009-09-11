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
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 * Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
 */

using System;
using System.Threading;

namespace SharpPcap
{
    public partial class PcapDevice
    {
        private Thread captureThread;
        private bool shouldCaptureThreadStop;

        /// <summary>
        /// Return a value indicating if the capturing process of this adapter is started
        /// </summary>
        public virtual bool Started
        {
            get{ return (captureThread != null); }
        }

        /// <summary>
        /// Starts the capturing process
        /// </summary>
        public virtual void StartCapture()
        {
            if (!Started)
            {
                if (!Opened)
                    throw new PcapDeviceNotReadyException("Can't start capture, the pcap device is not opened.");

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
            TimeSpan joinTimeout = new TimeSpan(0, 0, 1);
            if (Started)
            {
                shouldCaptureThreadStop = true;
                if(!captureThread.Join(joinTimeout))
                {
                    captureThread.Abort();
                    captureThread = null;
                    throw new PcapException("captureThread was aborted after " + joinTimeout.ToString());
                }

                captureThread = null; // otherwise we will always return true from PcapDevice.Started
            }
        }

        /// <summary>
        /// Captures packets on this network device. This method will block
        /// until capturing is finished.
        /// </summary>
        /// <param name="packetCount">The number of packets to be captured. 
        /// Value of '-1' means infinite.</param>
        public void Capture(int packetCount)
        {
            m_pcapPacketCount = packetCount;
            CaptureThread();
            m_pcapPacketCount = Pcap.INFINITE;;
        }

        private void CaptureThread()
        {
            if (!Opened)
                throw new PcapDeviceNotReadyException("Capture called before PcapDevice.Open()");

            SafeNativeMethods.pcap_handler Callback = new SafeNativeMethods.pcap_handler(PacketHandler);

            while(!shouldCaptureThreadStop)
            {
                int res = SafeNativeMethods.pcap_dispatch(PcapHandle, m_pcapPacketCount, Callback, IntPtr.Zero);

                // pcap_dispatch() returns the number of packets read or, a status value if the value
                // is negative
                if(res <= 0)
                {
                    switch (res)    // Check pcap loop status results and notify upstream.
                    {
                        case Pcap.LOOP_USER_TERMINATED:     // User requsted loop termination with StopCapture()
                            SendCaptureStoppedEvent(false);
                            return;
                        case Pcap.LOOP_COUNT_EXHAUSTED:     // m_pcapPacketCount exceeded (successful exit)
                        {
                            // NOTE: pcap_dispatch() returns 0 when a timeout occurrs so to prevent timeouts
                            //       from causing premature exiting from the capture loop we only consider
                            //       exhausted events to cause an escape from the loop when they are from
                            //       offline devices, ie. files read from disk
                            if(this is PcapOfflineDevice)
                            {
                                SendCaptureStoppedEvent(false);
                                return;
                            }
                            break;
                        }
                        case Pcap.LOOP_EXIT_WITH_ERROR:     // An error occoured whilst capturing.
                            SendCaptureStoppedEvent(true);
                            return;
                        default:    // This can only be triggered by a bug in libpcap.
                            throw new PcapException("Unknown pcap_loop exit status.");
                    }
                }
            }

            SendCaptureStoppedEvent(false);
        }
    }
}
