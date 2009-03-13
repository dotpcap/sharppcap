/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets
{
    /// <summary> IP protocol field encoding information.
    /// 
    /// </summary>
    public struct IPv4Fields_Fields{
        /// <summary> Width of the IP version and header length field in bytes.</summary>
        public readonly static int IP_VER_LEN = 1;

        /// <summary> Width of the TOS field in bytes.</summary>
        public readonly static int IP_TOS_LEN = 1;

        /// <summary> Width of the header length field in bytes.</summary>
        public readonly static int IP_LEN_LEN = 2;

        /// <summary> Width of the ID field in bytes.</summary>
        public readonly static int IP_ID_LEN = 2;

        /// <summary> Width of the fragmentation bits and offset field in bytes.</summary>
        public readonly static int IP_FRAG_LEN = 2;

        /// <summary> Width of the TTL field in bytes.</summary>
        public readonly static int IP_TTL_LEN = 1;

        /// <summary> Width of the IP protocol code in bytes.</summary>
        public readonly static int IP_CODE_LEN = 1;

        /// <summary> Width of the IP checksum in bytes.</summary>
        public readonly static int IP_CSUM_LEN = 2;

        /// <summary> Position of the version code and header length within the IP header.</summary>
        public readonly static int IP_VER_POS = 0;

        /// <summary> Position of the type of service code within the IP header.</summary>
        public readonly static int IP_TOS_POS;

        /// <summary> Position of the length within the IP header.</summary>
        public readonly static int IP_LEN_POS;

        /// <summary> Position of the packet ID within the IP header.</summary>
        public readonly static int IP_ID_POS;

        /// <summary> Position of the flag bits and fragment offset within the IP header.</summary>
        public readonly static int IP_FRAG_POS;

        /// <summary> Position of the ttl within the IP header.</summary>
        public readonly static int IP_TTL_POS;

        /// <summary> Position of the IP protocol code within the IP header.</summary>
        public readonly static int IP_CODE_POS;

        /// <summary> Position of the checksum within the IP header.</summary>
        public readonly static int IP_CSUM_POS;

        /// <summary> Position of the source IP address within the IP header.</summary>
        public readonly static int IP_SRC_POS;

        /// <summary> Position of the destination IP address within a packet.</summary>
        public readonly static int IP_DST_POS;

        /// <summary> Length in bytes of an IP header, excluding options.</summary>
        public readonly static int IP_HEADER_LEN; // == 20

        public readonly static int IP_ADDRESS_WIDTH = 4;

        static IPv4Fields_Fields()
        {
            IP_TOS_POS = IPv4Fields_Fields.IP_VER_POS + IPv4Fields_Fields.IP_VER_LEN;
            IP_LEN_POS = IPv4Fields_Fields.IP_TOS_POS + IPv4Fields_Fields.IP_TOS_LEN;
            IP_ID_POS = IPv4Fields_Fields.IP_LEN_POS + IPv4Fields_Fields.IP_LEN_LEN;
            IP_FRAG_POS = IPv4Fields_Fields.IP_ID_POS + IPv4Fields_Fields.IP_ID_LEN;
            IP_TTL_POS = IPv4Fields_Fields.IP_FRAG_POS + IPv4Fields_Fields.IP_FRAG_LEN;
            IP_CODE_POS = IPv4Fields_Fields.IP_TTL_POS + IPv4Fields_Fields.IP_TTL_LEN;
            IP_CSUM_POS = IPv4Fields_Fields.IP_CODE_POS + IPv4Fields_Fields.IP_CODE_LEN;
            IP_SRC_POS = IPv4Fields_Fields.IP_CSUM_POS + IPv4Fields_Fields.IP_CSUM_LEN;
            IP_DST_POS = IPv4Fields_Fields.IP_SRC_POS + IPv4Fields_Fields.IP_ADDRESS_WIDTH;
            IP_HEADER_LEN = IPv4Fields_Fields.IP_DST_POS + IPv4Fields_Fields.IP_ADDRESS_WIDTH;
        }
    }
}