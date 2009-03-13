/// <summary>
/// Patch to SharpPcap
///   David Bond 1/18/2009
///   Mokon@Mokon.Net
///   www.Mokon.Net
/// License
///   Distributed under the Mozilla Public License
///   http://www.mozilla.org/NPL/MPL-1.1.txt
/// Project
///   https://sourceforge.net/projects/sharppcap/
/// </summary>

namespace SharpPcap.Packets
{
    /// <summary>
    /// A struct containing length and position information about IPv6 Fields.
    /// </summary>
    public struct IPv6Fields_Fields
    {
        /// <summary>
        /// The IP Version, Traffic Class, and Flow Label field length. These must be in one
        /// field due to boundary crossings.
        /// </summary>
        public readonly static int LINE_ONE_LEN = 4;

        /// <summary>
        /// The payload length field length.
        /// </summary>
        public readonly static int PAYLOAD_LENGTH_LEN = 2;

        /// <summary>
        /// The next header field length.
        /// </summary>
        public readonly static int NEXT_HEADER_LEN = 1;

        /// <summary>
        /// The hop limit field length.
        /// </summary>
        public readonly static int HOP_LIMIT_LEN = 1;

        /// <summary>
        /// The source address field length.
        /// </summary>
        public readonly static int SRC_ADDRESS_LEN = 16;

        /// <summary>
        /// The destination address field length.
        /// </summary>
        public readonly static int DST_ADDRESS_LEN = 16;

        /// <summary>
        /// The byte position of the field line in the IPv6 header.
        /// This is where the IP version, Traffic Class, and Flow Label fields are.
        /// </summary>
        public readonly static int LINE_ONE_POS = 0;

        /// <summary>
        /// The byte position of the payload length field.
        /// </summary>
        public readonly static int PAYLOAD_LENGTH_POS;

        /// <summary>
        /// The byte position of the next header field. (Replaces the ipv4 protocol field)
        /// </summary>
        public readonly static int NEXT_HEADER_POS;

        /// <summary>
        /// The byte position of the hop limit field.
        /// </summary>
        public readonly static int HOP_LIMIT_POS;

        /// <summary>
        /// The byte position of the source address field.
        /// </summary>
        public readonly static int SRC_ADDRESS_POS;

        /// <summary>
        /// The byte position of the destination address field.
        /// </summary>
        public readonly static int DST_ADDRESS_POS;

        /// <summary>
        /// The byte length of the IPv6 Header
        /// </summary>
        public readonly static int IPv6_HEADER_LEN; // == 40

        /// <summary>
        /// Commutes the field positions.
        /// </summary>
        static IPv6Fields_Fields( )
        {
            PAYLOAD_LENGTH_POS = IPv6Fields_Fields.LINE_ONE_POS + IPv6Fields_Fields.LINE_ONE_LEN;
            NEXT_HEADER_POS = IPv6Fields_Fields.PAYLOAD_LENGTH_POS + IPv6Fields_Fields.PAYLOAD_LENGTH_LEN;
            HOP_LIMIT_POS = IPv6Fields_Fields.NEXT_HEADER_POS + IPv6Fields_Fields.NEXT_HEADER_LEN;
            SRC_ADDRESS_POS = IPv6Fields_Fields.HOP_LIMIT_POS + IPv6Fields_Fields.HOP_LIMIT_LEN;
            DST_ADDRESS_POS = IPv6Fields_Fields.SRC_ADDRESS_POS + IPv6Fields_Fields.SRC_ADDRESS_LEN;
            IPv6_HEADER_LEN = IPv6Fields_Fields.DST_ADDRESS_POS + IPv6Fields_Fields.DST_ADDRESS_LEN;
        }
    }
}