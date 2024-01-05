// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
// Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;

namespace SharpPcap
{
    /// <summary>
    /// General Pcap Exception.
    /// </summary>
    public class PcapException : Exception
    {
        public PcapError Error { get; }

        internal PcapException() : base()
        {
            Error = PcapError.Generic;
        }

        internal PcapException(string msg)
            : base(msg)
        {
            Error = PcapError.Generic;
        }

        internal PcapException(string msg, PcapError error)
            : base(msg + $" (Error Code: {error})")
        {
            Error = error;
        }

    }
}
