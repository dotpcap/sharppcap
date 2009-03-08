/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
using AnsiEscapeSequences = SharpPcap.Packets.Util.AnsiEscapeSequences;
using ArrayHelper = SharpPcap.Packets.Util.ArrayHelper;
using Timeval = SharpPcap.Packets.Util.Timeval;
using SharpPcap.Packets.Util;

namespace SharpPcap.Packets
{
    /// <summary> An ARP protocol packet.
    /// <p>
    /// Extends an ethernet packet, adding ARP header information and an ARP 
    /// data payload. 
    /// 
    /// </summary>
    [Serializable]
    public class ARPPacket : EthernetPacket, ARPFields
    {
        virtual public int ARPHwType
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ethOffset + ARPFields_Fields.ARP_HW_TYPE_POS, ARPFields_Fields.ARP_ADDR_TYPE_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ethOffset + ARPFields_Fields.ARP_HW_TYPE_POS, ARPFields_Fields.ARP_ADDR_TYPE_LEN);
            }
        }

        virtual public int ARPProtocolType
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ethOffset + ARPFields_Fields.ARP_PR_TYPE_POS, ARPFields_Fields.ARP_ADDR_TYPE_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ethOffset + ARPFields_Fields.ARP_PR_TYPE_POS, ARPFields_Fields.ARP_ADDR_TYPE_LEN);
            }
        }

        virtual public int ARPHwLength
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ethOffset + ARPFields_Fields.ARP_HW_LEN_POS, ARPFields_Fields.ARP_ADDR_SIZE_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ethOffset + ARPFields_Fields.ARP_HW_LEN_POS, ARPFields_Fields.ARP_ADDR_SIZE_LEN);
            }
        }

        virtual public int ARPProtocolLength
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ethOffset + ARPFields_Fields.ARP_PR_LEN_POS, ARPFields_Fields.ARP_ADDR_SIZE_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ethOffset + ARPFields_Fields.ARP_PR_LEN_POS, ARPFields_Fields.ARP_ADDR_SIZE_LEN);
            }
        }

        /// <summary> Fetch the operation code.
        /// Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
        /// </summary>
        /// <summary> Sets the operation code.
        /// Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
        /// </summary>
        virtual public int ARPOperation
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ethOffset + ARPFields_Fields.ARP_OP_POS, ARPFields_Fields.ARP_OP_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ethOffset + ARPFields_Fields.ARP_OP_POS, ARPFields_Fields.ARP_OP_LEN);
            }
        }

        /// <summary> Fetch the hardware source address.</summary>
        virtual public long ARPSenderHwAddressAsLong
        {
            get
            {
                return ArrayHelper.extractLong(Bytes, _ethOffset + ARPFields_Fields.ARP_S_HW_ADDR_POS, 6);
            }
        }

        /// <summary> Fetch the proto sender address.</summary>
        /// <summary> Sets the proto sender address.</summary>
        virtual public System.Net.IPAddress ARPSenderProtoAddress
        {
            get
            {
                return IPPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             _ethOffset + ARPFields_Fields.ARP_S_PR_ADDR_POS,
                                             Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0, Bytes, _ethOffset + ARPFields_Fields.ARP_S_PR_ADDR_POS, address.Length);
            }
        }

        /// <summary> Fetch the proto sender address.</summary>
        /// <summary> Sets the proto sender address.</summary>
        virtual public System.Net.IPAddress ARPTargetProtoAddress
        {
            get
            {
                return IPPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             _ethOffset + ARPFields_Fields.ARP_T_PR_ADDR_POS,
                                             Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0, Bytes, _ethOffset + ARPFields_Fields.ARP_T_PR_ADDR_POS, address.Length);
            }
        }

        /// <summary> Fetch the arp header, excluding arp data payload.</summary>
        virtual public byte[] ARPHeader
        {
            get
            {
                return PacketEncoding.extractHeader(_ethOffset, ARPFields_Fields.ARP_HEADER_LEN, Bytes);
            }
        }

        /// <summary> Fetch data portion of the arp header.</summary>
        virtual public byte[] ARPData
        {
            get
            {
                return PacketEncoding.extractData(_ethOffset, ARPFields_Fields.ARP_HEADER_LEN, Bytes);
            }
        }

        /// <summary> Fetch the arp header, excluding arp data payload.</summary>
        override public byte[] Header
        {
            get
            {
                return ARPHeader;
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences_Fields.PURPLE;
            }
        }

        /// <summary> Create a new ARP packet.</summary>
        public ARPPacket(int lLen, byte[] bytes)
            : base(lLen, bytes)
        {
        }

        /// <summary> Create a new ARP packet.</summary>
        public ARPPacket(int lLen, byte[] bytes, Timeval tv)
            : base(lLen, bytes, tv)
        {
        }

        /// <summary> Gets/Sets the hardware source address.</summary>
        public virtual System.String ARPSenderHwAddress
        {
            get
            {
                return MACAddress.extract(_ethOffset + ARPFields_Fields.ARP_S_HW_ADDR_POS, Bytes);
            }
            set
            {
                MACAddress.insert(value, _ethOffset + ARPFields_Fields.ARP_S_HW_ADDR_POS, Bytes);
            }
        }

        /// <summary> Sets the hardware source address.</summary>
        public virtual void setARPSenderHwAddress(long addr)
        {
            ArrayHelper.insertLong(Bytes, addr, _ethOffset + ARPFields_Fields.ARP_S_HW_ADDR_POS, 6);
        }

        /// <summary> Gets/Sets the hardware destination address.</summary>
        public virtual String ARPTargetHwAddress
        {
            get
            {
                return MACAddress.extract(_ethOffset + ARPFields_Fields.ARP_T_HW_ADDR_POS, Bytes);
            }
            set
            {
                MACAddress.insert(value, _ethOffset + ARPFields_Fields.ARP_T_HW_ADDR_POS, Bytes);
            }
        }

        /// <summary> Sets the hardware destination address.</summary>
        public virtual void setARPTargetHwAddress(long addr)
        {
            ArrayHelper.insertLong(Bytes, addr, _ethOffset + ARPFields_Fields.ARP_T_HW_ADDR_POS, 6);
        }

        /// <summary> Fetch data portion of the arp header.</summary>
        public override byte[] Data
        {
            get
            {
                return ARPData;
            }
        }

        /// <summary> Convert this ARP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this ARP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("ARPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences_Fields.RESET);
            buffer.Append(": ");
            buffer.Append(ARPOperation == ARPFields_Fields.ARP_OP_REQ_CODE ? "request" : "reply");
            buffer.Append(' ');
            buffer.Append(ARPSenderHwAddress + " -> " + ARPTargetHwAddress);
            buffer.Append(", ");
            buffer.Append(ARPSenderProtoAddress + " -> " + ARPTargetProtoAddress);
            //buffer.append(" l=" + header.length + "," + data.length);
            buffer.Append(']');

            return buffer.ToString();
        }
    }
}
