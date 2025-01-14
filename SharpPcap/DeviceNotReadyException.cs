// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2008-2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// A PcapDevice or dumpfile is not ready for capture operations.
    /// </summary>
    public class DeviceNotReadyException : PcapException
    {
        internal DeviceNotReadyException() : base()
        {
        }

        internal DeviceNotReadyException(string msg) : base(msg)
        {
        }
    }
}
