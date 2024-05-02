// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace SharpPcap.Tunneling.WinTap
{

    /// <summary>
    /// Driver to communicate with tap device file
    /// </summary>
    class WinTapDriver : ITunnelDriver
    {
        internal static readonly ITunnelDriver Instance = new WinTapDriver();

        public bool IsTunnelInterface(NetworkInterface networkInterface)
        {
            return networkInterface.Description.StartsWith("TAP-Windows Adapter");
        }

        public FileStream Open(NetworkInterface networkInterface, IPAddressConfiguration address, DeviceConfiguration configuration)
        {
            var bufferSize = configuration.BufferSize ?? 4096;
            var handle = CreateFile(@"\\.\Global\" + networkInterface.Id + ".tap",
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

            if (address.Address != null)
            {
                ConfigureDhcp(
                    handle,
                    address.Address,
                    address.IPv4Mask,
                    IPAddress.Parse("0.0.0.0"),
                    TimeSpan.FromHours(24)
                );
            }

            SetMediaStatus(handle, true);
            return new FileStream(handle, FileAccess.ReadWrite, bufferSize, true);
        }

        public Version GetVersion(NetworkInterface networkInterface, SafeFileHandle handle)
        {
            Span<byte> inBuffer = stackalloc byte[0];
            Span<byte> outBuffer = stackalloc byte[12];
            TapControl(handle, TapIoControl.GetVersion, inBuffer, ref outBuffer);
            var v = MemoryMarshal.Cast<byte, int>(outBuffer);
            return new Version(v[0], v[1], v[2]);
        }

        internal static void ConfigureDhcp(SafeFileHandle handle, IPAddress ip, IPAddress mask, IPAddress server, TimeSpan lease_time)
        {
            var inBuffer = new byte[16];
            ip.GetAddressBytes().CopyTo(inBuffer, 0);
            mask.GetAddressBytes().CopyTo(inBuffer, 4);
            server.GetAddressBytes().CopyTo(inBuffer, 8);
            BitConverter.GetBytes((int)lease_time.TotalSeconds).CopyTo(inBuffer, 12);
            Span<byte> outBuffer = stackalloc byte[1];
            TapControl(handle, TapIoControl.ConfigDhcpMasq, inBuffer, ref outBuffer);
        }

        internal static void SetMediaStatus(SafeFileHandle handle, bool connected)
        {
            int value = connected ? 1 : 0;
            Span<byte> inBuffer = stackalloc byte[4];
            Span<byte> outBuffer = stackalloc byte[4];
            MemoryMarshal.Write(inBuffer, in value);
            TapControl(handle, TapIoControl.SetMediaStatus, inBuffer, ref outBuffer);
        }

        private static unsafe void TapControl(SafeFileHandle handle, TapIoControl code, Span<byte> inBuffer, ref Span<byte> outBuffer)
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
                if (!retval)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private const uint METHOD_BUFFERED = 0;
        private const uint FILE_ANY_ACCESS = 0;
        private const uint FILE_DEVICE_UNKNOWN = 0x00000022;

        /// <summary>
        /// See https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/d4drvif/nf-d4drvif-ctl_code
        /// </summary>
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
