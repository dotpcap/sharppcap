
namespace SharpPcap.Tunneling
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
