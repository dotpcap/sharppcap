using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPcap
{
    public class PcapCaptureEventArgs : EventArgs
    {
        private Packets.Packet packet;

        public PcapCaptureEventArgs(Packets.Packet packet)
        {
            this.packet = packet;
        }

        public Packets.Packet Packet
        {
            get
            {
                return packet;
            }
        }
    }

    public class PcapStatisticsEventArgs : PcapCaptureEventArgs
    {        
        public PcapStatisticsEventArgs(Packets.Packet packet) : base(packet)
        {

        }

        public PcapStatistics Statistics
        {
            get
            {
                return new PcapStatistics(base.Packet);
            }
        }
    }

}
