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
        public LibPcapLiveDevice(PcapInterface pcapIf)
        {
            m_pcapIf = pcapIf;
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
            get { return (Flags & Pcap.PCAP_IF_LOOPBACK) == 1; }
        }

        /// <value>
        /// Set the kernel value buffer size in bytes
        /// Npcap extension
        /// </value>
        public virtual uint KernelBufferSize
        {
            set
            {
                int retval = LibPcapSafeNativeMethods.pcap_set_buffer_size(this.m_pcapAdapterHandle,
                    (int)value);
                if (retval != 0)
                {
                    throw new InvalidOperationException("pcap_set_buffer_size() failed - return value " + retval);
                }
            }
        }

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="mode">
        /// A <see cref="DeviceModes"/>
        /// </param>
        /// <param name="read_timeout">
        /// A <see cref="int"/>
        /// </param>
        /// <param name="monitor_mode">
        /// A <see cref="MonitorMode"/>
        /// </param>
        /// <param name="kernel_buffer_size">
        /// A <see cref="uint"/>
        /// </param>
        public override void Open(DeviceModes mode = DeviceModes.None, int read_timeout = 1000, MonitorMode monitor_mode = MonitorMode.Inactive, uint kernel_buffer_size = 0, RemoteAuthentication credentials = null)
        {
            if (credentials == null)
            {
                credentials = Interface.Credentials;
            }

            if (!Opened)
            {
                StringBuilder errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors

                // set the StopCaptureTimeout value to twice the read timeout to ensure that
                // we wait long enough before considering the capture thread to be stuck when stopping
                // a background capture via StopCapture()
                //
                // NOTE: Doesn't affect Mono if unix poll is available, doesn't affect Linux because
                //       Linux devices have no timeout, they always block. Only affects Windows devices.
                StopCaptureTimeout = new TimeSpan(0, 0, 0, 0, read_timeout * 2);

                // modes other than OpenFlags.Promiscuous require pcap_open()
                var otherModes = mode & ~DeviceModes.Promiscuous;
                if ((Interface.Credentials == null) || ((short)otherModes != 0))
                {
                    PcapHandle = LibPcapSafeNativeMethods.pcap_create(
                        Name, // name of the device
                        errbuf); // error buffer
                }
                else
                {
                    // We got authentication, so this is an rpcap device
                    var auth = RemoteAuthentication.CreateAuth(Name, Interface.Credentials);
                    PcapHandle = LibPcapSafeNativeMethods.pcap_open
                        (Name,                   // name of the device
                            Pcap.MAX_PACKET_SIZE,   // portion of the packet to capture.
                                                    // MAX_PACKET_SIZE (65536) grants that the whole packet will be captured on all the MACs.
                            (short)mode,           // flags
                            (short)read_timeout,    // read timeout
                            ref auth,              // authentication
                            errbuf);               // error buffer
                }

                if (PcapHandle == IntPtr.Zero)
                {
                    string err = "Unable to open the adapter (" + Name + "). " + errbuf.ToString();
                    throw new PcapException(err);
                }

                LibPcapSafeNativeMethods.pcap_set_snaplen(PcapHandle, Pcap.MAX_PACKET_SIZE);
                if (monitor_mode == MonitorMode.Active)
                {
                    try
                    {
                        LibPcapSafeNativeMethods.pcap_set_rfmon(PcapHandle, (int)monitor_mode);
                    }
                    catch (EntryPointNotFoundException)
                    {
                        throw new PcapException("This implementation of libpcap does not support monitor mode.");
                    }
                }

                LibPcapSafeNativeMethods.pcap_set_promisc(PcapHandle, (int)(mode & DeviceModes.Promiscuous));
                LibPcapSafeNativeMethods.pcap_set_timeout(PcapHandle, read_timeout);

                if (kernel_buffer_size != 0)
                    KernelBufferSize = kernel_buffer_size;

                var activationResult = LibPcapSafeNativeMethods.pcap_activate(PcapHandle);
                if (activationResult < 0)
                {
                    string err = "Unable to activate the adapter (" + Name + "). Return code: " + activationResult.ToString();
                    throw new PcapException(err);
                }
                Active = true;
                // retrieve the file descriptor of the adapter for use with poll()
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    FileDescriptor = LibPcapSafeNativeMethods.pcap_get_selectable_fd(PcapHandle);
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

                if (ret == enableBlocking)
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
        public override void SendPacket(ReadOnlySpan<byte> p)
        {
            ThrowIfNotOpen("Can't send packet, the device is closed");

            if (p.Length > Pcap.MAX_PACKET_SIZE)
            {
                throw new ArgumentException("Packet length can't be larger than " + Pcap.MAX_PACKET_SIZE);
            }
            int res;
            unsafe
            {
                fixed (byte* p_packet = p)
                {
                    res = LibPcapSafeNativeMethods.pcap_sendpacket(PcapHandle, new IntPtr(p_packet), p.Length);
                }
            }
            if (res < 0)
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
