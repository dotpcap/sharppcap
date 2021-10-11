
namespace SharpPcap.Tunneling
{
    public class TunnelHeader : ICaptureHeader
    {
        public PosixTimeval Timeval { get; set; }

        public TunnelHeader()
        {
            Timeval = new PosixTimeval();
        }
    }
}
