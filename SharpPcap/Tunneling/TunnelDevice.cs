﻿using SharpPcap.WinpkFilter;
using SharpPcap.Tunneling.WinTap;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using SharpPcap.Tunneling.Unix;
using System.Threading.Tasks;

namespace SharpPcap.Tunneling
{
    public partial class TunnelDevice : BaseLiveDevice, ILiveDevice
    {
        private static readonly ITunnelDriver Driver = GetDriver();

        private static ITunnelDriver GetDriver()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WinTapDriver.Instance;
            }
            return TuntapDriver.Instance;
        }

        private readonly NetworkInterface Interface;
        private FileStream Stream;

        protected FileStream GetFileStream()
        {
            return Stream ?? throw new DeviceNotReadyException("Device not open");
        }

        public string Name => "wintap:" + Interface.Name;

        public string FriendlyName => Interface.Name;

        public string Description => Interface.Description;

        public string LastError => null;

        public Version Version
        {
            get => Driver.GetVersion(Interface, GetFileStream().SafeFileHandle);
        }

        public PhysicalAddress MacAddress => Interface.GetPhysicalAddress();

        public TunnelDevice(NetworkInterface networkInterface)
        {
            this.Interface = networkInterface;
        }

        public static NetworkInterface[] GetTunnelInterfaces()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            return nics.Where(Driver.IsTunnelInterface)
                .OrderBy(n => n.Id)
                .ToArray();
        }

        public void Open(DeviceConfiguration configuration)
        {
            if (Stream != null)
            {
                return;
            }
            
            Stream = Driver.Open(Interface, configuration);
            ReadTimeout = TimeSpan.FromMilliseconds(configuration.ReadTimeout);
            Snaplen = configuration.Snaplen;
            
        }

        public override void Close()
        {
            base.Close();
            Stream?.Close();
            Stream = null;
        }

        private int Snaplen = Pcap.MAX_PACKET_SIZE;
        private readonly byte[] ReadBuffer = new byte[0x4000];
        protected override GetPacketStatus GetUnfilteredPacket(out PacketCapture e, TimeSpan timeout)
        {
            var fs = GetFileStream();
            var cts = new CancellationTokenSource(timeout);
            var task = fs.ReadAsync(ReadBuffer, 0, ReadBuffer.Length, cts.Token);

            var index = Task.WaitAny(new[] { task }, timeout);
            if (index == -1 || task.IsCanceled)
            {
                e = default;
                return GetPacketStatus.ReadTimeout;
            }
            if (task.IsFaulted)
            {
                e = default;
                return GetPacketStatus.Error;
            }
            if (task.Result == 0)
            {
                e = default;
                return GetPacketStatus.NoRemainingPackets;
            }
            var data = new Span<byte>(ReadBuffer).Slice(0, Math.Min(task.Result, Snaplen));
            e = new PacketCapture(this, new TunnelHeader(), data);
            return GetPacketStatus.PacketRead;
        }

        protected override void CaptureLoop(CancellationToken token)
        {
            var fs = GetFileStream();
            while (!token.IsCancellationRequested)
            {
                var task = fs.ReadAsync(ReadBuffer, 0, ReadBuffer.Length, token);
                task.Wait();
                if (!task.IsFaulted)
                {
                    var data = new Span<byte>(ReadBuffer).Slice(0, task.Result);
                    var p = new PacketCapture(this, new TunnelHeader(), data);
                    RaiseOnPacketArrival(p);
                }
            }
        }

        public void SendPacket(ReadOnlySpan<byte> p, ICaptureHeader header = null)
        {
            var data = p.ToArray();
            var fs = GetFileStream();
            fs.Write(data, 0, data.Length);
            fs.Flush();
        }
    }
}