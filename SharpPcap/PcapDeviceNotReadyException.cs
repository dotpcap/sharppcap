using System;

namespace SharpPcap
{
    /// <summary>
    /// A PcapDevice or dumpfile is not ready for capture operations.
    /// </summary>
    public class PcapDeviceNotReadyException : PcapException
    {
        internal PcapDeviceNotReadyException() : base()
        {
        }

        internal PcapDeviceNotReadyException(string msg) : base(msg)
        {
        }
    }
}
