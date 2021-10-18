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
 * Copyright 2021 Ayoub Kaanich <kayoub5@live.com>
 */

using PacketDotNet;
using SharpPcap.LibPcap;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SharpPcap
{

    /// <summary>
    /// Helper base class for non Libpcap based devices
    /// </summary>
    public abstract class BaseLiveDevice : IDisposable
    {

        private CancellationTokenSource TokenSource;
        private Task CaptureTask;
        public bool Started => CaptureTask?.IsCompleted == false;

        protected TimeSpan ReadTimeout { get; set; } = TimeSpan.FromSeconds(1);

        public TimeSpan StopCaptureTimeout { get; set; } = TimeSpan.FromSeconds(1);

        public ICaptureStatistics Statistics => null;

        public TimestampResolution TimestampResolution => TimestampResolution.Microsecond;

        public virtual LinkLayers LinkType => LinkLayers.Ethernet;

        public event PacketArrivalEventHandler OnPacketArrival;
        public event CaptureStoppedEventHandler OnCaptureStopped;

        protected BpfProgram FilterProgram;
        private string FilterValue;
        public string Filter
        {
            get => FilterValue;
            set
            {
                using (var pcapHandle = LibPcapSafeNativeMethods.pcap_open_dead((int)LinkType, Pcap.MAX_PACKET_SIZE))
                {
                    FilterProgram = BpfProgram.Create(pcapHandle, value);
                    FilterValue = value;
                }
            }
        }

        protected void RaiseOnPacketArrival(PacketCapture capture)
        {
            if (FilterProgram?.Matches(capture.Data) ?? true)
            {
                OnPacketArrival?.Invoke(this, capture);
            }
        }

        /// <summary>
        /// Retrieves the next packet from a device
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Status of the operation</returns>
        public GetPacketStatus GetNextPacket(out PacketCapture e)
        {
            var sw = Stopwatch.StartNew();
            var timeout = ReadTimeout - sw.Elapsed;
            while (timeout.TotalMilliseconds > 0)
            {
                var status = GetUnfilteredPacket(out e, timeout);
                if (FilterProgram?.Matches(e.Data) ?? true)
                {
                    return status;
                }
                timeout = ReadTimeout - sw.Elapsed;
            }
            e = default;
            return GetPacketStatus.ReadTimeout;
        }

        protected abstract GetPacketStatus GetUnfilteredPacket(out PacketCapture e, TimeSpan timeout);
        protected abstract void CaptureLoop(CancellationToken token);

        public void StopCapture()
        {
            TokenSource?.Cancel();
            TokenSource = null;
        }

        public void Capture()
        {
            CaptureLoop(default);
        }

        public void StartCapture()
        {
            if (OnPacketArrival == null)
            {
                throw new DeviceNotReadyException("No delegates assigned to OnPacketArrival, no where for captured packets to go.");
            }
            if (Started)
            {
                return;
            }
            CaptureTask = Task.Run(() =>
            {
                TokenSource?.Dispose();
                TokenSource = new CancellationTokenSource();
                CaptureLoop(TokenSource.Token);
                OnCaptureStopped?.Invoke(this, CaptureStoppedEventStatus.CompletedWithoutError);
            });
        }

        public virtual void Close()
        {
            StopCapture();
        }

        public void Dispose()
        {
            Close();
        }
    }
}