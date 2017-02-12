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

namespace SharpPcap.WinPcap
{
    /// <summary>
    /// WinPcap device
    /// </summary>
    public class WinPcapDevice : LibPcap.LibPcapLiveDevice
    {
        private CaptureMode    m_pcapMode          = CaptureMode.Packets;

        /// <summary>
        /// Constructs a new PcapDevice based on a 'pcapIf' struct
        /// </summary>
        /// <param name="pcapIf">A 'pcapIf' struct representing
        /// the pcap device</param>
        internal WinPcapDevice( PcapInterface pcapIf ) : base(pcapIf)
        {}

        /// <summary>
        /// Fires whenever a new pcap statistics is available for this Pcap Device.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Statistics" mode.
        /// </summary>
        public event StatisticsModeEventHandler OnPcapStatistics;

        /// <summary>
        /// Starts the capturing process via a background thread
        /// OnPacketArrival() will be called for each captured packet
        ///
        /// NOTE: Winpcap devices can capture packets or statistics updates
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

                shouldCaptureThreadStop = false;
                captureThread = new Thread(new ThreadStart(this.CaptureThread));
                captureThread.Start();
            }
        }

        /// <summary>
        /// Open the device
        /// </summary>
        public override void Open()
        {
            base.Open();
        }

        /// <summary>
        /// Open
        /// </summary>
        /// <param name="flags">
        /// A <see cref="OpenFlags"/>
        /// </param>
        /// <param name="readTimeoutMilliseconds">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="remoteAuthentication">
        /// A <see cref="RemoteAuthentication"/>
        /// </param>
        public void Open(OpenFlags flags,
                         int readTimeoutMilliseconds,
                         RemoteAuthentication remoteAuthentication)
        {
            if(!Opened)
            {
                var errbuf = new StringBuilder( Pcap.PCAP_ERRBUF_SIZE ); //will hold errors

                IntPtr rmAuthPointer;
                if (remoteAuthentication == null)
                    rmAuthPointer = IntPtr.Zero;
                else
                    rmAuthPointer = remoteAuthentication.GetUnmanaged();

                PcapHandle = SafeNativeMethods.pcap_open(Name,
                                                         Pcap.MAX_PACKET_SIZE,   // portion of the packet to capture.
                                                         (int)flags,
                                                         readTimeoutMilliseconds,
                                                         rmAuthPointer,
                                                         errbuf);

                if(rmAuthPointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(rmAuthPointer);

                if ( PcapHandle == IntPtr.Zero)
                {
                    string err = "Unable to open the adapter ("+Name+"). "+errbuf.ToString();
                    throw new PcapException( err );
                }
            }
        }

        /// <value>
        /// WinPcap specific property
        /// </value>
        public virtual CaptureMode Mode
        {
            get
            {
                return m_pcapMode;
            }

            set
            {
                ThrowIfNotWinPcap();
                ThrowIfNotOpen("Mode");

                m_pcapMode = value;
                int result = WinPcap.SafeNativeMethods.pcap_setmode(this.PcapHandle , (int)m_pcapMode);
                if (result < 0)
                    throw new PcapException("Error setting PcapDevice mode. : " + LastError);
            }
        }

        /// <summary>
        /// Open a device with specific flags
        /// WinPcap extension - Use of this method will exclude your application
        ///                     from working on Linux or Mac
        /// </summary>
        public virtual void Open(OpenFlags flags, int read_timeout)
        {
            ThrowIfNotWinPcap();

            if(!Opened)
            {
                var errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE);

                PcapHandle = SafeNativeMethods.pcap_open
                    (   Name,                   // name of the device
                        Pcap.MAX_PACKET_SIZE,   // portion of the packet to capture.
                                                // MAX_PACKET_SIZE (65536) grants that the whole packet will be captured on all the MACs.
                        (short)flags,           // one or more flags
                        (short)read_timeout,    // read timeout
                        IntPtr.Zero,            // no authentication right now
                        errbuf );               // error buffer

                if ( PcapHandle == IntPtr.Zero)
                {
                    string err = "Unable to open the adapter ("+Name+"). "+errbuf.ToString();
                    throw new PcapException( err );
                }
            }
        }

        /// <summary>
        /// Close the device
        /// </summary>
        public override void Close()
        {
            if ( OnPcapStatistics != null)
            {
                foreach(StatisticsModeEventHandler pse in OnPcapStatistics.GetInvocationList())
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
            if(Mode == CaptureMode.Packets)
            {
                base.SendPacketArrivalEvent(p);
            }
            else if(Mode == CaptureMode.Statistics)
            {
                var handler = OnPcapStatistics;
                if(handler != null)
                {
                    //Invoke the pcap statistics event
                    handler(this, new StatisticsModeEventArgs(p, this));
                }
            }
        }

        /// <summary>
        /// Sends all packets in a 'PcapSendQueue' out this pcap device
        /// </summary>
        /// <param name="q">
        /// A <see cref="SendQueue"/>
        /// </param>
        /// <param name="transmitMode">
        /// A <see cref="SendQueueTransmitModes"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public int SendQueue( WinPcap.SendQueue q, SendQueueTransmitModes transmitMode )
        {
            return q.Transmit( this, transmitMode);
        }

        /// <value>
        /// Set the kernel value buffer size in bytes
        /// WinPcap extension
        /// </value>
        public virtual uint KernelBufferSize
        {
            set
            {
                ThrowIfNotWinPcap();
                ThrowIfNotOpen("Can't set kernel buffer size, the device is not opened");

                int retval = WinPcap.SafeNativeMethods.pcap_setbuff(this.m_pcapAdapterHandle,
                                                                    (int)value);
                if(retval != 0)
                {
                    throw new System.InvalidOperationException("pcap_setbuff() failed");
                }
            }

            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <value>
        /// Set the minumum amount of data (in bytes) received by the kernel in a single call. 
        /// WinPcap extension
        /// </value>
        public int MinToCopy
        {
            set
            {
                ThrowIfNotWinPcap();
                ThrowIfNotOpen("Can't set MinToCopy size, the device is not opened");

                int retval = WinPcap.SafeNativeMethods.pcap_setmintocopy(this.m_pcapAdapterHandle,
                                                                 value);
                if (retval != 0)
                {
                    throw new System.InvalidOperationException("pcap_setmintocopy() failed");
                }
            }
        }

        /// <summary>
        /// Helper method for ensuring we are running in winpcap. Throws
        /// a PcapWinPcapRequiredException() if not on a windows platform
        /// </summary>
        internal static void ThrowIfNotWinPcap()
        {
            if((Environment.OSVersion.Platform != PlatformID.Win32NT) &&
               (Environment.OSVersion.Platform != PlatformID.Win32Windows))
            {
                throw new WinPcapRequiredException("only supported in winpcap");
            }
        }
    }
}

