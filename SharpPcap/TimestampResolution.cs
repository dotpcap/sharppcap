// Copyright 2021 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

using System;
namespace SharpPcap
{
    /// <summary>
    /// Precision / resolution of the timestamp
    ///
    /// See https://github.com/the-tcpdump-group/libpcap/blob/master/pcap/pcap.h#L511
    /// </summary>
    public enum TimestampResolution
    {
        Microsecond = 0,
        Nanosecond = 1
    }
}
