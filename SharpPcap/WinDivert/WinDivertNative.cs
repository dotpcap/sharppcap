using System;
using System.Runtime.InteropServices;

namespace SharpPcap.WinDivert
{
    internal static unsafe class WinDivertNative
    {
        const string WINDIVERT_DLL = "WinDivert.dll";

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr WinDivertOpen([MarshalAs(UnmanagedType.LPStr)] string filter, WinDivertLayer layer, short priority, ulong flags);

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool WinDivertRecvEx(
            IntPtr handle,
            [In, Out] byte[] pPacket,
            int packetLen,
            out int pRecvLen,
            ulong flags,
            [In, Out] WinDivertAddress[] pAddr,
            ref int pAddrLen,
            IntPtr lpOverlapped
        );

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool WinDivertClose(IntPtr handle);

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool WinDivertSetParam(IntPtr handle, WinDivertParam param, ulong value);

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool WinDivertGetParam(IntPtr handle, WinDivertParam param, out ulong pValue);

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool WinDivertHelperCompileFilter([MarshalAs(UnmanagedType.LPStr)] string filter, WinDivertLayer layer, IntPtr obj, uint objLen, out IntPtr errorStr, out uint errorPos);

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool WinDivertSend(IntPtr handle, IntPtr pPacket, uint packetLen, out uint pSendLen, ref WinDivertAddress pAddr);
    }
}