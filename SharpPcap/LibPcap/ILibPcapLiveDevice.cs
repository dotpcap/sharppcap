using System;
using System.Collections.ObjectModel;

namespace SharpPcap.LibPcap
{
    public interface ILibPcapLiveDevice : ILiveDevice
    {
        
        /// <summary>
        /// Addresses that represent this device
        /// </summary>
        ReadOnlyCollection<PcapAddress> Addresses { get; }

        /// <summary>
        /// Interface flags, see pcap_findalldevs() man page for more info
        /// </summary>
        uint Flags { get; }

        /// <summary>
        /// True if device is a loopback interface, false if not
        /// </summary>
        bool Loopback { get; }

        /// <summary>
        /// Set/Get Non-Blocking Mode. returns allways false for savefiles.
        /// </summary>
        bool NonBlockingMode { get; set; }

        /// <value>
        /// Low level pcap device values
        /// </value>
        IPcapInterface Interface { get; }

        /// <summary>
        /// Return a value indicating if this adapter is opened
        /// </summary>
        bool Opened { get; }

        /// <summary>
        /// The underlying pcap device handle
        /// </summary>
        PcapHandle Handle { get; }

        /// <summary>
        /// Override the default ToString() implementation
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        string ToString();
        
        /// <summary>
        /// Synchronously captures packets on this network device. This method will block
        /// until capturing is finished.
        /// </summary>
        /// <param name="packetCount">The number of packets to be captured.
        /// -1 means capture indefiniately</param>
        void Capture(int packetCount);
        
        /// <summary>
        /// Gets pointers to the next PCAP header and packet data.
        /// Data is only valid until next call to GetNextPacketNative.
        ///
        /// Advanced use only. Intended to allow unmanaged code to avoid the overhead of
        /// marshalling PcapHeader and packet contents to allocated memory.
        /// </summary>
        /// <returns>
        /// See https://www.tcpdump.org/manpages/pcap_next_ex.3pcap.html
        /// </returns>
        int GetNextPacketPointers(ref IntPtr header, ref IntPtr data);
        
    }
}