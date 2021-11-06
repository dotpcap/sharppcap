using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using PacketDotNet;

namespace SharpPcap.WinDivert
{
    public class WinDivertDevice : ILiveDevice
    {

        static readonly DateTime BootTime = DateTime.Now - TimeSpan.FromTicks(Stopwatch.GetTimestamp());

        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        private const int ERROR_NO_DATA = 232;
        private const int WINDIVERT_BATCH_MAX = 0xff;
        private const int MTU = 1500;

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

        private class PacketRecord
        {
            private readonly WinDivertHeader Header;
            private readonly ReadOnlyMemory<byte> Data;

            public PacketRecord(WinDivertAddress addr, ReadOnlyMemory<byte> data)
            {
                var timestamp = new PosixTimeval(BootTime + TimeSpan.FromTicks(addr.Timestamp));
                Header = new WinDivertHeader(timestamp)
                {
                    InterfaceIndex = addr.IfIdx,
                    SubInterfaceIndex = addr.SubIfIdx,
                    Flags = addr.Flags
                };
                ;
                Data = data;
            }

            public PacketCapture GetPacketCapture(ICaptureDevice device)
            {
                return new PacketCapture(device, Header, Data.Span);
            }
        }

        /// <summary>
        /// Packet data is only valid until the next call
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Status of the operation</returns>
        public GetPacketStatus GetNextPacket(out PacketCapture e)
        {
            var packets = new List<PacketRecord>();
            var res = GetNextPackets(packets, 1);
            e = packets.Count > 0 ? packets[0].GetPacketCapture(this) : default;
            return res;
        }

        private byte[] Buffer = new byte[MTU * WINDIVERT_BATCH_MAX];

        /// <summary>
        /// Packet data is only valid until the next call
        /// </summary>
        /// <param name="packets"></param>
        /// <returns>Status of the operation</returns>
        private GetPacketStatus GetNextPackets(List<PacketRecord> packets, int maxBatchSize)
        {
            ThrowIfNotOpen();
            while (true)
            {
                bool ret;
                var addressSize = Marshal.SizeOf<WinDivertAddress>();
                var addressesByteSize = maxBatchSize * addressSize;
                var addresses = new WinDivertAddress[maxBatchSize];
                var packetsData = new Memory<byte>(Buffer);
                ret = WinDivertNative.WinDivertRecvEx(
                    Handle,
                    ref MemoryMarshal.GetReference(packetsData.Span),
                    packetsData.Length,
                    out int readLen,
                    0,
                    addresses,
                    ref addressesByteSize,
                    IntPtr.Zero
                );
                if (!ret)
                {
                    var err = Marshal.GetLastWin32Error();
                    if (err == ERROR_INSUFFICIENT_BUFFER)
                    {
                        // Increase buffer size
                        Buffer = new byte[Buffer.Length * 2];
                        continue;
                    }
                    if (err == ERROR_NO_DATA)
                    {
                        return (GetPacketStatus)(-err);
                    }
                    ThrowWin32Error("Recv failed", err);
                }

                // Take only as many bytes as were written by the driver
                packetsData = packetsData.Slice(0, readLen);

                var addressesCount = addressesByteSize / addressSize;


                for (int i = 0; i < addressesCount - 1; i++)
                {

                    // We know how many packets we have, but not where they start/end in the buffer
                    // Helper function that's originally used for parsing, 
                    // but we only need it to know the size of the current packet
                    var retval = WinDivertNative.WinDivertHelperParsePacket(
                        ref MemoryMarshal.GetReference(packetsData.Span),
                        packetsData.Length,
                        IntPtr.Zero, IntPtr.Zero,
                        IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero,
                        IntPtr.Zero, IntPtr.Zero,
                        IntPtr.Zero, out var pNextLen
                    );

                    if (!retval)
                    {
                        return GetPacketStatus.Error;
                    }

                    var packetLength = packetsData.Length - pNextLen;

                    packets.Add(new PacketRecord(addresses[i], packetsData.Slice(0, packetLength)));

                    // Move packetsData to the next packet
                    packetsData = packetsData.Slice(packetLength);
                }

                // Avoid parsing the packet if it's the last packet or the only packet
                packets.Add(new PacketRecord(addresses[addressesCount - 1], packetsData));

                return GetPacketStatus.PacketRead;
            }
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

        /// <summary>
        /// </summary>
        /// <param name="p"></param>
        /// <param name="header"></param>
        public void SendPacket(ReadOnlySpan<byte> p, ICaptureHeader captureHeader)
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
                var header = captureHeader as WinDivertHeader;
                addr.IfIdx = header.InterfaceIndex;
                addr.SubIfIdx = header.SubInterfaceIndex;
                addr.Flags = header.Flags;
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

        public bool Started => !(captureThread?.IsCompleted ?? true);

        public TimeSpan StopCaptureTimeout { get; set; } = new TimeSpan(0, 0, 1);
        public WinDivertLayer Layer { get; set; } = WinDivertLayer.Network;
        public short Priority { get; set; }
        public ulong Flags { get; set; } = 1;

        public event PacketArrivalEventHandler OnPacketArrival;
        public event CaptureStoppedEventHandler OnCaptureStopped;

        /// <summary>
        /// Thread that is performing the background packet capture
        /// </summary>
        protected Task captureThread;
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
                captureThread = Task.Run(() => CaptureThread(cancellationToken), cancellationToken);
            }
        }

        protected void CaptureThread(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var packets = new List<PacketRecord>(WINDIVERT_BATCH_MAX);
                    var retval = GetNextPackets(packets, WINDIVERT_BATCH_MAX);
                    if (retval == GetPacketStatus.PacketRead)
                    {
                        foreach (var p in packets)
                        {
                            OnPacketArrival?.Invoke(this, p.GetPacketCapture(this));
                        }
                    }
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
                Task.WaitAny(new[] { captureThread }, StopCaptureTimeout);
                captureThread = null;
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
