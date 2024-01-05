// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap.LibPcap
{
    /// <summary>
    ///  A wrapper for libpcap's pcap_pkthdr structure
    /// </summary>
    public class PcapHeader : ICaptureHeader
    {
        private static readonly bool isMacOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        private static readonly bool is32BitTs = IntPtr.Size == 4 || RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        internal static readonly int MemorySize = GetTimevalSize() + sizeof(uint) + sizeof(uint);

        private static readonly int CaptureLengthOffset = GetTimevalSize();
        private static readonly int PacketLengthOffset = GetTimevalSize() + sizeof(uint);

        private static int GetTimevalSize()
        {
            if (is32BitTs)
            {
                return Marshal.SizeOf<timeval_windows>();
            }
            if (isMacOSX)
            {
                return Marshal.SizeOf<timeval_macosx>();
            }
            return Marshal.SizeOf<timeval_unix>();
        }

        /// <summary>
        ///  A wrapper class for libpcap's pcap_pkthdr structure
        /// </summary>
        private unsafe PcapHeader(IntPtr pcap_pkthdr, TimestampResolution resolution)
        {
            ulong tv_sec;
            ulong tv_usec;
            if (is32BitTs)
            {
                var ts = *(timeval_windows*)pcap_pkthdr;
                tv_sec = (ulong)ts.tv_sec;
                tv_usec = (ulong)ts.tv_usec;
            }
            else if (isMacOSX)
            {
                var ts = *(timeval_macosx*)pcap_pkthdr;
                tv_sec = (ulong)ts.tv_sec;
                tv_usec = (ulong)ts.tv_usec;
            }
            else
            {
                var ts = *(timeval_unix*)pcap_pkthdr;
                tv_sec = (ulong)ts.tv_sec;
                tv_usec = (ulong)ts.tv_usec;
            }
            Timeval = new PosixTimeval(tv_sec, tv_usec, resolution);
            CaptureLength = (uint)Marshal.ReadInt32(pcap_pkthdr + CaptureLengthOffset);
            PacketLength = (uint)Marshal.ReadInt32(pcap_pkthdr + PacketLengthOffset);
        }

        /// <summary>
        /// Get a PcapHeader structure from a pcap_pkthdr pointer.
        /// </summary>
        public unsafe static PcapHeader FromPointer(IntPtr pcap_pkthdr, TimestampResolution resolution)
        {
            return new PcapHeader(pcap_pkthdr, resolution);
        }

        /// <summary>
        /// Constructs a new PcapHeader
        /// </summary>
        /// <param name="seconds">The seconds value of the packet's timestamp</param>
        /// <param name="microseconds">The microseconds value of the packet's timestamp</param>
        /// <param name="packetLength">The actual length of the packet</param>
        /// <param name="captureLength">The length of the capture</param>
        public PcapHeader(uint seconds, uint microseconds, uint packetLength, uint captureLength)
        {
            this.Timeval = new PosixTimeval(seconds, microseconds);
            this.PacketLength = packetLength;
            this.CaptureLength = captureLength;
        }

        /// <summary>
        /// The length of the packet on the line
        /// </summary>
        public uint PacketLength { get; set; }

        /// <summary>
        /// The the bytes actually captured. If the capture length
        /// is small CaptureLength might be less than PacketLength
        /// </summary>
        public uint CaptureLength { get; set; }

        public PosixTimeval Timeval { get; set; }

        /// <summary>
        /// Marshal this structure into the platform dependent version and return
        /// and IntPtr to that memory
        ///
        /// NOTE: IntPtr MUST BE FREED via Marshal.FreeHGlobal()
        /// </summary>
        /// <returns>
        /// A <see cref="IntPtr"/>
        /// </returns>
        public IntPtr MarshalToIntPtr(TimestampResolution resolution)
        {
            var hdrPtr = Marshal.AllocHGlobal(MemorySize);
            var tv_sec = Timeval.Seconds;
            var unit = resolution == TimestampResolution.Nanosecond ? 1e9M : 1e6M;
            var tv_usec = (ulong)((Timeval.Value % 1) * unit);

            if (is32BitTs)
            {
                // setup the structure to marshal
                var timeval = new timeval_windows
                {
                    tv_sec = (int)tv_sec,
                    tv_usec = (int)tv_usec
                };
                Marshal.StructureToPtr(timeval, hdrPtr, true);
            }
            else if (isMacOSX)
            {
                var timeval = new timeval_macosx
                {
                    tv_sec = (IntPtr)tv_sec,
                    tv_usec = (int)tv_usec
                };
                Marshal.StructureToPtr(timeval, hdrPtr, true);
            }
            else
            {
                var timeval = new timeval_unix
                {
                    tv_sec = (IntPtr)tv_sec,
                    tv_usec = (IntPtr)tv_usec
                };
                Marshal.StructureToPtr(timeval, hdrPtr, true);
            }
            Marshal.WriteInt32(hdrPtr + CaptureLengthOffset, (int)CaptureLength);
            Marshal.WriteInt32(hdrPtr + PacketLengthOffset, (int)PacketLength);
            return hdrPtr;
        }
    }
}
