// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
// Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
//
// SPDX-License-Identifier: MIT

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
