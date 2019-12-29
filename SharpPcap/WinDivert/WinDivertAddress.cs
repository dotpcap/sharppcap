using System;
using System.Runtime.InteropServices;

namespace SharpPcap.WinDivert
{
    /// <summary>
    /// The WinDivertAddress structure represents the "address" of a captured or injected packet. The
    /// address includes the packet's timestamp, network interfaces, direction and other information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WinDivertAddress
    {
        public long Timestamp;
        public byte Layer;
        public byte Event;
        public byte Flags;
        public uint IfIdx;
        public uint SubIfIdx;
    }
}