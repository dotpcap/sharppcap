using System.Runtime.InteropServices;

namespace SharpPcap.Tunneling.Unix
{
    [StructLayout(LayoutKind.Explicit)]
   internal struct Ifreq
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        [FieldOffset(0)]
        internal string ifr_name; /* Interface name */

        [FieldOffset(16)]
        internal short ifr_flags;

        [FieldOffset(16)]
        internal int ifr_ifindex;

        [FieldOffset(16)]
        internal int ifr_metric;

        [FieldOffset(16)]
        internal int ifr_mtu;

        // force total struct size to 40
        [FieldOffset(32)]
        private ulong _padding;
    };
}
