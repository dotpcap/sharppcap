using System;
namespace SharpPcap
{
    /// <summary>
    /// Interfaces for capture devices
    /// </summary>
    public interface ICaptureDevice
    {
        /// <summary>
        /// Fires whenever a new packet is processed, either when the packet arrives
        /// from the network device or when the packet is read from the on-disk file.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Capture" mode.
        /// </summary>
        event PacketArrivalEventHandler OnPacketArrival;

        /// <summary>
        /// Fired when the capture process of this pcap device is stopped
        /// </summary>
        event CaptureStoppedEventHandler OnCaptureStopped;

        /// <summary>
        /// Gets the name of the device
        /// </summary>
        string Name { get; }

        /// <value>
        /// Description of the device
        /// </value>
        string Description { get; }

        /// <summary>
        /// The last pcap error associated with this pcap device
        /// </summary>
        string LastError { get; }

        /// <summary>
        /// Closes this adapter
        /// </summary>
        void Close();
    }
}

