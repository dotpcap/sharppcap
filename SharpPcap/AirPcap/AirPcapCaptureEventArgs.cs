using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// Class modeled after CaptureEventArgs that has
    /// AirPcap specific contents
    /// </summary>
    public class AirPcapCaptureEventArgs : EventArgs
    {
        private AirPcapRawPacket packet;

        /// <summary>
        /// Packet that was captured
        /// </summary>
        public AirPcapRawPacket Packet
        {
            get { return packet; }
        }

        private ICaptureDevice device;

        /// <summary>
        /// Device this EventArgs was generated for
        /// </summary>
        public ICaptureDevice Device
        {
            get { return device; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="packet">
        /// A <see cref="PacketDotNet.RawPacket"/>
        /// </param>
        /// <param name="device">
        /// A <see cref="PcapDevice"/>
        /// </param>
        public AirPcapCaptureEventArgs(AirPcapRawPacket packet, ICaptureDevice device)
        {
            this.packet = packet;
            this.device = device;
        }
    }
}
