// Copyright 2021 Chris Morgan <chmorgan@gmail.com>
// Copyright 2021 Ayoub Kaanich <kayoub5@live.com>
// SPDX-License-Identifier: MIT

using SharpPcap.LibPcap;
using System;
using System.Runtime.InteropServices;

namespace SharpPcap
{
    /// <summary>
    /// Provides interfaces to timestamp names, descriptions and features
    /// </summary>
    public class PcapClock
    {
        public string Name { get; }
        public string Description { get; }
        public TimestampSourceResolution Resolution { get; } // High, Low, Unknown

        public Synchronization Synchronized { get; }

        /// <summary>
        /// The type this clock maps back to
        /// </summary>
        public TimestampType TimestampType { get; }

        public PcapClock(TimestampType timestampType)
        {
            Name = TimestampName(timestampType);
            Description = TimestampDescription(timestampType);

            // Per https://www.tcpdump.org/manpages/pcap-tstamp.7.html,
            // "The precision of this time stamp is unspecified; it might or might not be synchronized with the host operating system's clock."
            if (timestampType == TimestampType.Host)
            {
                Resolution = TimestampSourceResolution.Unknown;
                Synchronized = Synchronization.Unknown;
            } else
            {
                Resolution = (timestampType == TimestampType.HostLowPrecision) ? TimestampSourceResolution.Low : TimestampSourceResolution.High;
                Synchronized = ((timestampType == TimestampType.AdapterUnsynced) ||
                                   (timestampType == TimestampType.HostHighPrecisionUnsynced)) ?
                                    Synchronization.None : Synchronization.WithHostOS;
            }
        }

        private static string TimestampName(TimestampType number)
        {
            return LibPcapSafeNativeMethods.pcap_tstamp_type_val_to_name((int)number);
        }

        private static string TimestampDescription(TimestampType number)
        {
            return LibPcapSafeNativeMethods.pcap_tstamp_type_val_to_description((int)number);
        }

        /// <summary>
        /// See https://www.tcpdump.org/manpages/pcap-tstamp.7.html
        /// </summary>
        public enum Synchronization
        {
            Unknown,
            WithHostOS,
            None
        };
    }
}
