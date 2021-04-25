using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using PacketDotNet;

namespace SharpPcap.WinDivert
{
    public class WinDivertDevice : ILiveDevice
    {

        static readonly DateTime BootTime = DateTime.Now - TimeSpan.FromTicks(Stopwatch.GetTimestamp());

        const int ERROR_INSUFFICIENT_BUFFER = 122;
        const int ERROR_NO_DATA = 232;

        protected IntPtr Handle;

        public string Name => "WinDivert";

        public string Description => "WinDivert Packet Driver";

        public LinkLayers LinkType => LinkLayers.Raw;

        public PhysicalAddress MacAddress => null;

        /// <summary>
        /// Not currently supported for this device
        /// </summary>
        public ICaptureStatistics Statistics => null;

        public string LastError { get; private set; }

        private string filter;

        /// <summary>
        /// See https://www.reqrypt.org/windivert-doc.html#filter_language for filter language details
        /// </summary>
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

        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        /// <returns>0 for no data present, 1 if a packet was read, negative upon error</returns>
        public int GetNextPacket(out CaptureEventArgs e)
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
                        e = default;
                        return -err;
                    }
                    ThrowWin32Error("Recv failed", err);
                }
                var timestamp = new PosixTimeval(BootTime + TimeSpan.FromTicks(addr.Timestamp));
                var data = buffer.Slice(0, (int)readLen).ToArray();
                var header = new WinDivertHeader(timestamp)
                {
                    InterfaceIndex = addr.IfIdx,
                    SubInterfaceIndex = addr.SubIfIdx,
                    Flags = addr.Flags
                };

                e = new CaptureEventArgs(this, header, data);

                return 1;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>0 for no data present, 1 if a packet was read, negative upon error</returns>
        private int SendPacketArrivalEvent()
        {
            CaptureEventArgs e;
            var retval = GetNextPacket(out e);
            if (retval == 1)
            {
                OnPacketArrival?.Invoke(this, e);
            }

            return retval;
        }

        public void Open(DeviceConfiguration configuration)
        {
            var handle = WinDivertNative.WinDivertOpen(Filter, Layer, Priority, Flags);
            if (handle == IntPtr.Zero || handle == new IntPtr(-1))
            {
                ThrowLastWin32Error("Failed to open");
            }
            Handle = handle;

            if (configuration.BufferSize > 0)
            {
                SetParam(WinDivertParam.QueueSize, (uint)configuration.BufferSize);
            }
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

        public void SendPacket(RawCapture p)
        {
            SendPacket(new ReadOnlySpan<byte>(p.Data));
        }

        /// <summary>
        /// Note: Assumes this span was received from a WinDivertDevice
        /// </summary>
        /// <param name="p"></param>
        public void SendPacket(ReadOnlySpan<byte> p)
        {
            var addr = GetAddress(p);
            var header = new WinDivertHeader(new PosixTimeval());
            header.InterfaceIndex = addr.IfIdx;
            header.SubInterfaceIndex = addr.SubIfIdx;
            header.Flags = addr.Flags;

            SendPacket(p, header);
        }

        /// <summary>
        /// Send a packet using header.Flags, header.InterfaceIndex, and header.SubInterfaceIndex
        /// </summary>
        /// <param name="p"></param>
        /// <param name="header"></param>
        public void SendPacket(ReadOnlySpan<byte> p, WinDivertHeader captureHeader)
        {
            ThrowIfNotOpen();
            bool res;
            WinDivertAddress addr = default;

            if (!(captureHeader is WinDivertHeader))
            {
                addr = GetAddress(p);
            }
            else
            {
                addr.IfIdx = captureHeader.InterfaceIndex;
                addr.SubIfIdx = captureHeader.SubInterfaceIndex;
                addr.Flags = captureHeader.Flags;
            }

            unsafe
            {
                fixed (byte* p_packet = p)
                {
                    res = WinDivertNative.WinDivertSend(Handle, new IntPtr(p_packet), (uint)p.Length, out var pSendLen, ref addr);
                }
            }
            if (!res)
            {
                ThrowLastWin32Error("Can't send packet");
            }
        }

        private static WinDivertAddress GetAddress(ReadOnlySpan<byte> p)
        {
            var version = p[0] >> 4;
            ReadOnlySpan<byte> srcBytes;
            ReadOnlySpan<byte> dstytes;
            if (version == 4)
            {
                srcBytes = p.Slice(IPv4Fields.SourcePosition, IPv4Fields.AddressLength);
                dstytes = p.Slice(IPv4Fields.DestinationPosition, IPv4Fields.AddressLength);
            }
            else
            {
                srcBytes = p.Slice(IPv6Fields.SourceAddressPosition, IPv6Fields.AddressLength);
                dstytes = p.Slice(IPv6Fields.DestinationAddressPosition, IPv6Fields.AddressLength);
            }
            var src = new IPAddress(srcBytes.ToArray());
            var dst = new IPAddress(dstytes.ToArray());
            WinDivertAddress addr = default;
            addr.IfIdx = (uint)IpHelper.GetBestInterfaceIndex(dst);
            if (IpHelper.IsOutbound((int)addr.IfIdx, src, dst))
            {
                addr.Flags |= WinDivertPacketFlags.Outbound;
            }
            return addr;
        }

        public bool Started => captureThread?.IsAlive ?? false;

        public TimeSpan StopCaptureTimeout { get; set; } = new TimeSpan(0, 0, 1);
        public WinDivertLayer Layer { get; set; } = WinDivertLayer.Network;
        public short Priority { get; set; }
        public ulong Flags { get; set; } = 1;

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
                    SendPacketArrivalEvent();
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

        #region Timestamp
        /// <summary>
        /// Per https://reqrypt.org/windivert-doc.html
        /// The Timestamp indicates when a packet was received and uses the same clock as QueryPerformancetimer()
        /// which itself has <1us resolution. This maps best to microsecond resolution.
        /// </summary>
        public virtual TimestampResolution TimestampResolution
        {
            get => TimestampResolution.Microsecond;
        }
        #endregion

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

        ~WinDivertDevice()
        {
            Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
