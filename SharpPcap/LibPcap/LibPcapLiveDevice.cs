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
 * Copyright 2008-2010 Phillip Lemon <lucidcomms@gmail.com>
 * Copyright 2008-2021 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Capture live packets from a network device
    /// </summary>
    public class LibPcapLiveDevice : PcapDevice, ILiveDevice
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

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="configuration">
        /// A <see cref="DeviceConfiguration"/>
        /// </param>
        public override void Open(DeviceConfiguration configuration)
        {
            if (Opened)
            {
                return;
            }
            var credentials = configuration.Credentials ?? Interface.Credentials;
            var mode = configuration.Mode;

            // Check if immediate is supported
            var immediate_supported = Pcap.LibpcapVersion >= new Version(1, 5, 0);
            // Check if we can do immediate by setting mintocopy to 0
            // See https://www.tcpdump.org/manpages/pcap_set_immediate_mode.3pcap.html
            var mintocopy_supported = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors

            // set the StopCaptureTimeout value to twice the read timeout to ensure that
            // we wait long enough before considering the capture thread to be stuck when stopping
            // a background capture via StopCapture()
            //
            // NOTE: Doesn't affect Mono if unix poll is available, doesn't affect Linux because
            //       Linux devices have no timeout, they always block. Only affects Windows devices.
            StopCaptureTimeout = new TimeSpan(0, 0, 0, 0, configuration.ReadTimeout * 2);

            // modes other than OpenFlags.Promiscuous require pcap_open()
            var otherModes = mode & ~DeviceModes.Promiscuous;
            if (immediate_supported || mintocopy_supported)
            {
                // We can do MaxResponsiveness through Immediate mode
                otherModes &= ~DeviceModes.MaxResponsiveness;
            }
            var immediateMode = configuration.Immediate;
            if (mode.HasFlag(DeviceModes.MaxResponsiveness))
            {
                immediateMode = true;
            }

            // Some configurations can only be used with pcap_create
            var use_pcap_create = credentials == null && (short)otherModes == 0;
            if (use_pcap_create)
            {
                Handle = LibPcapSafeNativeMethods.pcap_create(
                    Name, // name of the device
                    errbuf); // error buffer

                if (Handle.IsInvalid)
                {
                    var err = $"Unable to open the adapter '{Name}'. {errbuf}";
                    throw new PcapException(err);
                }
                // Those are configurations that pcap_open can handle differently
                Configure(
                    configuration, nameof(configuration.Snaplen),
                    LibPcapSafeNativeMethods.pcap_set_snaplen, configuration.Snaplen
                );
                Configure(
                    configuration, "Promiscuous",
                    LibPcapSafeNativeMethods.pcap_set_promisc, (int)(mode & DeviceModes.Promiscuous)
                );
                Configure(
                    configuration, nameof(configuration.ReadTimeout),
                    LibPcapSafeNativeMethods.pcap_set_timeout, configuration.ReadTimeout
                );
            }
            else
            {
                // We got authentication, so this is an rpcap device
                var auth = RemoteAuthentication.CreateAuth(credentials);
                // Immediate and MaxResponsiveness are the same thing
                if (immediateMode == true)
                {
                    mode |= DeviceModes.MaxResponsiveness;
                }
                // No need to worry about it anymore
                immediateMode = null;
                try
                {
                    Handle = LibPcapSafeNativeMethods.pcap_open(
                        Name,                               // name of the device
                        configuration.Snaplen,              // portion of the packet to capture.
                        (short)mode,                        // flags
                        (short)configuration.ReadTimeout,   // read timeout
                        ref auth,                           // authentication
                        errbuf);                            // error buffer
                }
                catch (TypeLoadException)
                {
                    var reason = credentials != null ? "Remote PCAP" : "Requested DeviceModes";
                    var err = $"Unable to open the adapter '{Name}'. {reason} not supported";
                    throw new PcapException(err, PcapError.PlatformNotSupported);
                }
            }

            if (Handle.IsInvalid)
            {
                var err = $"Unable to open the adapter '{Name}'. {errbuf}";
                throw new PcapException(err);
            }

            ConfigureIfCompatible(use_pcap_create,
                configuration, nameof(configuration.TimestampResolution),
                LibPcapSafeNativeMethods.pcap_set_tstamp_precision, (int?)configuration.TimestampResolution
            );

            ConfigureIfCompatible(use_pcap_create,
                configuration, nameof(configuration.TimestampType),
                LibPcapSafeNativeMethods.pcap_set_tstamp_type, (int?)configuration.TimestampType
            );

            ConfigureIfCompatible(use_pcap_create,
                configuration, nameof(configuration.Monitor),
                LibPcapSafeNativeMethods.pcap_set_rfmon, (int?)configuration.Monitor
            );

            ConfigureIfCompatible(use_pcap_create,
                configuration, nameof(configuration.BufferSize),
                LibPcapSafeNativeMethods.pcap_set_buffer_size, configuration.BufferSize
            );

            if (immediateMode.HasValue)
            {
                if (!immediate_supported && !mintocopy_supported)
                {
                    configuration.RaiseConfigurationFailed(
                        nameof(configuration.Immediate),
                        PcapError.PlatformNotSupported,
                        "Immediate mode not available"
                    );
                }
                else if (immediate_supported)
                {
                    var immediate = immediateMode.Value ? 1 : 0;
                    Configure(
                        configuration, nameof(configuration.Immediate),
                        LibPcapSafeNativeMethods.pcap_set_immediate_mode, immediate
                    );
                }
            }

            // pcap_open returns an already activated device
            if (use_pcap_create)
            {
                var activationResult = LibPcapSafeNativeMethods.pcap_activate(Handle);
                if (activationResult < 0)
                {
                    string err = "Unable to activate the adapter (" + Name + ").";
                    throw new PcapException(err, activationResult);
                }
            }
            base.Open(configuration);
            // retrieve the file descriptor of the adapter for use with poll()
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                FileDescriptor = LibPcapSafeNativeMethods.pcap_get_selectable_fd(Handle);
            }

            // Below configurations must be done after the device gets activated
            Configure(
                configuration, nameof(configuration.KernelBufferSize),
                LibPcapSafeNativeMethods.pcap_setbuff, configuration.KernelBufferSize
            );

            if (immediateMode == true && mintocopy_supported && !immediate_supported)
            {
                Configure(
                    configuration, nameof(configuration.Immediate),
                    LibPcapSafeNativeMethods.pcap_setmintocopy, 0
                );
            }
            Configure(
                configuration, nameof(configuration.MinToCopy),
                LibPcapSafeNativeMethods.pcap_setmintocopy, configuration.MinToCopy
            );

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
                ThrowIfNotOpen("Can't get blocking mode, the device is closed");

                var errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors
                int ret = LibPcapSafeNativeMethods.pcap_getnonblock(Handle, errbuf);

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
                ThrowIfNotOpen("Can't set blocking mode, the device is closed");

                var errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors

                int block = disableBlocking;
                if (value)
                    block = enableBlocking;

                int ret = LibPcapSafeNativeMethods.pcap_setnonblock(Handle, block, errbuf);

                // Errorbuf is only filled when ret = -1
                if (ret == -1)
                {
                    string err = "Unable to set non blocking" + errbuf.ToString();
                    throw new PcapException(err);
                }
            }
        }

        /// <summary>
        /// Sends a raw packet through this device
        /// </summary>
        /// <param name="p">The packet bytes to send</param>
        public void SendPacket(ReadOnlySpan<byte> p, ICaptureHeader header = null)
        {
            ThrowIfNotOpen("Can't send packet, the device is closed");
            int res;
            unsafe
            {
                fixed (byte* p_packet = p)
                {
                    res = LibPcapSafeNativeMethods.pcap_sendpacket(Handle, new IntPtr(p_packet), p.Length);
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

                return new PcapStatistics(Handle);
            }
        }
    }
}
