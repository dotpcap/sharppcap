using System;

namespace SharpPcap
{
    /// <summary>A delegate for Packet Arrival events</summary>
    public delegate void PacketArrivalEventHandler(object sender, PcapCaptureEventArgs e);
}
