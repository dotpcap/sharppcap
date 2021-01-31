using PacketDotNet;
using PacketDotNet.Utils;
using System;

namespace SharpPcap.WinDivert
{
    /// <summary>
    /// Same as RawCapture, but with extra information from the WinDivert driver
    /// </summary>
    public class WinDivertCapture : RawCapture
    {

        public uint InterfaceIndex { get; set; }
        public uint SubInterfaceIndex { get; set; }
        public WinDivertPacketFlags Flags { get; set; }

        public WinDivertCapture(PosixTimeval timeval, byte[] data) 
            : base(LinkLayers.Raw, timeval, data)
        {
        }
    }
}
