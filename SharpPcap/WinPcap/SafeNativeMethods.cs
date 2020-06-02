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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpPcap.WinPcap
{
    /// <summary>
    /// Per http://msdn.microsoft.com/en-us/ms182161.aspx 
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        // NOTE: For mono users on non-windows platforms a .config file is used to map
        //       the windows dll name to the unix/mac library name
        //       This file is called $assembly_name.dll.config and is placed in the
        //       same directory as the assembly
        //       See http://www.mono-project.com/Interop_with_Native_Libraries#Library_Names
        private const string PCAP_DLL = "wpcap";

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        static SafeNativeMethods()
        {
            if ((Environment.OSVersion.Platform != PlatformID.MacOSX) &&
               (Environment.OSVersion.Platform != PlatformID.Unix))
            {
                SetDllDirectory(Path.Combine(Environment.SystemDirectory, "Npcap"));
            }
        }

        #region WinPcap specific

        /// <summary>
        /// Set the working mode of the interface p to mode. 
        /// Valid values for mode are MODE_CAPT (default capture mode) 
        /// and MODE_STAT (statistical mode). See the tutorial 
        /// "\ref wpcap_tut9" for details about statistical mode.
        /// WinPcap specific method
        /// </summary>
        [DllImport(PCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_setmode(IntPtr/* pcap_t * */ p, int mode);

        /// <summary>
        /// WinPcap specific method for setting the kernel buffer size
        /// associated with this adapter. The old buffer is discarded
        /// when the buffer size is changed.
        /// See http://www.winpcap.org/docs/docs_40_2/html/group__wpcapfunc.html
        /// </summary>
        /// <param name="adapter">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <param name="bufferSizeInBytes">
        /// A <see cref="int"/>
        /// </param>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_setbuff(IntPtr /* pcap_t */ adapter, int bufferSizeInBytes);

        /// <summary>
        /// changes the minimum amount of data in the kernel buffer that causes 
        /// a read from the application to return (unless the timeout expires)
        /// See http://www.winpcap.org/docs/docs_412/html/group__wpcapfunc.html#gab14ceacbf1c2f63026416dd73f80dc0d
        /// </summary>
        /// <param name="adapter">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <param name="sizeInBytes">
        /// A <see cref="int"/>
        /// </param>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_setmintocopy(IntPtr /* pcap_t */ adapter, int sizeInBytes);

        #endregion
    }
}

