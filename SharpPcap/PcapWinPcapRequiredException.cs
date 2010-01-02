using System;

namespace SharpPcap
{
    /// <summary>
    /// Exception thrown when a WinPcap extension method is called from
    /// a non-Windows platform
    /// </summary>
    public class PcapWinPcapRequiredException : PcapException
    {
        public PcapWinPcapRequiredException(string msg) : base(msg)
        {
        }
    }
}
