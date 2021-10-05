using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SharpPcap.WinTap
{
    internal static class NativeMethods
    {

        internal static Version GetVersion(SafeFileHandle handle)
        {
            Span<byte> inBuffer = stackalloc byte[0];
            Span<byte> outBuffer = stackalloc byte[12];
            var retval = TapControl(handle, TapIoControl.GetVersion, inBuffer, ref outBuffer);
            if (!retval)
            {
                return null;
            }
            var v = MemoryMarshal.Cast<byte, int>(outBuffer);
            return new Version(v[0], v[1], v[2]);
        }

        internal static void SetMediaStatus(SafeFileHandle handle, bool connected)
        {
            int value = connected ? 1 : 0;
            Span<byte> inBuffer = stackalloc byte[4];
            Span<byte> outBuffer = stackalloc byte[4];
            MemoryMarshal.Write(inBuffer, ref value);
            var retval = TapControl(handle, TapIoControl.SetMediaStatus, inBuffer, ref outBuffer);
            if (!retval)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static unsafe bool TapControl(SafeFileHandle handle, TapIoControl code, Span<byte> inBuffer, ref Span<byte> outBuffer)
        {
            fixed (byte* inPtr = inBuffer, outPtr = outBuffer)
            {
                var controlCode = CTL_CODE(FILE_DEVICE_UNKNOWN, (uint)code, METHOD_BUFFERED, FILE_ANY_ACCESS);
                var retval = DeviceIoControl(handle, controlCode,
                    new IntPtr(inPtr), inBuffer.Length,
                    new IntPtr(outPtr), outBuffer.Length,
                    out var returnedBytes,
                    IntPtr.Zero
                );
                outBuffer = outBuffer.Slice(0, returnedBytes);
                return retval;
            }
        }

        private const uint METHOD_BUFFERED = 0;
        private const uint FILE_ANY_ACCESS = 0;
        private const uint FILE_DEVICE_UNKNOWN = 0x00000022;

        private static uint CTL_CODE(uint deviceType, uint function, uint method, uint access)
        {
            return ((deviceType << 16) | (access << 14) | (function << 2) | method);
        }


        [DllImport("kernel32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(
            string lpFileName,
            WinFileAccess dwDesiredAccess,
            uint dwShareMode,
            IntPtr SecurityAttributes,
            WinFileCreation dwCreationDisposition,
            WinFileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        internal static extern int GetFileAttributes(string lpFileName);


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool DeviceIoControl(
            [In] SafeFileHandle hDevice,
            [In] uint dwIoControlCode,
            [In] IntPtr lpInBuffer,
            [In] int nInBufferSize,
            [Out] IntPtr lpOutBuffer,
            [In] int nOutBufferSize,
            out int lpBytesReturned,
            [In] IntPtr lpOverlapped
        );
    }
}
