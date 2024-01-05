// Copyright 2009-2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// Status types when capture is stopped
    /// </summary>
    public enum CaptureStoppedEventStatus
    {
        /// <summary>
        /// Capture completed without errors
        /// </summary>
        CompletedWithoutError,

        /// <summary>
        /// Error while capturing
        /// </summary>
        ErrorWhileCapturing
    }
}
