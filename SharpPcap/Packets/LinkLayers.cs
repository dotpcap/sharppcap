/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/* 
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
namespace SharpPcap.Packets
{
    /// <summary> Link-layer type codes.
    /// <p>
    /// Taken from libpcap/bpf/net/bpf.h and pcap/net/bpf.h.
    /// </p>
    /// <p>
    /// The link-layer type is used to determine what data-structure the 
    /// IP protocol bits will be encapsulated inside of.
    /// </p>
    /// <p>
    /// On a 10/100mbps network, packets are encapsulated inside of ethernet.
    /// 14-byte ethernet headers which contain MAC addresses and an ethernet type 
    /// field.
    /// </p>
    /// <p>
    /// On ethernet over ppp, the link-layer type is raw, and packets 
    /// are not encapsulated in any ethernet header.
    /// </p>
    /// </summary>
    public enum LinkLayers
    {
        /// <summary> no link-layer encapsulation </summary>
        Null = 0,
        /// <summary> Ethernet (10Mb) </summary>
        Ethernet10Mb = 1,
        /// <summary> Experimental Ethernet (3Mb) </summary>
        ExperimentalEthernet3MB = 2,
        /// <summary> Amateur Radio AX.25 </summary>
        AmateurRadioAX25 = 3,
        /// <summary> Proteon ProNET Token Ring </summary>
        ProteonProNetTokenRing = 4,
        /// <summary> Chaos </summary>
        Chaos = 5,
        /// <summary> IEEE 802 Networks </summary>
        Ieee802 = 6,
        /// <summary> ARCNET </summary>
        ArcNet = 7,
        /// <summary> Serial Line IP </summary>
        Slip = 8,
        /// <summary> Point-to-point Protocol </summary>
        Ppp = 9,
        /// <summary> FDDI </summary>
        Fddi = 10,
        /// <summary> LLC/SNAP encapsulated atm </summary>
        AtmRfc1483 = 11,
        /// <summary> Packet begins with an IP header </summary>
        Raw = 12,
        /// <summary> BSD Slip.</summary>
        SlipBSD = 15,
        /// <summary> BSD PPP.</summary>
        PppBSD = 16,
        /// <summary> IP over ATM.</summary>
        AtmClip = 19,
        /// <summary> PPP over HDLC.</summary>
        PppSerial = 50,
        /// <summary> Cisco HDLC.</summary>
        CiscoHDLC = 104,
        /// <summary> IEEE 802.11 wireless.</summary>
        Ieee80211 = 105,
        /// <summary> OpenBSD loopback.</summary>
        Loop = 108,
        /// <summary> Linux cooked sockets.</summary>
        LinuxSLL = 113,
        /// <summary> unknown link-layer type</summary>
        Unknown = - 1,
    }
}
