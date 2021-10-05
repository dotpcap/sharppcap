
namespace SharpPcap.WinTap
{
    public class WinTapHeader : ICaptureHeader
    {
        public PosixTimeval Timeval { get; set; }

        public WinTapHeader()
        {
            Timeval = new PosixTimeval();
        }
    }
}
