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
    public interface ICaptureDevice : IPcapDevice
    {

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
        /// <param name="e"></param>
        /// <returns>Status of the operation</returns>
        GetPacketStatus GetNextPacket(out PacketCapture e);

        /// <summary>
        /// Retrieves pcap statistics
        ///
        /// Devices that lack statistics support return null
        /// </summary>
        ICaptureStatistics Statistics { get; }

        #region Timestamp
        /// <summary>
        /// Note: libpcap docs and code use the term 'precision' and 'resolution'. We use the term
        /// 'resolution' as it more closely reflects this setting
        ///
        /// Note: It isn't possible to set the resolution on a device that has already
        /// been activated. Please Open() the device with the desired resolution.
        /// </summary>
        TimestampResolution TimestampResolution { get; }
        #endregion
    }
}

