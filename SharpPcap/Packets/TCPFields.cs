// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************
namespace SharpPcap.Packets
{
    /// <summary> IP protocol field encoding information.
    /// 
    /// </summary>
    public struct TCPFields_Fields
    {
        // flag bitmasks
        public readonly static int TCP_CWR_MASK = 0x0080;
        public readonly static int TCP_ECN_MASK = 0x0040;
        public readonly static int TCP_URG_MASK = 0x0020;
        public readonly static int TCP_ACK_MASK = 0x0010;
        public readonly static int TCP_PSH_MASK = 0x0008;
        public readonly static int TCP_RST_MASK = 0x0004;
        public readonly static int TCP_SYN_MASK = 0x0002;
        public readonly static int TCP_FIN_MASK = 0x0001;
        /// <summary> Length of a TCP port in bytes.</summary>
        public readonly static int TCP_PORT_LEN = 2;
        /// <summary> Length of the sequence number in bytes.</summary>
        public readonly static int TCP_SEQ_LEN = 4;
        /// <summary> Length of the acknowledgment number in bytes.</summary>
        public readonly static int TCP_ACK_LEN = 4;
        /// <summary> Length of the header length and flags field in bytes.</summary>
        public readonly static int TCP_FLAG_LEN = 2;
        /// <summary> Length of the window size field in bytes.</summary>
        public readonly static int TCP_WIN_LEN = 2;
        /// <summary> Length of the checksum field in bytes.</summary>
        public readonly static int TCP_CSUM_LEN = 2;
        /// <summary> Length of the urgent field in bytes.</summary>
        public readonly static int TCP_URG_LEN = 2;
        /// <summary> Position of the source port field.</summary>
        public readonly static int TCP_SP_POS = 0;
        /// <summary> Position of the destination port field.</summary>
        public readonly static int TCP_DP_POS;
        /// <summary> Position of the sequence number field.</summary>
        public readonly static int TCP_SEQ_POS;
        /// <summary> Position of the acknowledgment number field.</summary>
        public readonly static int TCP_ACK_POS;
        /// <summary> Position of the header length and flags field.</summary>
        public readonly static int TCP_FLAG_POS;
        /// <summary> Position of the window size field.</summary>
        public readonly static int TCP_WIN_POS;
        /// <summary> Position of the checksum field.</summary>
        public readonly static int TCP_CSUM_POS;
        /// <summary> Position of the urgent pointer field.</summary>
        public readonly static int TCP_URG_POS;
        /// <summary> Length in bytes of a TCP header.</summary>
        public readonly static int TCP_HEADER_LEN; // == 20
        static TCPFields_Fields()
        {
            TCP_DP_POS = TCPFields_Fields.TCP_PORT_LEN;
            TCP_SEQ_POS = TCPFields_Fields.TCP_DP_POS + TCPFields_Fields.TCP_PORT_LEN;
            TCP_ACK_POS = TCPFields_Fields.TCP_SEQ_POS + TCPFields_Fields.TCP_SEQ_LEN;
            TCP_FLAG_POS = TCPFields_Fields.TCP_ACK_POS + TCPFields_Fields.TCP_ACK_LEN;
            TCP_WIN_POS = TCPFields_Fields.TCP_FLAG_POS + TCPFields_Fields.TCP_FLAG_LEN;
            TCP_CSUM_POS = TCPFields_Fields.TCP_WIN_POS + TCPFields_Fields.TCP_WIN_LEN;
            TCP_URG_POS = TCPFields_Fields.TCP_CSUM_POS + TCPFields_Fields.TCP_CSUM_LEN;
            TCP_HEADER_LEN = TCPFields_Fields.TCP_URG_POS + TCPFields_Fields.TCP_URG_LEN;
        }
    }
}
