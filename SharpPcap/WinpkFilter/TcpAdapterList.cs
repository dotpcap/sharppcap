
using System;
using System.Runtime.InteropServices;
using static SharpPcap.WinpkFilter.NativeMethods;

namespace SharpPcap.WinpkFilter
{
    /// <summary>
    /// Used for requesting information about currently bound TCPIP adapters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TcpAdapterList
    {
        internal uint AdapterCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADAPTER_LIST_SIZE * ADAPTER_NAME_SIZE)]
        internal byte[] AdapterNames;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADAPTER_LIST_SIZE)]
        internal IntPtr[] AdapterHandles;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADAPTER_LIST_SIZE)]
        internal uint[] AdapterMediums;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADAPTER_LIST_SIZE * ETHER_ADDR_LENGTH)]
        internal byte[] CurrentAddresses;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADAPTER_LIST_SIZE)]
        internal ushort[] MTUs;

    }

}