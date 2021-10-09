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
            get => NativeMethods.GetVersion(GetFileStream().SafeFileHandle);
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
            if (Stream != null)
            {
                return;
            }
            var handle = NativeMethods.CreateFile(@"\\.\Global\" + Interface.Id + ".tap",
                WinFileAccess.GenericRead | WinFileAccess.GenericWrite,
                0,
                IntPtr.Zero,
                WinFileCreation.OpenExisting,
                WinFileAttributes.System | WinFileAttributes.Overlapped,
                IntPtr.Zero
            );
            if (handle.IsInvalid)
            {
                throw new PcapException("Failed to open device");
            }
            NativeMethods.SetMediaStatus(handle, true);
            this.Stream = new FileStream(handle, FileAccess.ReadWrite, 0x1000, true);
        }

        public override void Close()
        {
            base.Close();
            Stream?.Close();
            Stream = null;
        }

        private readonly byte[] ReadBuffer = new byte[0x4000];
        protected override GetPacketStatus GetUnfilteredPacket(out PacketCapture e, TimeSpan timeout)
        {
            var fs = GetFileStream();
            var cts = new CancellationTokenSource(timeout);
            var task = fs.ReadAsync(ReadBuffer, 0, ReadBuffer.Length, cts.Token);
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
            var fs = GetFileStream();
            while (!token.IsCancellationRequested)
            {
                var task = fs.ReadAsync(ReadBuffer, 0, ReadBuffer.Length, token);
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
            var fs = GetFileStream();
            fs.Write(data, 0, data.Length);
            fs.Flush();
        }
    }
}
