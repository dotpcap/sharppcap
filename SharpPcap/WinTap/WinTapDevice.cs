using Microsoft.Win32.SafeHandles;
using SharpPcap.WinpkFilter;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
namespace SharpPcap.WinTap
{
    public partial class WinTapDevice : BaseLiveDevice, ILiveDevice
    {
        private readonly NetworkInterface Interface;
        private FileStream Stream;

        protected SafeFileHandle Handle { get; set; } = new SafeFileHandle(IntPtr.Zero, false);

        public string Name => "wintap:" + Interface.Name;

        public string FriendlyName => Interface.Name;

        public string Description => Interface.Description;

        public string LastError => null;

        public Version Version
        {
            get => NativeMethods.GetVersion(Handle);
        }

        public PhysicalAddress MacAddress => Interface.GetPhysicalAddress();

        public WinTapDevice(NetworkInterface networkInterface)
        {
            this.Interface = networkInterface;
        }

        public static NetworkInterface[] GetTapInterfaces()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            return nics.Where(n => n.Description.StartsWith("TAP-Windows Adapter"))
                .ToArray();
        }

        public void Open(DeviceConfiguration configuration)
        {
            if (!(Handle.IsClosed || Handle.IsInvalid))
            {
                return;
            }
            this.Handle = NativeMethods.CreateFile(@"\\.\Global\" + Interface.Id + ".tap",
                WinFileAccess.GenericRead | WinFileAccess.GenericWrite,
                0,
                IntPtr.Zero,
                WinFileCreation.OpenExisting,
                WinFileAttributes.System | WinFileAttributes.Overlapped,
                IntPtr.Zero
            );
            if (Handle.IsInvalid)
            {
                throw new PcapException("Failed to open device");
            }
            NativeMethods.SetMediaStatus(Handle, true);
            this.Stream = new FileStream(Handle, FileAccess.ReadWrite, 0x1000, true);
        }

        public override void Close()
        {
            base.Close();
            Stream?.Close();
            Handle.Close();
        }

        private readonly byte[] ReadBuffer = new byte[0x4000];
        protected override GetPacketStatus GetUnfilteredPacket(out PacketCapture e, TimeSpan timeout)
        {
            var cts = new CancellationTokenSource(timeout);
            var task = Stream.ReadAsync(ReadBuffer, 0, ReadBuffer.Length, cts.Token);
            task.Wait();

            if (!task.IsFaulted)
            {
                if (task.Result == 0)
                {
                    e = default;
                    return GetPacketStatus.NoRemainingPackets;
                }
                var data = new Span<byte>(ReadBuffer).Slice(0, task.Result);
                e = new PacketCapture(this, new WinpkFilterHeader(), data);
                return GetPacketStatus.PacketRead;
            }
            else
            {
                throw task.Exception;
            }
        }

        protected override void CaptureLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var task = Stream.ReadAsync(ReadBuffer, 0, ReadBuffer.Length, token);
                task.Wait();
                if (!task.IsFaulted)
                {
                    var data = new Span<byte>(ReadBuffer).Slice(0, task.Result);
                    var p = new PacketCapture(this, new WinpkFilterHeader(), data);
                    RaiseOnPacketArrival(p);
                }
            }
        }

        public void SendPacket(ReadOnlySpan<byte> p, ICaptureHeader header = null)
        {
            var data = p.ToArray();
            Stream.Write(data, 0, data.Length);
            Stream.Flush();
        }
    }
}
