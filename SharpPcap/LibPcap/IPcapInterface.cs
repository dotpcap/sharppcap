using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace SharpPcap.LibPcap
{
    public interface IPcapInterface
    {
        /// <value>
        /// Name of the interface. Used internally when passed to pcap_open_live()
        /// </value>
        string Name { get; }

        /// <value>
        /// Human readable interface name derived from System.Net.NetworkInformation.NetworkInterface.Name
        /// </value>
        string FriendlyName { get; }

        /// <value>
        /// Text description of the interface as given by pcap/npcap
        /// </value>
        string Description { get; }

        /// <value>
        /// Gateway address of this device
        /// NOTE: May only be available on Windows
        /// </value>
        List<IPAddress> GatewayAddresses { get; }

        /// <value>
        /// Addresses associated with this device
        /// </value>
        List<PcapAddress> Addresses { get; set; }

        /// <value>
        /// Pcap interface flags
        /// </value>
        uint Flags { get; set; }

        /// <summary>
        /// MacAddress of the interface
        /// </summary>
        PhysicalAddress MacAddress { get; }

        /// <summary>
        /// Timestamps supported by this device
        /// </summary>
        ///
        /// <remarks>
        /// Note: Live devices can have supported timestamps but offline devices
        /// (such as file readers etc) do not. See https://www.tcpdump.org/manpages/pcap-tstamp.7.html
        /// </remarks>
        System.Collections.Generic.List<PcapClock> TimestampsSupported { get; }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        string ToString();

        /// <summary>
        /// Credentials to use in case of remote pcap
        /// </summary>
        RemoteAuthentication Credentials { get; }
    }
}