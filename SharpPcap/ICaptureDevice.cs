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
namespace SharpPcap
{
    /// <summary>
    /// Interfaces for capture devices
    /// </summary>
    public interface ICaptureDevice
    {
        /// <summary>
        /// Fires whenever a new packet is processed, either when the packet arrives
        /// from the network device or when the packet is read from the on-disk file.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Capture" mode.
        /// </summary>
        event PacketArrivalEventHandler OnPacketArrival;

        /// <summary>
        /// Fired when the capture process of this pcap device is stopped
        /// </summary>
        event CaptureStoppedEventHandler OnCaptureStopped;

        /// <summary>
        /// Gets the name of the device
        /// </summary>
        string Name { get; }

        /// <value>
        /// Description of the device
        /// </value>
        string Description { get; }

        /// <summary>
        /// The last pcap error associated with this pcap device
        /// </summary>
        string LastError { get; }

        /// <summary>
        /// Closes this adapter
        /// </summary>
        void Close();
    }
}

