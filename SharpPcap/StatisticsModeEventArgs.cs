using System;

namespace SharpPcap
{
    public class PcapStatisticsModeEventArgs : CaptureEventArgs
    {        
        public PcapStatisticsModeEventArgs(Packets.Packet packet, PcapDevice device) : base(packet, device)
        {

        }

        public PcapStatisticsModePacket Statistics
        {
            get
            {
                return new PcapStatisticsModePacket(base.Packet);
            }
        }
    }
}
