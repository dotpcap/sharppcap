// Copyright 2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;

namespace SharpPcap
{
    /// <summary>
    /// The mode used when opening a device
    /// See pcap_open() documentation
    /// https://github.com/the-tcpdump-group/libpcap/blob/aa818d3fe5add3f69b87671fc17f8eeb09f10139/pcap/pcap.h#L938
    /// </summary>
    [Flags]
    public enum DeviceModes : short
    {
        /// <summary>
        /// No flags set
        /// </summary>
        None = 0,

        /// <summary>
        /// Defines if the adapter has to go in promiscuous mode. 
        /// </summary>
        Promiscuous = 1,

        /// <summary>
        /// Defines if the data trasfer (in case of a remote capture)
        /// has to be done with UDP protocol. 
        /// </summary>
        DataTransferUdp = 2,

        /// <summary>
        /// Defines if the remote probe will capture its own generated traffic. 
        /// </summary>
        NoCaptureRemote = 4,

        /// <summary>
        /// Defines if the local adapter will capture its own generated traffic. 
        /// </summary>
        NoCaptureLocal = 8,

        /// <summary>
        /// This flag configures the adapter for maximum responsiveness. 
        /// </summary>
        MaxResponsiveness = 16
    }
}
