// Copyright 2011 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
namespace SharpPcap
{
    public interface IInjectionDevice : IPcapDevice
    {
        /// <summary>
        /// Sends a raw packet through this device
        /// </summary>
        /// <param name="p">The packet bytes to send</param>
        /// <param name="size">The number of bytes to send</param>
        void SendPacket(ReadOnlySpan<byte> p, ICaptureHeader header = null);
    }
}

