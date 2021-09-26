using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpPcap.WinpkFilter
{
    [SuppressUnmanagedCodeSecurity]
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlZeroMemory")]
        internal static extern void ZeroMemory(IntPtr destination, int length);
    }
}