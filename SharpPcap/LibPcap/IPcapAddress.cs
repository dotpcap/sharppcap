namespace SharpPcap.LibPcap
{
    public interface IPcapAddress
    {
        /// <summary>
        /// The address value of this PcapAddress, null if none is present
        /// </summary>
        Sockaddr Addr { get; }

        /// <summary>
        /// Netmask of this PcapAddress, null if none is present
        /// </summary>
        Sockaddr Netmask { get; }

        /// <summary>
        /// Broadcast address of this PcapAddress, null if none is present
        /// </summary>
        Sockaddr Broadaddr { get; }

        /// <summary>
        /// Destination address, null if the interface isn't a point-to-point interface
        /// </summary>
        Sockaddr Dstaddr { get; }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        string ToString();
    }
}