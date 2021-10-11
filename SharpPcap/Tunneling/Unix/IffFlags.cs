using System;

namespace SharpPcap.Tunneling.Unix
{
    [Flags]
    internal enum IffFlags : short
    {
        Tun = 0x0001,
        Tap = 0x0002,
        NoPi = 0x1000,
    }
}
