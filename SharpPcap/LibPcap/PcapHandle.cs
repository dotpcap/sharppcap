// Copyright 2010 Chris Morgan <chmorgan@gmail.com>
// Copyright 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    public class PcapHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public PcapHandle()
            : base(true)
        {
        }

        private PcapHandle(IntPtr handle)
            : base(true)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            LibPcapSafeNativeMethods.pcap_close(handle);
            return true;
        }

        internal static readonly PcapHandle Invalid = new PcapHandle(IntPtr.Zero);
    }

    /// <summary>
    /// Wrapper class that maintains reference to both pcap handle and file handle
    /// </summary>
    internal class PcapFileHandle : PcapHandle
    {
        private readonly SafeHandle FileHandle;

        public PcapFileHandle(IntPtr pcapHandle, SafeHandle fileHandle)
        {
            bool gotRef = false;
            // The file handle must not be closed by the runtime until the pcap handle is also closed
            // Incrementing the ref count ensure this
            fileHandle.DangerousAddRef(ref gotRef);
            FileHandle = gotRef ? fileHandle : new SafeFileHandle(IntPtr.Zero, false);
            SetHandle(pcapHandle);
        }

        // If somehow the file handle became invalid, the pcap handle will also be invalid
        public override bool IsInvalid => base.IsInvalid || FileHandle.IsInvalid;

        protected override bool ReleaseHandle()
        {
            // Closing the pcap handle will also close the file handle
            FileHandle.SetHandleAsInvalid();
            return base.ReleaseHandle();
        }
    }

}
