using System;

namespace SharpPcap
{
    /// <summary>
    /// Thrown when a method not supported on an offline device is called
    /// </summary>
    public class NotSupportedOnOfflineDeviceException : PcapException
    {
        public NotSupportedOnOfflineDeviceException(string msg) : base(msg)
        {
        }
    }
}
