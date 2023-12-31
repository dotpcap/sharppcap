// SPDX-FileCopyrightText: 2020 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpPcap.WinDivert
{
    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class WinDivertNative
    {
        const string WINDIVERT_DLL = "WinDivert.dll";

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr WinDivertOpen([MarshalAs(UnmanagedType.LPStr)] string filter, WinDivertLayer layer, short priority, ulong flags);

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool WinDivertRecvEx(
            IntPtr handle,
            ref byte pPacket,
            int packetLen,
            out int pRecvLen,
            ulong flags,
            [Out] WinDivertAddress[] pAddr,
            ref int pAddrLen,
            IntPtr lpOverlapped
        );

        [DllImport(WINDIVERT_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool WinDivertHelperParsePacket(
            ref byte pPacket,
            int packetLen,
            IntPtr /* PWINDIVERT_IPHDR * */ ppIpHdr,
            IntPtr /* PWINDIVERT_IPV6HDR * */ ppIpv6Hdr,
            IntPtr /* UINT8 * */ pProtocol,
            IntPtr /* PWINDIVERT_ICMPHDR * */ ppIcmpHdr,
            IntPtr /* PWINDIVERT_ICMPV6HDR * */ ppIcmpv6Hdr,
            IntPtr /* PWINDIVERT_TCPHDR * */ ppTcpHdr,
            IntPtr /* PWINDIVERT_UDPHDR * */ ppUdpHdr,
            IntPtr /* PVOID * */ ppData,
            IntPtr /* UINT * */ pDataLen,
            IntPtr /* PVOID * */ ppNext,
            out int /* UINT * */ pNextLen
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