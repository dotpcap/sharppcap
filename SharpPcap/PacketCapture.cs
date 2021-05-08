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
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 * Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
 */
using System;

namespace SharpPcap
{
    /// <summary>
    /// Packet capture data
    /// </summary>
    public readonly ref struct PacketCapture
    {
        /// <summary>
        /// Packet that was captured
        /// </summary>
        public RawCapture GetPacket() {
            return new RawCapture(Device, Header, Data);
        }

        /// <summary>
        /// Device this EventArgs was generated for
        /// </summary>
        public ICaptureDevice Device { get; }

        public ICaptureHeader Header { get; }

        public ReadOnlySpan<byte> Data { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="packet">
        /// A <see cref="RawCapture"/>
        /// </param>
        /// <param name="device">
        /// A <see cref="ICaptureDevice"/>
        /// </param>
        public PacketCapture(ICaptureDevice device, ICaptureHeader header, ReadOnlySpan<byte> data)
        {
            this.Header = header;
            this.Device = device;
            this.Data = data;
        }
    }
}
