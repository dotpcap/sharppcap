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
        virtual public EthernetPacketType EthernetProtocol
        {
            get
            {
                return (EthernetPacketType)ArrayHelper.extractInteger(_bytes,
                                                                      EthernetFields_Fields.ETH_CODE_POS,
                                                                      EthernetFields_Fields.ETH_CODE_LEN);
            }

            set
            {
                ArrayHelper.insertInt16(_bytes, (ushort)value,
                                        EthernetFields_Fields.ETH_CODE_POS);
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
        protected internal int _ethPayloadOffset;

        // time that the packet was captured off the wire
        protected internal Timeval _timeval;


        /// <summary>
        /// Construct a new ethernet packet from source and destination mac addresses
        /// </summary>
        public EthernetPacket(PhysicalAddress SourceHwAddress,
                              PhysicalAddress DestinationHwAddress,
                              EthernetPacketType ethernetPacketType,
                              byte[] EthernetPayload)
        {
            int ethernetPayloadLength = 0;
            if(EthernetPayload != null)
            {
                ethernetPayloadLength = EthernetPayload.Length;
            }

            _bytes = new byte[EthernetFields_Fields.ETH_HEADER_LEN + ethernetPayloadLength];
            _ethernetHeaderLength = EthernetFields_Fields.ETH_HEADER_LEN;
            _ethPayloadOffset = _ethernetHeaderLength;

            // if we have a payload, copy it into the byte array
            if(EthernetPayload != null)
            {
                Array.Copy(EthernetPayload, 0, _bytes, EthernetFields_Fields.ETH_HEADER_LEN, EthernetPayload.Length);
            }

            // set the instance values
            this.SourceHwAddress = SourceHwAddress;
            this.DestinationHwAddress = DestinationHwAddress;
            this.EthernetProtocol = ethernetPacketType;
        }

        /// <summary> Construct a new ethernet packet.
        /// <p>
        /// When the type of ethernet packet is 
        /// recognized as a protocol for which a class exists network library, 
        /// then a more specific class like IPPacket or ARPPacket is instantiated.
        /// The subclass can always be cast into a more generic form.
        /// </summary>
        public EthernetPacket(int byteOffsetToEthernetPayload, byte[] bytes)
        {
            _bytes = bytes;
            _ethernetHeaderLength = byteOffsetToEthernetPayload;
            _ethPayloadOffset = byteOffsetToEthernetPayload;
        }

        /// <summary> Construct a new ethernet packet, including the capture time.</summary>
        public EthernetPacket(int byteOffsetToEthernetPayload, byte[] bytes, Timeval tv)
            : this(byteOffsetToEthernetPayload, bytes)
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
            buffer.Append(" proto=0x" + System.Convert.ToString((ushort)EthernetProtocol, 16));
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
