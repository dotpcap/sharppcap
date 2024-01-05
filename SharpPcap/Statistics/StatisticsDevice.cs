// Copyright 2010-2011 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using PacketDotNet;
using SharpPcap.LibPcap;

namespace SharpPcap.Statistics
{
    /// <summary>
    /// Npcap device
    /// </summary>
    public class StatisticsDevice : IPcapDevice
    {
        private readonly LibPcapLiveDevice LiveDevice;

        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Constructs a new PcapDevice based on a 'pcapIf' struct
        /// </summary>
        /// <param name="pcapIf">A 'pcapIf' struct representing
        /// the pcap device</param>
        public StatisticsDevice(PcapInterface pcapIf)
        {
            LiveDevice = new LibPcapLiveDevice(pcapIf);
        }

        /// <summary>
        /// Fires whenever a new pcap statistics is available for this Pcap Device.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Statistics" mode.
        /// </summary>
        public event EventHandler<StatisticsEventArgs> OnPcapStatistics;

        /// <summary>
        /// Starts the capturing process via a background thread
        /// OnPcapStatistics() will be called for each statistics update
        /// </summary>
        public void StartCapture()
        {
            PrepareCapture();
            LiveDevice.StartCapture();
        }

        public void Capture()
        {
            PrepareCapture();
            LiveDevice.Capture();
        }

        public void Capture(int packetCount)
        {
            PrepareCapture();
            LiveDevice.Capture(packetCount);
        }

        private void PrepareCapture()
        {
            if (OnPcapStatistics == null)
            {
                throw new DeviceNotReadyException("No delegates assigned to OnPcapStatistics, no where for captured packets to go.");
            }
            ReceivedPackets = 0;
            ReceivedBytes = 0;
            // prevent handler from being added twice
            LiveDevice.OnPacketArrival -= LiveDevice_OnPacketArrival;
            LiveDevice.OnPacketArrival += LiveDevice_OnPacketArrival;
        }

        public void StopCapture()
        {
            LiveDevice.OnPacketArrival -= LiveDevice_OnPacketArrival;
            LiveDevice.StopCapture();
        }

        long ReceivedPackets;
        long ReceivedBytes;
        private void LiveDevice_OnPacketArrival(object sender, PacketCapture e)
        {
            var packet = e.GetPacket();
            if (IsWindows)
            {
                ReceivedPackets += BitConverter.ToInt64(packet.Data, 0);
                ReceivedBytes += BitConverter.ToInt64(packet.Data, 8);
            }
            else
            {
                ReceivedPackets++;
                ReceivedBytes += packet.PacketLength;
            }
            var args = new StatisticsEventArgs(
                this,
                packet.Timeval,
                ReceivedPackets,
                ReceivedBytes
            );

            OnPcapStatistics?.Invoke(this, args);
        }

        public string Name => LiveDevice.Name;

        public string Description => LiveDevice.Description;

        public string LastError => LiveDevice.LastError;

        public string Filter
        {
            get => LiveDevice.Filter;
            set => LiveDevice.Filter = value;
        }

        /// <summary>
        /// Retrieves pcap statistics
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public ICaptureStatistics Statistics => LiveDevice.Statistics;

        public PhysicalAddress MacAddress => LiveDevice.MacAddress;

        public LinkLayers LinkType => LiveDevice.LinkType;

        public void Open(DeviceConfiguration configuration)
        {
            LiveDevice.Open(configuration);
            if (IsWindows)
            {
                LibPcapSafeNativeMethods.pcap_setmode(LiveDevice.Handle, (int)CaptureMode.Statistics);
            }
        }

        /// <summary>
        /// Close the device
        /// </summary>
        public void Close()
        {
            OnPcapStatistics = null;
            StopCapture();
            LiveDevice.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}

