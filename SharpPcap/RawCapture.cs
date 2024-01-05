// Copyright 2011 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using PacketDotNet;

namespace SharpPcap
{
    /// <summary>
    /// Represents a raw captured packet
    /// </summary>
    public class RawCapture
    {
        /// <value>
        /// Link layer from which this packet was captured
        /// </value>
        public LinkLayers LinkLayerType
        {
            get;
            set;
        }

        /// <value>
        /// The unix timeval when the packet was created
        /// </value>
        public PosixTimeval Timeval
        {
            get;
            set;
        }

        /// <summary> Fetch data portion of the packet.</summary>
        ///
        /// Data as a class field vs. a virtual property improves performance
        /// significantly. ~2.5% when parsing the packet with Packet.Net and
        /// ~20% when reading each byte of the packet
        public byte[] Data;

        /// <summary>
        /// The length of the packet on the line
        /// </summary>
        public int PacketLength { get; set; }

        /// <summary>
        /// Creates a Packet object from the LinkLayerType and Data
        /// </summary>
        /// <returns></returns>
        public virtual Packet GetPacket()
        {
            return Packet.ParsePacket(LinkLayerType, Data);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LinkLayerType">
        /// A <see cref="LinkLayers"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="Data">
        /// A <see cref="byte"/>
        /// </param>
        public RawCapture(LinkLayers LinkLayerType,
                          PosixTimeval Timeval,
                          byte[] Data,
                          int? packetLength = null)
        {
            this.LinkLayerType = LinkLayerType;
            this.Timeval = Timeval;
            this.Data = Data;
            this.PacketLength = packetLength ?? Data?.Length ?? 0;
        }

        public RawCapture(ICaptureDevice device, ICaptureHeader header, System.ReadOnlySpan<byte> data)
        {
            this.LinkLayerType = device.LinkType;
            this.Timeval = header.Timeval;
            this.Data = data.ToArray();
            this.PacketLength = Data?.Length ?? 0;
        }

        /// <summary>Output this packet as a readable string</summary>
        public override System.String ToString()
        {
            var buffer = new System.Text.StringBuilder();

            // build the output string
            buffer.AppendFormat("[RawCapture: LinkLayerType={0}, Timeval={1}]",
                LinkLayerType,
                Timeval);

            return buffer.ToString();
        }
    }
}
