// Copyright 2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// Thrown when an operation can't be performed because
    /// a background capture has been started via PcapDevice.StartCapture()
    /// </summary>
    public class InvalidOperationDuringBackgroundCaptureException : PcapException
    {
        /// <summary>
        /// string constructor
        /// </summary>
        /// <param name="msg">
        /// A <see cref="string"/>
        /// </param>
        public InvalidOperationDuringBackgroundCaptureException(string msg) : base(msg)
        {
        }
    }
}
