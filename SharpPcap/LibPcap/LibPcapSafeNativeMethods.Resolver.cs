// Copyright 2020-2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Per http://msdn.microsoft.com/en-us/ms182161.aspx 
    /// </summary>
    internal static partial class LibPcapSafeNativeMethods
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        static LibPcapSafeNativeMethods()
        {
            if (OperatingSystem.IsWindows())
            {
                SetDllDirectory(Path.Combine(Environment.SystemDirectory, "Npcap"));
            }
            RegisterResolver();
            StringEncoding = ConfigureStringEncoding();
        }

        /// <summary>
        /// The class NativeLibrary is only available since .NET Core 3.0
        /// It's not available in .NET Framework, 
        /// but that does not affect us since we would use the Windows dll name wpcap
        /// </summary>
        private static void RegisterResolver()
        {
            NativeLibrary.SetDllImportResolver(typeof(LibPcapSafeNativeMethods).Assembly, Resolver);
        }

        public static IntPtr Resolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName != PCAP_DLL)
            {
                // Use default resolver
                return IntPtr.Zero;
            }

            var names = new List<string>();

            if (OperatingSystem.IsLinux())
            {
                names.Add("libpcap.so");
                names.Add("libpcap.so.0");
                names.Add("libpcap.so.0.8");
                names.Add("libpcap.so.1");
            }

            else if (OperatingSystem.IsWindows())
            {
                names.Add("wpcap.dll");
            }

            else if (OperatingSystem.IsMacOS())
            {
                names.Add("libpcap.dylib");
            }

            foreach (var name in names)
            {
                if (NativeLibrary.TryLoad(name, out var handle))
                {
                    return handle;
                }
            }

            return IntPtr.Zero;
        }
    }
}
