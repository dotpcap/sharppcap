using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// Type of decryption the adapter performs.
    /// An adapter can be instructed to turn decryption (based on the device-configured keys configured 
    /// with \ref AirpcapSetDeviceKeys()) on or off.
    /// </summary>
    public enum AirPcapDecryptionState : int
    {
        ///<summary>This adapter performs decryption</summary>
        DecryptionOn = 1,
        ///<summary>This adapter does not perform decryption</summary>
        DecryptionOff = 2
    };
}
