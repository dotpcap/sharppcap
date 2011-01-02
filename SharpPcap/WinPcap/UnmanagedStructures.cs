using System;
using System.Runtime.InteropServices;

namespace SharpPcap.WinPcap
{
    /// <summary>
    /// WinPcap specific unmanaged structures
    /// </summary>
    public class UnmanagedStructures
    {
        #region Unmanaged Structs Implementation

        /// <summary>
        /// Struct to specifiy Remote Address using rpcapd.exe, Winpcaps Remote Packet Capture Daemon
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)] //Note: Pack =1 cause problems with Win 7 64b
        public struct pcap_rmtauth
        {
            /// <summary>
            /// NOTE: IntPtr used to ensure that the correct data size is used depending on
            /// the platform being used, 32bits on a 32bit machine, 64bits on a 64bit machine
            ///
            /// Auth Type, 0=Null, 1= Password
            /// </summary>
            public IntPtr       type;
            /// <summary>
            /// Username
            /// </summary>
            public string   username;

            /// <summary>
            /// Password
            /// </summary>
            public string   password;
        }

        #endregion
    }
}

