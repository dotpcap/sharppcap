using SharpPcap.LibPcap;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap.Tunneling.Unix
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct IfReq
    {
        /// <summary>
        /// Interface name
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        [FieldOffset(0)]
        internal string ifr_name;

        [FieldOffset(16)]
        internal short ifr_flags;

        [FieldOffset(16)]
        internal int ifr_ifindex;

        [FieldOffset(16)]
        internal int ifr_metric;

        [FieldOffset(16)]
        internal int ifr_mtu;

        [FieldOffset(16)]
        internal IntPtr ifr_data;

        [FieldOffset(16)]
        public sockaddr_in ifr_addr;

        // force total struct size to 40
        [FieldOffset(32)]
        private ulong _padding;

    }
}
