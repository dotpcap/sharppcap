// Copyright 2011 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

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

