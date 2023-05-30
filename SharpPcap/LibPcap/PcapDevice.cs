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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Base class for all pcap devices
    /// </summary>
    public abstract partial class PcapDevice : ICaptureDevice
    {
        /// <summary>
        /// Low level interface object that contains device specific information
        /// </summary>
        protected PcapInterface m_pcapIf;

        /// <summary>
        /// Number of packets that this adapter should capture
        /// </summary>
        protected int m_pcapPacketCount = Pcap.InfinitePacketCount;

        /// <summary>
        /// Device name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Description
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Fires whenever a new packet is processed, either when the packet arrives
        /// from the network device or when the packet is read from the on-disk file.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Capture" mode.
        /// </summary>
        public event PacketArrivalEventHandler OnPacketArrival;

        /// <summary>
        /// Fired when the capture process of this pcap device is stopped
        /// </summary>
        public event CaptureStoppedEventHandler OnCaptureStopped;

        /// <value>
        /// Low level pcap device values
        /// </value>
        public PcapInterface Interface
        {
            get { return m_pcapIf; }
        }

        private PacketDotNet.LinkLayers linkType;

        /// <summary>
        /// Return a value indicating if this adapter is opened
        /// </summary>
        public virtual bool Opened
        {
            get { return !(Handle.IsInvalid || Handle.IsClosed); }
        }

        /// <summary>
        /// The file descriptor obtained from pcap_fileno
        /// Used for polling
        /// </summary>
        protected internal int FileDescriptor = -1;

        /// <summary>
        /// The underlying pcap device handle
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public PcapHandle Handle { get; protected set; } = PcapHandle.Invalid;

        /// <summary>
        /// Retrieve the last error string for a given pcap_t* device
        /// </summary>
        /// <param name="deviceHandle">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        internal static string GetLastError(PcapHandle deviceHandle)
        {
            return LibPcapSafeNativeMethods.pcap_geterr(deviceHandle);
        }

        /// <summary>
        /// The last pcap error associated with this pcap device
        /// </summary>
        public virtual string LastError
        {
            get { return GetLastError(Handle); }
        }

        /// <summary>
        /// Link type in terms of PacketDotNet.LinkLayers
        /// </summary>
        public virtual PacketDotNet.LinkLayers LinkType
        {
            get
            {
                ThrowIfNotOpen("Cannot get datalink, the pcap device is not opened");
                return linkType;
            }
        }

        /// <summary>
        /// Open the device. To start capturing call the 'StartCapture' function
        /// </summary>
        /// <param name="configuration">
        /// A <see cref="DeviceConfiguration"/>
        /// </param>
        public virtual void Open(DeviceConfiguration configuration)
        {
            // Caches linkType value.
            // Open refers to the device being "created"
            // This method is called by sub-classes in the override method
            int dataLink = 0;
            if (Opened)
            {
                dataLink = LibPcapSafeNativeMethods.pcap_datalink(Handle);
            }
            if (dataLink >= 0)
            {
                linkType = (PacketDotNet.LinkLayers)dataLink;
            }
        }

        /// <summary>
        /// Closes this adapter
        /// </summary>
        public virtual void Close()
        {
            //Remove event handlers
            OnPacketArrival = null;
            if (Started)
            {
                try
                {
                    StopCapture();
                }
                catch (Exception) { }
            }
            Handle.Close();
            Handle = PcapHandle.Invalid;
        }

        /// <summary>
        /// Retrieves pcap statistics
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public abstract ICaptureStatistics Statistics { get; }

        /// <summary>
        /// Mac address of the physical device
        /// </summary>
        public virtual System.Net.NetworkInformation.PhysicalAddress MacAddress
        {
            get
            {
                return Interface.MacAddress;
            }
        }

        /// <summary>
        /// Notify the OnPacketArrival delegates about a newly captured packet
        /// </summary>
        /// <param name="header"></param>
        /// <param name="data"></param>
        protected virtual void SendPacketArrivalEvent(PcapHeader header, Span<byte> data)
        {
            OnPacketArrival?.Invoke(this, new PacketCapture(this, header, data));
        }

        /// <summary>
        /// Notify the delegates that are subscribed to the capture stopped event
        /// </summary>
        /// <param name="status">
        /// A <see cref="CaptureStoppedEventStatus"/>
        /// </param>
        protected virtual void SendCaptureStoppedEvent(CaptureStoppedEventStatus status)
        {
            OnCaptureStopped?.Invoke(this, status);
        }

        /// <summary>
        /// Retrieve the next packet data
        /// </summary>
        /// <param name="e">Structure to hold the packet data info</param>
        /// <returns>Status of the operation</returns>
        public virtual GetPacketStatus GetNextPacket(out PacketCapture e)
        {
            //Pointer to a packet info struct
            IntPtr header = IntPtr.Zero;

            //Pointer to a packet struct
            IntPtr data = IntPtr.Zero;

            // using an invalid PcapHandle can result in an unmanaged segfault
            // so check for that here
            ThrowIfNotOpen("Device must be opened via Open() prior to use");

            // If a user is calling GetNextPacket() when the background capture loop
            // is also calling into libpcap then bad things can happen
            //
            // The bad behavior I (Chris M.) saw was that the background capture would keep running
            // but no more packets were captured. Took two days to debug and regular users
            // may hit the issue more often so check and report the issue here
            if (Started)
            {
                throw new InvalidOperationDuringBackgroundCaptureException("GetNextPacket() invalid during background capture");
            }

            if (!PollFileDescriptor())
            {
                e = default;

                // We checked, there is no data using poll()
                return GetPacketStatus.ReadTimeout;
            }

            int res;

            unsafe
            {
                //Get a packet from npcap
                res = LibPcapSafeNativeMethods.pcap_next_ex(Handle, ref header, ref data);
                var pcapHeader = PcapHeader.FromPointer(header, TimestampResolution);
                var dataSpan = new Span<byte>(data.ToPointer(), (int)pcapHeader.CaptureLength);

                e = new PacketCapture(this, pcapHeader, dataSpan);
            }

            return (GetPacketStatus)res;
        }

        /// <summary>
        /// Gets pointers to the next PCAP header and packet data.
        /// Data is only valid until next call to GetNextPacketNative.
        ///
        /// Advanced use only. Intended to allow unmanaged code to avoid the overhead of
        /// marshalling PcapHeader and packet contents to allocated memory.
        /// </summary>
        /// <returns>
        /// See https://www.tcpdump.org/manpages/pcap_next_ex.3pcap.html
        /// </returns>
        public int GetNextPacketPointers(ref IntPtr header, ref IntPtr data)
        {
            return LibPcapSafeNativeMethods.pcap_next_ex(Handle, ref header, ref data);
        }

        /// <summary>
        /// Pcap_loop callback method.
        /// </summary>
        protected virtual void PacketHandler(IntPtr handlePtr, IntPtr /* pcap_pkthdr* */ header, IntPtr data)
        {
            var handle = Handle;
            if (handle.DangerousGetHandle() != handlePtr)
            {
                // The handle changed, we are being called from a soon to be dead thread.
                return;
            }
            var gotRef = false;
            try
            {
                // Make sure that handle does not get closed until this function is done
                // See https://github.com/chmorgan/sharppcap/issues/343
                handle.DangerousAddRef(ref gotRef);
                if (!gotRef)
                {
                    return;
                }
                unsafe
                {
                    var pcapHeader = PcapHeader.FromPointer(header, TimestampResolution);
                    var dataSpan = new Span<byte>(data.ToPointer(), (int)pcapHeader.CaptureLength);
                    SendPacketArrivalEvent(pcapHeader, dataSpan);
                }
            }
            catch (ObjectDisposedException)
            {
                // If Dispose was called in another thread, DangerousAddRef will throw this
                // Ignore
            }
            finally
            {
                if (gotRef)
                {
                    handle.DangerousRelease();
                }
            }
        }

        /// <summary>
        /// Convert an unmanaged packet into a managed PacketDotNet.RawPacket
        /// </summary>
        /// <param name="header">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <param name="data">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="RawCapture"/>
        /// </returns>
        protected virtual RawCapture MarshalRawPacket(IntPtr /* pcap_pkthdr* */ header, IntPtr data)
        {
            RawCapture p;

            // marshal the header
            var pcapHeader = PcapHeader.FromPointer(header, TimestampResolution);

            var pkt_data = new byte[pcapHeader.CaptureLength];
            Marshal.Copy(data, pkt_data, 0, (int)pcapHeader.CaptureLength);

            p = new RawCapture(LinkType, pcapHeader.Timeval,
                               pkt_data, (int)pcapHeader.PacketLength);

            return p;
        }

        #region Filtering
        /// <summary>
        /// Assign a filter to this device given a filterExpression
        /// </summary>
        /// <param name="filterExpression">The filter expression to compile</param>
        protected void SetFilter(string filterExpression)
        {
            // save the filter string
            _filterString = filterExpression;

            int res;

            // pcap_setfilter() requires a valid pcap_t which isn't present if
            // the device hasn't been opened
            ThrowIfNotOpen("device is not open");

            // attempt to compile the program
            var bpfProgram = BpfProgram.Create(Handle, filterExpression);

            //associate the filter with this device
            res = LibPcapSafeNativeMethods.pcap_setfilter(Handle, bpfProgram);

            // Free the program whether or not we were successful in setting the filter
            // we don't want to leak unmanaged memory if we throw an exception.
            bpfProgram.Dispose();

            //watch for errors
            if (res < 0)
            {
                var errorString = string.Format("Can't set filter ({0}) : {1}", filterExpression, LastError);
                throw new PcapException(errorString);
            }
        }

        private string _filterString;

        /// <summary>
        /// Kernel level filtering expression associated with this device.
        /// For more info on filter expression syntax, see:
        /// https://www.tcpdump.org/manpages/pcap-filter.7.html
        /// </summary>
        public virtual string Filter
        {
            get
            {
                return _filterString;
            }

            set
            {
                SetFilter(value);
            }
        }

        /// <summary>
        /// Returns true if the filter expression was able to be compiled into a
        /// program without errors
        /// </summary>
        public static bool CheckFilter(string filterExpression,
                                       out string errorString)
        {
            errorString = null;
            using (var pcapHandle = LibPcapSafeNativeMethods.pcap_open_dead((int)PacketDotNet.LinkLayers.Ethernet, Pcap.MAX_PACKET_SIZE))
            {
                var bpfProgram = BpfProgram.TryCreate(pcapHandle, filterExpression);
                if (bpfProgram == null)
                {
                    errorString = GetLastError(pcapHandle);
                    return false;
                }
                else
                {
                    bpfProgram.Dispose();
                    return true;
                }
            }
        }
        #endregion

        #region Timestamp
        /// <summary>
        /// To set a device's timestamp resolution pass the desired setting in when opening the device
        /// </summary>
        /// <remarks>
        /// The name mismatch between resolution and precision is intentional. To the end user
        /// we use the more correct term of 'resolution' but we have to match the libpcap term of 'precision'
        /// used in functions for proper pinvoke mapping.
        /// </remarks>
        public virtual TimestampResolution TimestampResolution
        {
            get
            {
                ThrowIfNotOpen("device is not open");
                return (TimestampResolution)LibPcapSafeNativeMethods.pcap_get_tstamp_precision(Handle);
            }
        }

        #endregion

        /// <summary>
        /// Most pcap configuration functions have the signature int pcap_set_foo(pcap_t, int)
        /// those functions also set the error buffer, so we read it
        /// This is a helper method to use them and detect/report errors
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="setter"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        protected void Configure(
            DeviceConfiguration configuration,
            string property,
            Func<PcapHandle, int, PcapError> setter,
            int? value
        )
        {
            if (value.HasValue)
            {
                var retval = setter(Handle, value.Value);
                if (retval != 0)
                {
                    configuration.RaiseConfigurationFailed(property, retval, GetLastError(Handle));
                }
            }
        }

        protected internal void ConfigureIfCompatible(
            bool compatible,
            DeviceConfiguration configuration,
            string property,
            Func<PcapHandle, int, PcapError> setter,
            int? value
        )
        {
            if (!value.HasValue)
            {
                return;
            }
            if (!compatible)
            {
                configuration.RaiseConfigurationFailed(
                    property, PcapError.Generic,
                    $"Can not configure {property} with current device and selected modes"
                );
            }
            else
            {
                Configure(configuration, property, setter, value);
            }
        }

        /// <summary>
        /// Helper method for checking that the adapter is open, throws an
        /// exception with a string of ExceptionString if the device isn't open
        /// </summary>
        /// <param name="ExceptionString">
        /// A <see cref="string"/>
        /// </param>
        protected void ThrowIfNotOpen(string ExceptionString)
        {
            if (!Opened)
            {
                throw new DeviceNotReadyException(ExceptionString);
            }
        }

        /// <summary>
        /// Override the default ToString() implementation
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        public override string ToString()
        {
            return "interface: " + m_pcapIf.ToString() + "\n";
        }


        /// <summary>
        /// IEnumerable helper allows for easy foreach usage, extension method and Linq usage
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RawCapture> GetSequence(ICaptureDevice dev, bool maskExceptions = true)
        {
            try
            {
                PacketCapture e;
                dev.Open();
                while (true)
                {
                    RawCapture packet = null;
                    try
                    {
                        var retval = dev.GetNextPacket(out e);
                        if (retval != GetPacketStatus.PacketRead)
                            break;
                        packet = e.GetPacket();
                    }
                    catch (PcapException pe)
                    {
                        if (!maskExceptions)
                            throw pe;
                    }

                    if (packet == null)
                        break;
                    yield return packet;
                }
            }
            finally
            {
                dev.Close();
            }
        }

        ~PcapDevice()
        {
            Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
