/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/* 
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 * Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
 */

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SharpPcap
{
    /// <summary>
    /// Constants and static helper methods
    /// </summary>
    public class Pcap
    {
        /// <summary>Represents the infinite number for packet captures </summary>
        internal const int InfinitePacketCount = -1;

        /* interface is loopback */
        internal const uint PCAP_IF_LOOPBACK = 0x00000001;
        internal const int MAX_PACKET_SIZE = 65536;
        internal const int PCAP_ERRBUF_SIZE = 256;

        // Constants for address families
        // These are set in a Pcap static initializer because the values
        // differ between Windows and Linux
        internal readonly static int AF_INET;
        internal readonly static int AF_PACKET;
        internal readonly static int AF_INET6;

        // Constants for pcap loop exit status.
        internal const int LOOP_USER_TERMINATED = -2;
        internal const int LOOP_EXIT_WITH_ERROR = -1;
        internal const int LOOP_COUNT_EXHAUSTED = 0;

        /// <summary>
        /// Returns the pcap version string retrieved via a call to pcap_lib_version()
        /// </summary>
        public static string Version
        {
            get
            {
                try
                {
                    return LibPcap.LibPcapSafeNativeMethods.pcap_lib_version();
                }
                catch
                {
                    return "Pcap version can't be identified. It is likely that pcap is not installed " +
                        "but you could be using a very old version.";
                }
            }
        }

        private static Version _libpcapVersion;
        public static Version LibpcapVersion
        {
            get
            {
                _libpcapVersion = _libpcapVersion ?? GetLibpcapVersion(Version);
                return _libpcapVersion;
            }
        }

        public static Version SharpPcapVersion
        {
            get
            {
                return typeof(Pcap).Assembly.GetName().Version;
            }
        }

        internal static Version GetLibpcapVersion(string version)
        {
            var regex = new Regex(@"libpcap version (\d+\.\d+(\.\d+)?)");
            var match = regex.Match(version);
            if (match.Success)
            {
                return new Version(match.Groups[1].Value);
            }
            return new Version();
        }

        static Pcap()
        {
            // happens to have the same value on Windows and Linux
            AF_INET = 2;

            // AF_PACKET = 17 on Linux, AF_NETBIOS = 17 on Windows
            // FIXME: need to resolve the discrepency at some point
            AF_PACKET = 17;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                AF_INET6 = 10; // value for linux from socket.h
            }
            else
            {
                AF_INET6 = 23; // value for windows from winsock.h
            }
        }
    }
}
