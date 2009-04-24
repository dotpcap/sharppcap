/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
using System.Net.NetworkInformation;
using SharpPcap.Packets.Util;
using ArrayHelper = SharpPcap.Packets.Util.ArrayHelper;
using Timeval = SharpPcap.Packets.Util.Timeval;
namespace SharpPcap.Packets
{
    /// <summary> An ethernet packet.
    /// <p>
    /// Contains link-level header and data payload encapsulated by an ethernet
    /// packet.
    /// <p>
    /// There are currently two subclasses. IP and ARP protocols are supported.
    /// IPPacket extends with ip header and data information.
    /// ARPPacket extends with hardware and protocol addresses.
    /// 
    /// </summary>
    [Serializable]
    public class EthernetPacket : Packet, EthernetFields
    {
        /// <summary> Code constants for well-defined ethernet protocols.
        /// <p>
        /// Taken from linux/if_ether.h and tcpdump/ethertype.h
        /// 
        /// </summary>
        public struct EtherType
        {
            /// <summary> IP protocol.</summary>
            public const int IP = 0x0800;
            /// <summary> Address resolution protocol.</summary>
            public const int ARP = 0x0806;
            /// <summary> Reverse address resolution protocol.</summary>
            public const int RARP = 0x8035;
            /// <summary> Ethernet Loopback packet </summary>
            public const int LOOP = 0x0060;
            /// <summary> Ethernet Echo packet      </summary>
            public const int ECHO = 0x0200;
            /// <summary> Xerox PUP packet</summary>
            public const int PUP = 0x0400;
            /// <summary> CCITT X.25            </summary>
            public const int X25 = 0x0805;
            /// <summary> G8BPQ AX.25 Ethernet Packet   [ NOT AN OFFICIALLY REGISTERED ID ] </summary>
            public const int BPQ = 0x08FF;
            /// <summary> DEC Assigned proto</summary>
            public const int DEC = 0x6000;
            /// <summary> DEC DNA Dump/Load</summary>
            public const int DNA_DL = 0x6001;
            /// <summary> DEC DNA Remote Console</summary>
            public const int DNA_RC = 0x6002;
            /// <summary> DEC DNA Routing</summary>
            public const int DNA_RT = 0x6003;
            /// <summary> DEC LAT</summary>
            public const int LAT = 0x6004;
            /// <summary> DEC Diagnostics</summary>
            public const int DIAG = 0x6005;
            /// <summary> DEC Customer use</summary>
            public const int CUST = 0x6006;
            /// <summary> DEC Systems Comms Arch</summary>
            public const int SCA = 0x6007;
            /// <summary> Appletalk DDP </summary>
            public const int ATALK = 0x809B;
            /// <summary> Appletalk AARP</summary>
            public const int AARP = 0x80F3;
            /// <summary> IPX over DIX</summary>
            public const int IPX = 0x8137;
            /// <summary> IPv6 over bluebook</summary>
            public const int IPV6 = 0x86DD;
            /// <summary> Dummy type for 802.3 frames  </summary>
            public const int N802_3 = 0x0001;
            /// <summary> Dummy protocol id for AX.25  </summary>
            public const int AX25 = 0x0002;
            /// <summary> Every packet.</summary>
            public const int ALL = 0x0003;
            /// <summary> 802.2 frames</summary>
            public const int N802_2 = 0x0004;
            /// <summary> Internal only</summary>
            public const int SNAP = 0x0005;
            /// <summary> DEC DDCMP: Internal only</summary>
            public const int DDCMP = 0x0006;
            /// <summary> Dummy type for WAN PPP frames</summary>
            public const int WAN_PPP = 0x0007;
            /// <summary> Dummy type for PPP MP frames </summary>
            public const int PPP_MP = 0x0008;
            /// <summary> Localtalk pseudo type </summary>
            public const int LOCALTALK = 0x0009;
            /// <summary> Dummy type for Atalk over PPP</summary>
            public const int PPPTALK = 0x0010;
            /// <summary> 802.2 frames</summary>
            public const int TR_802_2 = 0x0011;
            /// <summary> Mobitex (kaz@cafe.net)</summary>
            public const int MOBITEX = 0x0015;
            /// <summary> Card specific control frames</summary>
            public const int CONTROL = 0x0016;
            /// <summary> Linux/IR</summary>
            public const int IRDA = 0x0017;
            // others not yet documented..
            
            public const int NS = 0x0600;
            public const int SPRITE = 0x0500;
            public const int TRAIL = 0x1000;
            public const int LANBRIDGE = 0x8038;
            public const int DECDNS = 0x803c;
            public const int DECDTS = 0x803e;
            public const int VEXP = 0x805b;
            public const int VPROD = 0x805c;
            public const int N8021Q = 0x8100;
            public const int PPP = 0x880b;
            public const int PPPOED = 0x8863;
            public const int PPPOES = 0x8864;
            public const int LOOPBACK = 0x9000;
            // spanning tree bridge protocol
            public const int STBPDU = 0x0026;
            // intel adapter fault tolerance heartbeats
            public const int INFTH = 0x886d;
            /// <summary> Ethernet protocol mask.</summary>
            public const int MASK = 0xffff;
        }

        /// <summary> Extract the protocol type field from packet data.
        /// <p>
        /// The type field indicates what type of data is contained in the 
        /// packet's data block.
        /// </summary>
        /// <param name="packetBytes">packet bytes.
        /// </param>
        /// <returns> the ethernet type code. i.e. 0x800 signifies IP datagram.
        /// </returns>
        public static int extractProtocol(byte[] packetBytes)
        {
            // convert the bytes that contain the type code into a value..
            return packetBytes[EthernetFields_Fields.ETH_CODE_POS] << 8 | packetBytes[EthernetFields_Fields.ETH_CODE_POS + 1];
        }

        /// <summary> Fetch the ethernet header length in bytes.</summary>
        virtual public int EthernetHeaderLength
        {
            get
            {
                return _ethernetHeaderLength;
            }

        }
        /// <summary> Fetch the packet ethernet header length.</summary>
        virtual public int HeaderLength
        {
            get
            {
                return EthernetHeaderLength;
            }

        }
        /// <summary> Fetch the ethernet header as a byte array.</summary>
        virtual public byte[] EthernetHeader
        {
            get
            {
                return PacketEncoding.extractHeader(0, EthernetHeaderLength, _bytes);
            }

        }
        /// <summary> Fetch the ethernet header as a byte array.</summary>
        override public byte[] Header
        {
            get
            {
                return EthernetHeader;
            }

        }
        /// <summary> Fetch the ethernet data as a byte array.</summary>
        virtual public byte[] EthernetData
        {
            get
            {
                return PacketEncoding.extractData(0, EthernetHeaderLength, _bytes);
            }

        }

        /// <summary> Fetch the ethernet protocol.</summary>
        /// <summary> Sets the ethernet protocol.</summary>
        virtual public int EthernetProtocol
        {
            get
            {
                return ArrayHelper.extractInteger(_bytes, EthernetFields_Fields.ETH_CODE_POS, EthernetFields_Fields.ETH_CODE_LEN);
            }

            set
            {
                ArrayHelper.insertLong(_bytes, value, EthernetFields_Fields.ETH_CODE_POS, EthernetFields_Fields.ETH_CODE_LEN);
            }

        }

        /// <summary>
        ///  should be overriden by upper classes
        /// </summary>
        public virtual void OnOffsetChanged()
        {
            if(PcapHeader!=null)
            {
                PcapHeader = new PcapHeader(PcapHeader.Seconds,
                                            PcapHeader.MicroSeconds,
                                            (uint)_bytes.Length,
                                            (uint)_bytes.Length);
            }
        }

        /// <summary> Fetch the timeval containing the time the packet arrived on the 
        /// device where it was captured.
        /// </summary>
        override public Timeval Timeval
        {
            get
            {
                return _timeval;
            }

        }
        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences_Fields.DARK_GRAY;
            }

        }
        override public byte[] Bytes
        {
            get
            {
                return _bytes;
            }
            protected set
            {
                _bytes = value;
            }

        }
        // store the data here, all subclasses can offset into this
        private byte[] _bytes;

        // offset from beginning of byte array where the data payload 
        // (i.e. IP packet) starts. The size of the ethernet frame header.
        protected internal int _ethOffset;

        // time that the packet was captured off the wire
        protected internal Timeval _timeval;


        /// <summary> Construct a new ethernet packet.
        /// <p>
        /// For the purpose of jpcap, when the type of ethernet packet is 
        /// recognized as a protocol for which a class exists network library, 
        /// then a more specific class like IPPacket or ARPPacket is instantiated.
        /// The subclass can always be cast into a more generic form.
        /// </summary>
        public EthernetPacket(int lLen, byte[] bytes)
        {
            _bytes = bytes;
            _ethernetHeaderLength = lLen;
            _ethOffset = lLen;
        }

        /// <summary> Construct a new ethernet packet, including the capture time.</summary>
        public EthernetPacket(int lLen, byte[] bytes, Timeval tv)
            : this(lLen, bytes)
        {
            this._timeval = tv;
        }

        // set in constructor
        private int _ethernetHeaderLength;

        /// <summary> Fetch the ethernet data as a byte array.</summary>
        public override byte[] Data
        {
            get
            {
                return EthernetData;
            }
        }

        private static int macAddressLength = 6;

        /// <summary> Fetch the MAC address of the host where the packet originated from.</summary>
        public virtual PhysicalAddress SourceHwAddress
        {
            get
            {
                byte[] hwAddress = new byte[macAddressLength];
                Array.Copy(_bytes, EthernetFields_Fields.ETH_SRC_POS,
                           hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }
            set
            {
                byte[] hwAddress = value.GetAddressBytes();
                if(hwAddress.Length != macAddressLength)
                {
                    throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                               + " not equal to the expected length of "
                                                               + macAddressLength);
                }

                Array.Copy(hwAddress, 0, _bytes, EthernetFields_Fields.ETH_SRC_POS,
                           hwAddress.Length);
            }
        }

        /// <summary> Fetch the MAC address of the host where the packet originated from.</summary>
        public virtual PhysicalAddress DestinationHwAddress
        {
            get
            {
                byte[] hwAddress = new byte[macAddressLength];
                Array.Copy(_bytes, EthernetFields_Fields.ETH_DST_POS,
                           hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }
            set
            {
                byte[] hwAddress = value.GetAddressBytes();
                if(hwAddress.Length != macAddressLength)
                {
                    throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                               + " not equal to the expected length of "
                                                               + macAddressLength);
                }

                Array.Copy(hwAddress, 0, _bytes, EthernetFields_Fields.ETH_DST_POS,
                           hwAddress.Length);
            }
        }

        /// <summary> Convert this ethernet packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this ethernet packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("EthernetPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences_Fields.RESET);
            buffer.Append(": ");
            buffer.Append(SourceHwAddress + " -> " + DestinationHwAddress);
            buffer.Append(" proto=0x" + System.Convert.ToString(EthernetProtocol, 16));
            buffer.Append(" l=" + EthernetHeaderLength); // + "," + data.length);
            buffer.Append(']');

            // append the base output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary> Convert this IP packet to a more verbose string.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            //TODO: just output the colored output for now
            return ToColoredString(colored);
        }
    }
}
