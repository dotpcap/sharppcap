// Copyright 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

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