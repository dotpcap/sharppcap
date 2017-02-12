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
 * Copyright 2008-2010 Phillip Lemon <lucidcomms@gmail.com>
 */

using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Capture live packets from a network device
    /// </summary>
    public class LibPcapLiveDevice : PcapDevice
    {
        /// <summary>
        /// Constructs a new PcapDevice based on a 'pcapIf' struct
        /// </summary>
        /// <param name="pcapIf">A 'pcapIf' struct representing
        /// the pcap device</param>
        internal LibPcapLiveDevice( PcapInterface pcapIf )
        {
            m_pcapIf = pcapIf;

            // go through the network interfaces and attempt to populate the mac address, 
            // friendly name etc of this device
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                // if the name and id match then we have found the NetworkInterface
                // that matches the PcapDevice
                if (Name.EndsWith(adapter.Id))
                {
                    var ipProperties = adapter.GetIPProperties();
                    if (ipProperties.GatewayAddresses.Count != 0)
                    {
                        Interface.GatewayAddress = ipProperties.GatewayAddresses[0].Address;
                    }

                    Interface.MacAddress = adapter.GetPhysicalAddress();
                    Interface.FriendlyName = adapter.Name;
                }
            }
        }

        /// <summary>
        /// Default contructor for subclasses
        /// </summary>
        protected LibPcapLiveDevice()
        {
        }

        /// <summary>
        /// PcapDevice finalizer.  Ensure PcapDevices are stopped and closed before exit.
        /// </summary>
        ~LibPcapLiveDevice()
        {
            this.Close();
        }

        /// <summary>
        /// Gets the pcap name of this network device
        /// </summary>
        public override string Name
        {
            get { return m_pcapIf.Name; }
        }

        /// <summary>
        /// Addresses that represent this device
        /// </summary>
        public virtual ReadOnlyCollection<PcapAddress> Addresses
        {
            get { return new ReadOnlyCollection<PcapAddress>(m_pcapIf.Addresses); }
        }

        /// <summary>
        /// Gets the pcap description of this device
        /// </summary>
        public override string Description
        {
            get { return m_pcapIf.Description; }
        }

        /// <summary>
        /// Interface flags, see pcap_findalldevs() man page for more info
        /// </summary>
        public virtual uint Flags
        {
            get { return m_pcapIf.Flags; }
        }

        /// <summary>
        /// True if device is a loopback interface, false if not
        /// </summary>
        public virtual bool Loopback
        {
            get { return (Flags & Pcap.PCAP_IF_LOOPBACK)==1; }
        }

        /// <summary>
        /// Open the device with default values of: promiscuous_mode = false, read_timeout = 1000
        /// To start capturing call the 'StartCapture' function
        /// </summary>
        public override void Open()
        {
            this.Open(DeviceMode.Normal);
        }

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="mode">
        /// A <see cref="DeviceMode"/>
        /// </param>
        public override void Open(DeviceMode mode)
        {
            const int readTimeoutMilliseconds = 1000;
            this.Open(mode, readTimeoutMilliseconds);
        }

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="mode">
        /// A <see cref="DeviceMode"/>
        /// </param>
        /// <param name="read_timeout">
        /// A <see cref="System.Int32"/>
        /// </param>
        public override void Open(DeviceMode mode, int read_timeout)
        {
            if ( !Opened )
            {
                StringBuilder errbuf = new StringBuilder( Pcap.PCAP_ERRBUF_SIZE ); //will hold errors

                // set the StopCaptureTimeout value to twice the read timeout to ensure that
                // we wait long enough before considering the capture thread to be stuck when stopping
                // a background capture via StopCapture()
                //
                // NOTE: Doesn't affect Mono if unix poll is available, doesn't affect Linux because
                //       Linux devices have no timeout, they always block. Only affects Windows devices.
                StopCaptureTimeout = new TimeSpan(0, 0, 0, 0, read_timeout * 2);

                PcapHandle = LibPcapSafeNativeMethods.pcap_open_live
                    (   Name,                   // name of the device
                        Pcap.MAX_PACKET_SIZE,   // portion of the packet to capture. 
                                                // MAX_PACKET_SIZE (65536) grants that the whole packet will be captured on all the MACs.
                        (int)mode,              // promiscuous mode
                        read_timeout,           // read timeout
                        errbuf );               // error buffer

                if ( PcapHandle == IntPtr.Zero)
                {
                    string err = "Unable to open the adapter ("+Name+"). "+errbuf.ToString();
                    throw new PcapException( err );
                }
            }
        }

        private const int disableBlocking = 0;
        private const int enableBlocking = 1;

        /// <summary>
        /// Set/Get Non-Blocking Mode. returns allways false for savefiles.
        /// </summary>
        public bool NonBlockingMode
        {
            get
            {
                var errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors
                int ret = LibPcapSafeNativeMethods.pcap_getnonblock(PcapHandle, errbuf);

                // Errorbuf is only filled when ret = -1
                if (ret == -1)
                {
                    string err = "Unable to set get blocking" + errbuf.ToString();
                    throw new PcapException(err);
                }

                if(ret == enableBlocking)
                    return true;
                return false;
            }
            set 
            {
                var errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors

                int block = disableBlocking;
                if (value)
                    block = enableBlocking;

                int ret = LibPcapSafeNativeMethods.pcap_setnonblock(PcapHandle, block, errbuf);

                // Errorbuf is only filled when ret = -1
                if (ret == -1)
                {
                    string err = "Unable to set non blocking" + errbuf.ToString();
                    throw new PcapException(err);
                }
            }
        }

        /// <summary>
        /// Sends a raw packet throgh this device
        /// </summary>
        /// <param name="p">The packet bytes to send</param>
        /// <param name="size">The number of bytes to send</param>
        public override void SendPacket(byte[] p, int size)
        {
            ThrowIfNotOpen("Can't send packet, the device is closed");

            if (size > p.Length)
            {
                throw new ArgumentException("Invalid packetSize value: "+size+
                "\nArgument size is larger than the total size of the packet.");
            }

            if (p.Length > Pcap.MAX_PACKET_SIZE) 
            {
                throw new ArgumentException("Packet length can't be larger than "+Pcap.MAX_PACKET_SIZE);
            }

            IntPtr p_packet = IntPtr.Zero;          
            p_packet = Marshal.AllocHGlobal( size );
            Marshal.Copy(p, 0, p_packet, size);     

            int res = LibPcapSafeNativeMethods.pcap_sendpacket(PcapHandle, p_packet, size);
            Marshal.FreeHGlobal(p_packet);
            if(res < 0)
            {
                throw new PcapException("Can't send packet: " + LastError);
            }
        }

        /// <summary>
        /// Retrieves pcap statistics
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public override ICaptureStatistics Statistics
        {
            get
            {
                // can only call PcapStatistics on an open device
                ThrowIfNotOpen("device not open");

                return new PcapStatistics(this.m_pcapAdapterHandle);
            }
        }
    }
}
