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
 * Copyright 2010-2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using SharpPcap.LibPcap;

namespace SharpPcap.Npcap
{
    /// <summary>
    /// Npcap device
    /// </summary>
    public class NpcapDevice : LibPcapLiveDevice
    {
        private CaptureMode m_pcapMode = CaptureMode.Packets;

        /// <summary>
        /// Constructs a new PcapDevice based on a 'pcapIf' struct
        /// </summary>
        /// <param name="pcapIf">A 'pcapIf' struct representing
        /// the pcap device</param>
        public NpcapDevice(PcapInterface pcapIf) : base(pcapIf)
        { }

        /// <summary>
        /// Fires whenever a new pcap statistics is available for this Pcap Device.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Statistics" mode.
        /// </summary>
        public event StatisticsModeEventHandler OnPcapStatistics;

        /// <summary>
        /// Starts the capturing process via a background thread
        /// OnPacketArrival() will be called for each captured packet
        ///
        /// NOTE: npcap devices can capture packets or statistics updates
        ///       so only if both a packet handler AND a statistics handler
        ///       are defined will an exception be thrown
        /// </summary>
        public override void StartCapture()
        {
            if (!Started)
            {
                if (!Opened)
                    throw new DeviceNotReadyException("Can't start capture, the pcap device is not opened.");

                if ((IsOnPacketArrivalNull == true) && (OnPcapStatistics == null))
                    throw new DeviceNotReadyException("No delegates assigned to OnPacketArrival or OnPcapStatistics, no where for captured packets to go.");

                var cancellationToken = threadCancellationTokenSource.Token;
                captureThread = new Thread(() => this.CaptureThread(cancellationToken));
                captureThread.Start();
            }
        }

        /// <value>
        /// Npcap specific property
        /// </value>
        public virtual CaptureMode Mode
        {
            get
            {
                return m_pcapMode;
            }

            set
            {
                ThrowIfNotNpcap();
                ThrowIfNotOpen("Mode");

                m_pcapMode = value;
                int result = SafeNativeMethods.pcap_setmode(this.PcapHandle, (int)m_pcapMode);
                if (result < 0)
                    throw new PcapException("Error setting PcapDevice mode. : " + LastError);
            }
        }

        /// <summary>
        /// Close the device
        /// </summary>
        public override void Close()
        {
            if (OnPcapStatistics != null)
            {
                foreach (StatisticsModeEventHandler pse in OnPcapStatistics.GetInvocationList())
                {
                    OnPcapStatistics -= pse;
                }
            }

            // call the base method
            base.Close();
        }

        /// <summary>
        /// Notify the OnPacketArrival delegates about a newly captured packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="RawCapture"/>
        /// </param>
        override protected void SendPacketArrivalEvent(RawCapture p)
        {
            if (Mode == CaptureMode.Packets)
            {
                base.SendPacketArrivalEvent(p);
            }
            else if (Mode == CaptureMode.Statistics)
            {
                OnPcapStatistics?.Invoke(this, new StatisticsModeEventArgs(p, this));
            }
        }

        /// <value>
        /// Set the kernel value buffer size in bytes
        /// Npcap extension
        /// </value>
        public override uint KernelBufferSize
        {
            set
            {
                ThrowIfNotNpcap();
                ThrowIfNotOpen("Can't set kernel buffer size, the device is not opened");

                int retval = SafeNativeMethods.pcap_setbuff(this.m_pcapAdapterHandle,
                                                                    (int)value);
                if (retval != 0)
                {
                    throw new InvalidOperationException("pcap_setbuff() failed");
                }
            }

            get
            {
                throw new NotImplementedException();
            }
        }

        /// <value>
        /// Set the minumum amount of data (in bytes) received by the kernel in a single call. 
        /// Npcap extension
        /// </value>
        public int MinToCopy
        {
            set
            {
                ThrowIfNotNpcap();
                ThrowIfNotOpen("Can't set MinToCopy size, the device is not opened");

                int retval = SafeNativeMethods.pcap_setmintocopy(this.m_pcapAdapterHandle,
                                                                 value);
                if (retval != 0)
                {
                    throw new InvalidOperationException("pcap_setmintocopy() failed");
                }
            }
        }

        /// <summary>
        /// Helper method for ensuring we are running in npcap. Throws
        /// a NpcapRequiredException() if not on a windows platform
        /// </summary>
        internal static void ThrowIfNotNpcap()
        {
            if ((Environment.OSVersion.Platform != PlatformID.Win32NT) &&
               (Environment.OSVersion.Platform != PlatformID.Win32Windows))
            {
                throw new NpcapRequiredException("only supported in npcap");
            }
        }
    }
}

