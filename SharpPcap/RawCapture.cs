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

using System;
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
        /// Constructor
        /// </summary>
        /// <param name="LinkLayerType">
        /// A <see cref="LinkLayers"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="Data">
        /// A <see cref="System.Byte"/>
        /// </param>
        public RawCapture(LinkLayers LinkLayerType,
                          PosixTimeval Timeval,
                          byte[] Data)
        {
            this.LinkLayerType = LinkLayerType;
            this.Timeval = Timeval;
            this.Data = Data;
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
