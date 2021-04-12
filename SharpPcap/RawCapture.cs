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
 * Copyright 2011 Chris Morgan <chmorgan@gmail.com>
 */

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
