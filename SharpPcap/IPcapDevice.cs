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
    /// Base interface for live and file devices
    /// </summary>
    public interface IPcapDevice : IDisposable
    {
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
        /// Kernel level filtering expression associated with this device.
        /// For more info on filter expression syntax, see:
        /// https://www.winpcap.org/docs/docs_412/html/group__language.html
        /// </summary>
        string Filter { get; set; }

        /// <summary>
        /// Mac address of the physical device
        /// </summary>
        System.Net.NetworkInformation.PhysicalAddress MacAddress { get; }

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="configuration">
        /// A <see cref="DeviceConfiguration"/>
        /// </param>
        void Open(DeviceConfiguration configuration);

        /// <summary>
        /// Closes this adapter
        /// </summary>
        void Close();

        /// <summary>
        /// Return the pcap link layer value of an adapter. 
        /// </summary>
        PacketDotNet.LinkLayers LinkType { get; }
    }
}

