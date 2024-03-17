// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2009-2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// A delegate for notifying of a capture stopped event
    /// </summary>
    public delegate void CaptureStoppedEventHandler(object sender, CaptureStoppedEventStatus status);
}
