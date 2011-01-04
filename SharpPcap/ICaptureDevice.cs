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
        /// http://www.winpcap.org/docs/docs31/html/group__language.html
        /// </summary>
        string Filter { get; set; }

        /// <summary>
        /// Retrieves pcap statistics
        /// </summary>
        ICaptureStatistics Statistics { get; }

#region Dump related
        /// <summary>
        /// Gets a value indicating whether a dump file is already associated with this device
        /// </summary>
        bool DumpOpened { get; }

        /// <summary>
        /// Writes a packet to the pcap dump file associated with this device.
        /// </summary>
        /// <param name="p">The packet to write</param>
        void Dump(PacketDotNet.RawPacket p);

        /// <summary>
        /// Opens a file for packet writings
        /// </summary>
        /// <param name="fileName"></param>
        void DumpOpen(string fileName);

        /// <summary>
        /// Closes the opened dump file
        /// </summary>
        void DumpClose();

        /// <summary>
        /// Flushes all write buffers of the opened dump file
        /// </summary>
        void DumpFlush();
#endregion

        /// <summary>
        /// Opens the adapter
        /// </summary>
        void Open();

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="mode">
        /// A <see cref="DeviceMode"/>
        /// </param>
        void Open(DeviceMode mode);

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="mode">
        /// A <see cref="DeviceMode"/>
        /// </param>
        /// <param name="read_timeout">
        /// A <see cref="System.Int32"/>
        /// </param>
        void Open(DeviceMode mode, int read_timeout);

        /// <summary>
        /// Closes this adapter
        /// </summary>
        void Close();

        #region Capture methods and properties
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
        /// Return a value indicating if the capturing process of this adapter is started
        /// </summary>
        bool Started { get; }

        /// <summary>
        /// Maximum time within which the capture thread must join the main thread (on
        /// <see cref="StopCapture"/>) or else the thread is aborted and an exception thrown.
        /// </summary>
        TimeSpan StopCaptureTimeout { get; set; }

        /// <summary>
        /// Start the capture
        /// </summary>
        void StartCapture();

        /// <summary>
        /// Stop the capture
        /// </summary>
        void StopCapture();

        /// <summary>
        /// Synchronously capture packets on this device. Method blocks forever.
        /// </summary>
        void Capture();

        #endregion

        /// <summary>
        /// Retrieves the next packet from a device
        /// </summary>
        /// <returns></returns>
        PacketDotNet.RawPacket GetNextPacket();

        /// <summary>
        /// Sends a raw packet throgh this device
        /// </summary>
        /// <param name="p">The packet to send</param>
        void SendPacket(PacketDotNet.Packet p);

        /// <summary>
        /// Sends a raw packet throgh this device
        /// </summary>
        /// <param name="p">The packet to send</param>
        /// <param name="size">The number of bytes to send</param>
        void SendPacket(PacketDotNet.Packet p, int size);

        /// <summary>
        /// Sends a raw packet throgh this device
        /// </summary>
        /// <param name="p">The packet bytes to send</param>
        void SendPacket(byte[] p);

        /// <summary>
        /// Sends a raw packet throgh this device
        /// </summary>
        /// <param name="p">The packet bytes to send</param>
        /// <param name="size">The number of bytes to send</param>
        void SendPacket(byte[] p, int size);

        /// <summary>
        /// Return the pcap link layer value of an adapter. 
        /// </summary>
        PacketDotNet.LinkLayers LinkType { get; }
    }
}

