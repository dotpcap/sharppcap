using PacketDotNet;
using PacketDotNet.Utils;
using System;

namespace SharpPcap.WinDivert
{
    /// <summary>
    /// Same as RawIPPacket, but allow specifying WinDivert metadata such as interface id
    /// </summary>
    public class WinDivertPacket : RawIPPacket
    {
        public uint InterfaceIndex { get; set; }
        public uint SubInterfaceIndex { get; set; }
        public WinDivertPacketFlags Flags { get; set; }

        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public WinDivertPacket(ByteArraySegment byteArraySegment)
            : base(byteArraySegment)
        {
        }

    }
}
