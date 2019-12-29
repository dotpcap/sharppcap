using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using PacketDotNet;

namespace SharpPcap.WinDivert
{
    public class WinDivertDevice : ICaptureDevice
    {

        static readonly DateTime BootTime = DateTime.Now - TimeSpan.FromTicks(Stopwatch.GetTimestamp());

        const int ERROR_INSUFFICIENT_BUFFER = 122;
        const int ERROR_NO_DATA = 232;

        protected IntPtr Handle;

        public string Name => "WinDivert";

        public string Description => "WinDivert Packet Driver";

        public LinkLayers LinkType => LinkLayers.Raw;

        public PhysicalAddress MacAddress => null;

        public ICaptureStatistics Statistics => null;

        public string LastError { get; private set; }

        private string filter;
        public string Filter
        {
            get
            {
                return string.IsNullOrEmpty(filter) ? "true" : filter;
            }
            set
            {
                var ret = WinDivertNative.WinDivertHelperCompileFilter(value, WinDivertLayer.Network, IntPtr.Zero, 0, out IntPtr errStrPtr, out uint errPos);
                if (!ret)
                {
                    var errStr = Marshal.PtrToStringAnsi(errStrPtr);
                    throw new PcapException($"Filter string is invalid at position {errPos}\n{errStr}");
                }
                filter = value;
            }
        }

        public void Close()
        {
            if (Handle != IntPtr.Zero)
            {
                if (!WinDivertNative.WinDivertClose(Handle))
                {
                    ThrowLastWin32Error("Failed to close device");
                }
                Handle = IntPtr.Zero;
            }
        }

        public RawCapture GetNextPacket()
        {
            ThrowIfNotOpen();
            Span<byte> buffer = stackalloc byte[4096];
            while (true)
            {
                bool ret;
                WinDivertAddress addr;
                uint readLen;
                unsafe
                {
                    fixed (byte* p = buffer)
                    {
                        ret = WinDivertNative.WinDivertRecv(Handle, new IntPtr(p), (uint)buffer.Length, out readLen, out addr);
                    }
                }
                if (!ret)
                {
                    var err = Marshal.GetLastWin32Error();
                    if (err == ERROR_INSUFFICIENT_BUFFER)
                    {
                        // Increase buffer size
                        buffer = stackalloc byte[buffer.Length * 2];
                        continue;
                    }
                    if (err == ERROR_NO_DATA)
                    {
                        return null;
                    }
                    ThrowWin32Error("Recv failed", err);
                }
                var timestamp = new PosixTimeval(BootTime + TimeSpan.FromTicks(addr.Timestamp));
                var data = buffer.Slice(0, (int)readLen).ToArray();
                var raw = new RawCapture(LinkType, timestamp, data);
                return raw;
            }
        }

        public int GetNextPacketPointers(ref IntPtr header, ref IntPtr data)
        {
            throw new NotSupportedException();
        }

        public void Open()
        {
            var handle = WinDivertNative.WinDivertOpen(Filter, WinDivertLayer.Network, 0, 1);
            if (handle == IntPtr.Zero || handle == new IntPtr(-1))
            {
                ThrowLastWin32Error("Failed to open");
            }
            Handle = handle;
        }

        public void Open(DeviceMode mode)
        {
            Open();
        }

        public void Open(DeviceMode mode, int read_timeout)
        {
            Open();
        }

        public void Open(DeviceMode mode, int read_timeout, uint kernel_buffer_size)
        {
            Open();
            SetParam(WinDivertParam.QueueSize, kernel_buffer_size);
        }

        public void Open(DeviceMode mode, int read_timeout, MonitorMode monitor_mode)
        {
            Open(mode, read_timeout);
        }

        public void Open(DeviceMode mode, int read_timeout, MonitorMode monitor_mode, uint kernel_buffer_size)
        {
            Open(mode, read_timeout, kernel_buffer_size);
        }

        protected void ThrowIfNotOpen()
        {
            if (Handle == IntPtr.Zero)
            {
                throw new DeviceNotReadyException("Device not open");
            }
        }

        public void SetParam(WinDivertParam param, ulong value)
        {
            ThrowIfNotOpen();
            var ret = WinDivertNative.WinDivertSetParam(Handle, param, value);
            if (!ret)
            {
                ThrowLastWin32Error("Failed to set param");
            }
        }

        public ulong GetParam(WinDivertParam param)
        {
            ThrowIfNotOpen();
            var ret = WinDivertNative.WinDivertGetParam(Handle, param, out ulong value);
            if (!ret)
            {
                ThrowLastWin32Error("Failed to get param");
            }
            return value;
        }

        public void SendPacket(Packet p)
        {
            SendPacket(new ReadOnlySpan<byte>(p.Bytes));
        }

        public void SendPacket(Packet p, int size)
        {
            SendPacket(new ReadOnlySpan<byte>(p.Bytes, 0, size));
        }

        public void SendPacket(byte[] p)
        {
            SendPacket(new ReadOnlySpan<byte>(p));
        }

        public void SendPacket(byte[] p, int size)
        {
            SendPacket(new ReadOnlySpan<byte>(p, 0, size));
        }

        public void SendPacket(ReadOnlySpan<byte> p)
        {
            // TODO
            throw new NotSupportedException();
        }

        public bool Started => captureThread?.IsAlive ?? false;

        public TimeSpan StopCaptureTimeout { get; set; } = new TimeSpan(0, 0, 1);

        public event PacketArrivalEventHandler OnPacketArrival;
        public event CaptureStoppedEventHandler OnCaptureStopped;

        /// <summary>
        /// Thread that is performing the background packet capture
        /// </summary>
        protected Thread captureThread;
        private CancellationTokenSource threadCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Starts the capturing process via a background thread
        /// OnPacketArrival() will be called for each captured packet
        /// </summary>
        public virtual void StartCapture()
        {
            if (!Started)
            {
                ThrowIfNotOpen();
                if (OnPacketArrival == null)
                {
                    throw new DeviceNotReadyException("No delegates assigned to OnPacketArrival, no where for captured packets to go.");
                }
                var cancellationToken = threadCancellationTokenSource.Token;
                captureThread = new Thread(() => this.CaptureThread(cancellationToken));
                captureThread.Start();
            }
        }

        protected void CaptureThread(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var packet = GetNextPacket();
                    if (packet == null)
                    {
                        break;
                    }
                    OnPacketArrival?.Invoke(this, new CaptureEventArgs(packet, this));
                }
                OnCaptureStopped?.Invoke(this, CaptureStoppedEventStatus.CompletedWithoutError);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                OnCaptureStopped?.Invoke(this, CaptureStoppedEventStatus.ErrorWhileCapturing);
            }
        }

        public virtual void Capture()
        {
            ThrowIfNotOpen();
            CaptureThread(threadCancellationTokenSource.Token);
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
                if (!captureThread.Join(StopCaptureTimeout))
                {
                    try
                    {
                        captureThread.Abort();
                    }
                    catch (PlatformNotSupportedException)
                    {
                        // ignore
                    }
                }
                captureThread = null; // otherwise we will always return true from PcapDevice.Started
            }
        }

        private static void ThrowWin32Error(string message, int err)
        {
            var win32Message = new Win32Exception(err).Message;
            throw new PcapException($"{message}\n{win32Message} (Error Code: {err})");
        }

        private static void ThrowLastWin32Error(string message)
        {
            var err = Marshal.GetLastWin32Error();
            ThrowWin32Error(message, err);
        }
    }
}
