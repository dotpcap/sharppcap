using PacketDotNet;
using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpPcap.WinpkFilter
{
    public class WinpkFilterDevice : BaseLiveDevice, ILiveDevice
    {

        /// <summary>
        /// Gets the medium of the TCP adapter.
        /// </summary>
        public uint NdisMedium { get; }

        /// <summary>
        /// Gets the MTU of the network adapter.
        /// </summary>
        public ushort Mtu { get; }

        /// <summary>
        /// Gets the name of the network adapter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the physical address of the network adapter.
        /// </summary>
        public PhysicalAddress MacAddress { get; }

        /// <summary>
        /// Gets the friendly name of the network adapter, as shown in Windows' control panel.
        /// </summary>
        public string FriendlyName { get; }

        public string Description => null;

        public string LastError => null;

        private readonly DriverHandle DriverHandle;

        /// <summary>
        /// Gets the handle of the network adapter.
        /// </summary>
        private readonly IntPtr AdapterHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinpkFilterDevice" /> class.
        /// </summary>
        /// <param name="adapterHandle">The handle.</param>
        /// <param name="nameBytes">Name of the adapter in bytes.</param>
        /// <param name="medium">The medium.</param>
        /// <param name="address">The mac address.</param>
        /// <param name="mtu">The mtu.</param>
        internal WinpkFilterDevice(
            DriverHandle driverHandle, IntPtr adapterHandle,
            byte[] nameBytes,
            uint medium, byte[] address, ushort mtu)
        {
            DriverHandle = driverHandle;
            AdapterHandle = adapterHandle;
            Mtu = mtu;
            NdisMedium = medium;

            MacAddress = new PhysicalAddress(address);

            Name = GetPrivateName(nameBytes);
            FriendlyName = ConvertAdapterName(nameBytes);
        }

        /// <summary>
        /// Gets a value indicating whether the network adapter is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                foreach (var b in MacAddress.GetAddressBytes())
                {
                    if (b != 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the private name.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetPrivateName(byte[] adapterNameBytes)
        {
            var name = Encoding.ASCII.GetString(adapterNameBytes);
            var i = name.IndexOf((char)0);
            return i >= 0 ? name.Substring(0, i) : name;
        }

        /// <summary>
        /// Converts the name of the adapter.
        /// </summary>
        /// <param name="adapterNameBytes">Bytes of the adapter name.</param>
        /// <returns><see cref="string"/>.</returns>
        private static string ConvertAdapterName(byte[] adapterNameBytes)
        {

            var friendlyNameBytes = new byte[NativeMethods.ADAPTER_NAME_SIZE];
            string friendlyName = null;

            var success = NativeMethods.ConvertWindows2000AdapterName(adapterNameBytes, friendlyNameBytes, (uint)friendlyNameBytes.Length);

            if (success)
            {
                var indexOfZero = 0;
                while (indexOfZero < 256 && friendlyNameBytes[indexOfZero] != 0)
                    ++indexOfZero;
                friendlyName = Encoding.Default.GetString(friendlyNameBytes, 0, indexOfZero);
            }

            return friendlyName;
        }

        /// <summary>
        /// The adapter mode.
        /// </summary>
        public AdapterModes AdapterMode
        {
            get
            {
                var adapterMode = new AdapterMode { AdapterHandle = AdapterHandle };
                NativeMethods.GetAdapterMode(DriverHandle, ref adapterMode);
                return adapterMode.Flags;
            }
            set
            {
                var adapterMode = new AdapterMode { Flags = value, AdapterHandle = AdapterHandle };
                if (!NativeMethods.SetAdapterMode(DriverHandle, ref adapterMode))
                {
                    throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// The hardware packet filter.
        /// </summary>
        public HardwarePacketFilters HardwarePacketFilter
        {
            get
            {
                HardwarePacketFilters ndisPacketType = default;
                NativeMethods.GetHwPacketFilter(DriverHandle, AdapterHandle, ref ndisPacketType);
                return ndisPacketType;
            }
            set
            {
                if (!NativeMethods.SetHwPacketFilter(DriverHandle, AdapterHandle, value))
                {
                    throw new ArgumentException();
                }
            }
        }

        private readonly byte[] ReadBuffer = new byte[NativeMethods.IntermediateBufferSize];

        protected override GetPacketStatus GetUnfilteredPacket(out PacketCapture e, TimeSpan timeout)
        {
            unsafe
            {
                fixed (byte* bufferPtr = ReadBuffer)
                {
                    var ethRequest = new EthRequest();
                    ethRequest.AdapterHandle = AdapterHandle;
                    ethRequest.Buffer = new IntPtr(bufferPtr);

                    var ret = NativeMethods.ReadPacket(DriverHandle, ref ethRequest);
                    if (!ret)
                    {
                        e = default;
                        return GetPacketStatus.ReadTimeout;
                    }
                }
            }
            var bufferHeader = MemoryMarshal.Read<IntermediateBufferHeader>(ReadBuffer);
            var bufferData = new ReadOnlySpan<byte>(ReadBuffer, NativeMethods.IntermediateBufferHeaderSize, (int)bufferHeader.Length);
            var header = new WinpkFilterHeader()
            {
                Source = bufferHeader.Source,
                Dot1q = bufferHeader.Dot1q,
            };
            e = new PacketCapture(this, header, bufferData);
            return GetPacketStatus.PacketRead;
        }

        /// <summary>
        /// Sends a raw packet through this device
        /// </summary>
        /// <param name="p">The packet bytes to send</param>
        /// <param name="header">The packet header</param>
        public void SendPacket(ReadOnlySpan<byte> p, ICaptureHeader header = null)
        {
            if (p.Length > NativeMethods.MAX_ETHER_FRAME)
            {
                throw new ArgumentOutOfRangeException(nameof(p));
            }
            var hdr = header as WinpkFilterHeader;
            var buffer = new IntermediateBuffer();
            buffer.Header.Dot1q = hdr?.Dot1q ?? 0;
            buffer.Header.Source = hdr?.Source ?? PacketSource.System;
            buffer.Header.Length = (uint)p.Length;
            EthRequest ethRequest;
            unsafe
            {
                p.CopyTo(new Span<byte>(buffer.Frame, p.Length));
                ethRequest.AdapterHandle = AdapterHandle;
                ethRequest.Buffer = new IntPtr(&buffer);
            }

            var ret = buffer.Header.Source == PacketSource.System
                ? NativeMethods.SendPacketToAdapter(DriverHandle, ref ethRequest)
                : NativeMethods.SendPacketToMstcp(DriverHandle, ref ethRequest);

            if (!ret)
            {
                throw new PcapException("Failed to send packet");
            }
        }

        public void Open(DeviceConfiguration configuration)
        {
            if (configuration.Mode.HasFlag(DeviceModes.Promiscuous))
            {
                HardwarePacketFilter |= HardwarePacketFilters.Promiscuous;
            }
            if (AdapterMode == AdapterModes.None)
            {
                // Most simular mode to Libpcap
                AdapterMode = AdapterModes.RecvListen;
            }
        }

        protected override void CaptureLoop(CancellationToken token)
        {
            using (var manualResetEvent = new ManualResetEvent(false))
            {
                var waitHandles = new WaitHandle[] { token.WaitHandle, manualResetEvent };
                var success = NativeMethods.SetPacketEvent(DriverHandle, AdapterHandle, manualResetEvent.SafeWaitHandle);
                if (!success)
                {
                    throw new PcapException("Can not perform capture");
                }
                while (!token.IsCancellationRequested)
                {
                    if (WaitHandle.WaitAny(waitHandles) == 0)
                    {
                        // We got a cancellation request
                        return;
                    }
                    while (GetNextPacket(out var capture) == GetPacketStatus.PacketRead)
                    {
                        RaiseOnPacketArrival(capture);
                    }
                    manualResetEvent.Reset();
                }
            }
        }

    }
}



