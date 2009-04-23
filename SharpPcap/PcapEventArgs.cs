using System;

namespace SharpPcap
{
    public class PcapCaptureEventArgs : EventArgs
    {
        private Packets.Packet packet;
        public Packets.Packet Packet
        {
            get { return packet; }
        }

        private PcapDevice device;
        public PcapDevice Device
        {
            get { return device; }
        }

        public PcapCaptureEventArgs(Packets.Packet packet, PcapDevice device)
        {
            this.packet = packet;
            this.device = device;
        }

    }

    public class PcapStatisticsEventArgs : PcapCaptureEventArgs
    {        
        public PcapStatisticsEventArgs(Packets.Packet packet, PcapDevice device) : base(packet, device)
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
