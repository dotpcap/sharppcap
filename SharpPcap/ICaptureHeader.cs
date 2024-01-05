// Copyright 2021 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// Information common to all captured packets
    /// </summary>
    public interface ICaptureHeader
    {
        /// <summary>
        /// Timestamp of this header instance
        /// </summary>
        PosixTimeval Timeval { get; }
    }
}
