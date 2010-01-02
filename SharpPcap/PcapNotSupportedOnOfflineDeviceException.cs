using System;

namespace SharpPcap
{
    /// <summary>
    /// Thrown when a method not supported on an offline device is called
    /// </summary>
    public class PcapNotSupportedOnOfflineDeviceException : PcapException
    {
        public PcapNotSupportedOnOfflineDeviceException(string msg) : base(msg)
        {
        }
    }
}
