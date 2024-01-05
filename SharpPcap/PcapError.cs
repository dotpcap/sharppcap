// Copyright 2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// Error codes for the pcap API.
    /// See <code>#define PCAP_ERROR*</code> in https://github.com/the-tcpdump-group/libpcap/blob/master/pcap/pcap.h
    /// </summary>
    public enum PcapError : int
    {
        /// <summary>
        /// generic error code
        /// </summary>
        Generic = -1,
        /// <summary>
        /// loop terminated by pcap_breakloop
        /// </summary>
        Break = -2,
        /// <summary>
        /// the capture needs to be activated
        /// </summary>
        NotActivated = -3,
        /// <summary>
        /// the operation can't be performed on already activated captures
        /// </summary>
        Activated = -4,
        /// <summary>
        ///  no such device exists
        /// </summary>
        NoSuchDevice = -5,
        /// <summary>
        /// this device doesn't support rfmon (monitor) mode
        /// </summary>
        RfmonNotSupported = -6,
        /// <summary>
        /// operation supported only in monitor mode
        /// </summary>
        NotRfmon = -7,
        /// <summary>
        /// no permission to open the device
        /// </summary>
        PermissionDenied = -8,
        /// <summary>
        /// interface isn't up
        /// </summary>
        InterfaceNotUp = -9,
        /// <summary>
        /// this device doesn't support setting the time stamp type
        /// </summary>
        TimestampTypeNotSupported = -10,
        /// <summary>
        /// you don't have permission to capture in promiscuous mode
        /// </summary>
        PromiscuousPermissionDenied = -11,
        /// <summary>
        /// the requested time stamp precision is not supported
        /// </summary>
        TimestampPrecisionNotSupported = -12,

        /// <summary>
        /// Defined by SharpPcap
        /// Corresponds to HRESULT of <see cref="System.PlatformNotSupportedException"/>
        /// This means either the OS or the Libpcap version does not support requested configuration
        /// </summary>
        PlatformNotSupported = unchecked((int)0x80131539),
    }
}
