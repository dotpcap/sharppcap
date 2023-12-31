// Copyright 2011 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

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

