// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
// Copyright 2008-2011 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Per http://msdn.microsoft.com/en-us/ms182161.aspx 
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static partial class LibPcapSafeNativeMethods
    {

        internal static PcapError pcap_setbuff(PcapHandle /* pcap_t */ adapter, int bufferSizeInBytes)
        {
            return
#if NET6_0_OR_GREATER
            OperatingSystem.IsWindows()
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
# endif
                ? _pcap_setbuff(adapter, bufferSizeInBytes)
                : PcapError.PlatformNotSupported;
        }
        internal static PcapError pcap_setmintocopy(PcapHandle /* pcap_t */ adapter, int sizeInBytes)
        {
            return
#if NET6_0_OR_GREATER
            OperatingSystem.IsWindows()
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
#endif
                ? _pcap_setmintocopy(adapter, sizeInBytes)
                : PcapError.PlatformNotSupported;
        }

        /// <summary>
        /// pcap_set_rfmon() sets whether monitor mode should be set on a capture handle when the handle is activated.
        /// If rfmon is non-zero, monitor mode will be set, otherwise it will not be set.  
        /// </summary>
        /// <param name="p">A <see cref="IntPtr"/></param>
        /// <param name="rfmon">A <see cref="int"/></param>
        /// <returns>Returns 0 on success or PCAP_ERROR_ACTIVATED if called on a capture handle that has been activated.</returns>
        internal static PcapError pcap_set_rfmon(PcapHandle /* pcap_t* */ p, int rfmon)
        {
            try
            {
                return _pcap_set_rfmon(p, rfmon);
            }
            catch (EntryPointNotFoundException)
            {
                return PcapError.RfmonNotSupported;
            }
        }

        #region Timestamp related functions

        private static readonly Version Libpcap_1_5 = new Version(1, 5, 0);
        /// <summary>
        /// Available since libpcap 1.5
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        internal static PcapError pcap_set_tstamp_precision(PcapHandle /* pcap_t* p */ adapter, int precision)
        {
            if (Pcap.LibpcapVersion < Libpcap_1_5)
            {
                return PcapError.TimestampPrecisionNotSupported;
            }
            return _pcap_set_tstamp_precision(adapter, precision);
        }

        /// <summary>
        /// Available since libpcap 1.5
        /// </summary>
        /// <param name="adapter"></param>
        internal static int pcap_get_tstamp_precision(PcapHandle /* pcap_t* p */ adapter)
        {
            if (Pcap.LibpcapVersion < Libpcap_1_5)
            {
                return (int)TimestampResolution.Microsecond;
            }
            return _pcap_get_tstamp_precision(adapter);
        }

        #endregion

        /// <summary>
        /// Wraps a Pcap handle around an existing file handle.
        /// </summary>
        /// <param name="handle">Native file handle. On non-Windows systems, this must be a pointer
        /// to a C runtime <c>FILE</c> object.</param>
        /// <param name="precision">Desired timestamp precision (micro/nano).</param>
        /// <param name="errbuf">Buffer that will receive an error description if an error occurs.</param>
        /// <returns></returns>
        internal static PcapHandle pcap_open_handle_offline_with_tstamp_precision(
            SafeHandle handle, uint precision, StringBuilder errbuf)
        {
            var pointer =
#if NET6_0_OR_GREATER
            OperatingSystem.IsWindows()
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
# endif
                ? _pcap_hopen_offline_with_tstamp_precision(handle, precision, errbuf)
                : _pcap_fopen_offline_with_tstamp_precision(handle, precision, errbuf);
            if (pointer == IntPtr.Zero)
            {
                return PcapHandle.Invalid;
            }
            return new PcapFileHandle(pointer, handle);
        }
    }
}
