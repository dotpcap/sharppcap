using System;

namespace SharpPcap
{
    /// <summary>
    /// A PcapDevice or dumpfile is not ready for capture operations.
    /// </summary>
    public class DeviceNotReadyException : PcapException
    {
        internal DeviceNotReadyException() : base()
        {
        }

        internal DeviceNotReadyException(string msg) : base(msg)
        {
        }
    }
}
