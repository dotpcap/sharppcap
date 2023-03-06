/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/* 
 * Copyright 2020-2021 Ayoub Kaanich <kayoub5@live.com>
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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
            if (
#if NET6_0_OR_GREATER
            OperatingSystem.IsWindows()
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
#endif
            )
            {
                SetDllDirectory(Path.Combine(Environment.SystemDirectory, "Npcap"));
                RegisterResolver();
            }
            StringEncoding = ConfigureStringEncoding();
        }

        /// <summary>
        /// The class NativeLibrary is only available since .NET Core 3.0
        /// It's not available in .NET Framework, 
        /// but that does not affect us since we would use the Windows dll name wpcap
        /// </summary>
        private static void RegisterResolver()
        {
            NativeLibraryHelper.SetDllImportResolver(typeof(LibPcapSafeNativeMethods).Assembly, Resolver);
        }

        public static IntPtr Resolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName != PCAP_DLL)
            {
                // Use default resolver
                return IntPtr.Zero;
            }

            return NativeLibraryHelper.TryLoad("wpcap.dll", out var library) 
                ? library : IntPtr.Zero;
        }
    }
}
