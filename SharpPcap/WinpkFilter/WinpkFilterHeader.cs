
namespace SharpPcap.WinpkFilter
{
    public class WinpkFilterHeader : ICaptureHeader
    {
        public PosixTimeval Timeval { get; set; }
        public PacketSource Source { get; set; }
        public uint Dot1q { get; set; }

        public WinpkFilterHeader()
        {
            Timeval = new PosixTimeval();
        }
    }
}
