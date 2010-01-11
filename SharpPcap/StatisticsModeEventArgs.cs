using System;

namespace SharpPcap
{
    public class PcapStatisticsModeEventArgs : CaptureEventArgs
    {        
        public PcapStatisticsModeEventArgs(Packets.Packet packet, PcapDevice device) : base(packet, device)
        {

        }

        public StatisticsModePacket Statistics
        {
            get
            {
                return new StatisticsModePacket(base.Packet);
            }
        }
    }
}
