using System;

namespace SharpPcap
{
    public static class CaptureDeviceExtensions
    {
        /// <summary>
        /// Defined as extension method for easier migration, since this is the most used form of Open in SharpPcap 5.x
        /// </summary>
        /// <param name="device"></param>
        /// <param name="mode"></param>
        /// <param name="read_timeout"></param>
        public static void Open(this ICaptureDevice device, DeviceModes mode = DeviceModes.None, int read_timeout = 1000)
        {
            var configuration = new DeviceConfiguration()
            {
                Mode = mode,
                ReadTimeout = read_timeout,
            };
            device.Open(configuration);
        }
    }
}
