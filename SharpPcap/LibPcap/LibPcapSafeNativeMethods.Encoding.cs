﻿// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
// Copyright 2008-2011 Chris Morgan <chmorgan@gmail.com>
// Copyright 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpPcap.LibPcap
{
    internal static partial class LibPcapSafeNativeMethods
    {

        /// <summary>
        /// This defaul is good enough for .NET Framework and .NET Core on non Windows with Libpcap default config
        /// </summary>
        private static readonly Encoding StringEncoding = Encoding.Default;

        private static Encoding ConfigureStringEncoding()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // libpcap always use UTF-8 when not on Windows
                return Encoding.UTF8;
            }
            try
            {
                // Try to change Libpcap to UTF-8 mode
                var errorBuffer = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE);
                const uint PCAP_CHAR_ENC_UTF_8 = 1;
                var res = pcap_init(PCAP_CHAR_ENC_UTF_8, errorBuffer);
                if (res == 0)
                {
                    // We made it
                    return Encoding.UTF8;
                }
            }
            catch (TypeLoadException)
            {
                // pcap_init not supported, using old Libpcap
            }
            // Needed especially in .NET Core, to make sure codepage 0 returns the system default non-unicode code page
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // In windows by default, system code page is used
            return Encoding.GetEncoding(0);
        }

        /// <summary>
        /// Helper class to marshall StringBuilder depending on encoding used by Libpcap
        /// </summary>
        private class PcapStringBuilderMarshaler : ICustomMarshaler
        {

            public static ICustomMarshaler GetInstance(string cookie)
            {
                return new PcapStringBuilderMarshaler();
            }

            public void CleanUpManagedData(object managedObj)
            {
                // Nothing to clean
            }

            public void CleanUpNativeData(IntPtr nativeData)
            {
                // Nothing to clean
            }

            public int GetNativeDataSize()
            {
                return -1;
            }

            public IntPtr MarshalManagedToNative(object managedObj)
            {
                var builder = (StringBuilder)managedObj;
                var byteCount = builder.Capacity + 1;
                var gcHandle = GCHandle.ToIntPtr(GCHandle.Alloc(managedObj));

                // The problem is that we need a reference to the StringBuilder in MarshalNativeToManaged
                // So we get a pointer to it with GCHandle, and put it as prefix of the pointer we return
                var ptr = Marshal.AllocHGlobal(byteCount + IntPtr.Size);
                Marshal.WriteIntPtr(ptr, gcHandle);
                ptr += IntPtr.Size;
                return ptr;
            }

            public unsafe object MarshalNativeToManaged(IntPtr nativeData)
            {
                var gcHandlePtr = Marshal.ReadIntPtr(nativeData - IntPtr.Size);
                var bytes = (byte*)nativeData;
                var nbBytes = 0;
                while (*(bytes + nbBytes) != 0)
                {
                    nbBytes++;
                }
                var stringData = StringEncoding.GetString(bytes, nbBytes);

                var gcHandle = GCHandle.FromIntPtr(gcHandlePtr);
                var stringBuilder = (StringBuilder)gcHandle.Target;
                gcHandle.Free();
                stringBuilder.Append(stringData);

                return null;
            }
        }

        /// <summary>
        /// Helper class to marshall string depending on encoding used by Libpcap
        /// </summary>
        private class PcapStringMarshaler : ICustomMarshaler
        {

            public static ICustomMarshaler GetInstance(string cookie)
            {
                return new PcapStringMarshaler(cookie);
            }

            private readonly bool FreeOnClean;
            public PcapStringMarshaler(string cookie)
            {
                // If the string was not allocated by us, don't free it
                FreeOnClean = !cookie.Contains("no_free");
            }

            public void CleanUpManagedData(object managedObj)
            {
                // Nothing to clean
            }

            public void CleanUpNativeData(IntPtr nativeData)
            {
                if (FreeOnClean)
                {
                    Marshal.FreeHGlobal(nativeData);
                }
            }

            public int GetNativeDataSize()
            {
                return -1;
            }

            public IntPtr MarshalManagedToNative(object managedObj)
            {
                if (managedObj is null)
                {
                    return IntPtr.Zero;
                }
                var str = (string)managedObj;
                var bytes = StringEncoding.GetBytes(str);
                // The problem is that we need a reference to the StringBuilder in MarshalNativeToManaged
                // So we get a pointer to it with GCHandle, and put it as prefix of the pointer we return
                var ptr = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                // Put zero string termination
                Marshal.WriteByte(ptr + bytes.Length, 0);
                return ptr;
            }

            public unsafe object MarshalNativeToManaged(IntPtr nativeData)
            {
                if (nativeData == IntPtr.Zero)
                {
                    return null;
                }
                var gcHandlePtr = Marshal.ReadIntPtr(nativeData);
                var bytes = (byte*)nativeData;
                var nbBytes = 0;
                while (*(bytes + nbBytes) != 0)
                {
                    nbBytes++;
                }
                return StringEncoding.GetString(bytes, nbBytes);
            }
        }
    }
}
