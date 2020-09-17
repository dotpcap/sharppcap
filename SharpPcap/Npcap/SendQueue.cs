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
using SharpPcap.LibPcap;

namespace SharpPcap.Npcap
{
    /// <summary>
    /// Interface to the Npcap send queue extension methods
    /// </summary>
    [Obsolete("Obsolete. Use LibPcap namespace instead.")]
    public class SendQueue : LibPcap.SendQueue
    {
        /// <summary>
        /// Creates and allocates a new SendQueue
        /// </summary>
        /// <param name="memSize">
        /// The maximun amount of memory (in bytes) 
        /// to allocate for the queue</param>
        public SendQueue(int memSize)
            : base(memSize)
        {
        }

        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet bytes to add</param>
        /// <returns>True if success, else false</returns>
        public bool Add(byte[] packet)
        {
            return SendQueueExtensions.Add(this, packet);
        }

        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet to add</param>
        /// <returns>True if success, else false</returns>
        public bool Add(RawCapture packet)
        {
            return SendQueueExtensions.Add(this, packet);
        }

        /// <summary>
        /// Add a packet to this send queue.
        /// </summary>
        /// <param name="packet">The packet to add</param>
        /// <param name="seconds">The 'seconds' part of the packet's timestamp</param>
        /// <param name="microseconds">The 'microseconds' part of the packet's timestamp</param>
        /// <returns>True if success, else false</returns>
        public bool Add(byte[] packet, int seconds, int microseconds)
        {
            return SendQueueExtensions.Add(this, packet, seconds, microseconds);
        }

        /// <summary>
        /// Send a queue of raw packets to the network. 
        /// </summary>
        /// <param name="device">
        /// The device on which to send the queue
        /// A <see cref="PcapDevice"/>
        /// </param>
        /// <param name="transmitMode">
        /// A <see cref="SendQueueTransmitModes"/>
        /// </param>
        /// <returns>
        /// The number of bytes sent as an <see cref="int"/>
        /// </returns>
        public int Transmit(NpcapDevice device, SendQueueTransmitModes transmitMode)
        {
            return Transmit(device, transmitMode == SendQueueTransmitModes.Synchronized);
        }
    }
}
