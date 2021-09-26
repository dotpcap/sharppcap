using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace SharpPcap.WinpkFilter
{
    [SuppressUnmanagedCodeSecurity]
    static class NativeMethods
    {
        /// <summary>
        /// The file name of the Ndis Api DLL.
        /// </summary>
        private const string NDISAPI = "ndisapi.dll";

        static NativeMethods()
        {
            IntermediateBufferHeaderSize = Marshal.SizeOf<IntermediateBufferHeader>();
            IntermediateBufferSize = Marshal.SizeOf<IntermediateBuffer>();
        }

        #region Methods

        [DllImport(NDISAPI)]
        internal static extern DriverHandle OpenFilterDriver(byte[] pszDriverName);

        [DllImport(NDISAPI)]
        internal static extern void CloseFilterDriver(IntPtr hOpen);

        [DllImport(NDISAPI)]
        internal static extern uint GetDriverVersion(DriverHandle hOpen);

        [DllImport(NDISAPI)]
        internal static extern bool GetTcpipBoundAdaptersInfo(DriverHandle hOpen, ref TcpAdapterList adapters);

        [DllImport(NDISAPI)]
        internal static extern bool SendPacketToMstcp(DriverHandle hOpen, ref EthRequest packet);

        [DllImport(NDISAPI)]
        internal static extern bool SendPacketToAdapter(DriverHandle hOpen, ref EthRequest packet);

        [DllImport(NDISAPI)]
        internal static extern bool ReadPacket(DriverHandle hOpen, ref EthRequest packet);

        [DllImport(NDISAPI)]
        internal static extern bool SetAdapterMode(DriverHandle hOpen, ref AdapterMode mode);

        [DllImport(NDISAPI)]
        internal static extern bool GetAdapterMode(DriverHandle hOpen, ref AdapterMode mode);

        [DllImport(NDISAPI)]
        internal static extern bool GetAdapterPacketQueueSize(DriverHandle hOpen, IntPtr hAdapter, ref uint dwSize);

        [DllImport(NDISAPI)]
        internal static extern bool SetPacketEvent(DriverHandle hOpen, IntPtr hAdapter, SafeWaitHandle hWin32Event);

        [DllImport(NDISAPI)]
        internal static extern bool SetHwPacketFilter(DriverHandle hOpen, IntPtr hAdapter, HardwarePacketFilters filter);

        [DllImport(NDISAPI)]
        internal static extern bool GetHwPacketFilter(DriverHandle hOpen, IntPtr hAdapter, ref HardwarePacketFilters pFilter);

        [DllImport(NDISAPI)]
        internal static extern bool ConvertWindows2000AdapterName(
            [MarshalAs(UnmanagedType.LPArray, SizeConst=ADAPTER_NAME_SIZE)]
            [In]
            byte[] szAdapterName,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]
            [Out]
            byte[] szUserFriendlyName,
            uint dwUserFriendlyNameLength
        );

        #endregion


        #region Sizes & Offsets

        /// <summary>
        /// The maximum adapter name length.
        /// </summary>
        internal const int ADAPTER_NAME_SIZE = 256;

        /// <summary>
        /// The adapter list size.
        /// </summary>
        internal const int ADAPTER_LIST_SIZE = 32;

        /// <summary>
        /// The maximum ether frame size.
        /// </summary>
        internal const int MAX_ETHER_FRAME = 1514;

        /// <summary>
        /// The ether address length.
        /// </summary>
        internal const int ETHER_ADDR_LENGTH = 6;

        internal static readonly int IntermediateBufferHeaderSize;
        internal static readonly int IntermediateBufferSize;

        #endregion
    }
}