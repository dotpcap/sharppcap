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
using System.Collections.Generic;
using System.Text;
using SharpPcap.Containers;

namespace SharpPcap
{
    /// <summary>
    /// Summary description for SharpPcap.
    /// </summary>
    public class Pcap
    {
        /// <summary>A delegate for Packet Arrival events</summary>
        public delegate void PacketArrivalEvent(object sender, PcapCaptureEventArgs e);

        /// <summary>
        /// A delegate for delivering network statistics
        /// </summary>
        public delegate void PcapStatisticsExEvent(object sender, PcapStatisticsExEventArgs e);

        /// <summary>
        /// A delegate for notifying of a capture stopped event
        /// </summary>
        public delegate void PcapCaptureStoppedEvent(object sender, bool error);

        /// <summary>Represents the infinite number for packet captures </summary>
        public const int INFINITE = -1;

        /// <summary>A string value that prefixes avery pcap device name </summary>
        internal const string PCAP_NAME_PREFIX = @"\Device\NPF_";


        /* interface is loopback */
        internal const uint     PCAP_IF_LOOPBACK                = 0x00000001;
        private  const int      MAX_ADAPTER_NAME_LENGTH         = 256;      
        private  const int      MAX_ADAPTER_DESCRIPTION_LENGTH  = 128;
        internal const int      MAX_PACKET_SIZE                 = 65536;
        internal const int      PCAP_ERRBUF_SIZE                = 256;
        internal const int      MODE_CAPT                       =   0;
        internal const int      MODE_STAT                       =   1;
        internal const string   PCAP_SRC_IF_STRING              = "rpcap://";

        // Constants for address families
        // These are set in a Pcap static initializer because the values
        // differ between Windows and Linux
        public readonly static int      AF_INET;
        public readonly static int      AF_PACKET;
        public readonly static int      AF_INET6;

        // Constants for pcap loop exit status.
        internal const int LOOP_USER_TERMINATED  = -2;
        internal const int LOOP_EXIT_WITH_ERROR  = -1;
        internal const int LOOP_COUNT_EXHAUSTED  =  0;

        public static string Version
        {
            get
            {
                try
                {
                    return System.Runtime.InteropServices.Marshal.PtrToStringAnsi (SafeNativeMethods.pcap_lib_version ());
                }
                catch
                {
                    return "pcap version can't be identified, you are either using "+
                        "an older version, or pcap is not installed.";
                }
            }
        }

        private static bool isUnix()
        {
            int p = (int) Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                return true;
            } else {
                return false;
            }
        }

        static Pcap()
        {
            // happens to have the same value on Windows and Linux
            AF_INET = 2;

            // AF_PACKET = 17 on Linux, AF_NETBIOS = 17 on Windows
            // FIXME: need to resolve the discrepency at some point
            AF_PACKET = 17;

            if(isUnix())
            {
                AF_INET6 = 10; // value for linux from socket.h
            } else
            {
                AF_INET6 = 23; // value for windows from winsock.h
            }
        }

        /// <summary>
        /// Depreciated: Backwards compatability wrapper around PcapDeviceList. Don't use this.
        /// </summary>
        /// <returns>A List of PcapDevices</returns>
        public static List<PcapDevice> GetAllDevices()
        {
            System.Diagnostics.Debug.WriteLine("List<PcapDevice> GetAllDevices() is depreciated.  Use Pcap.Devices instead.");
            return new List<PcapDevice>(PcapDeviceList.Instance);
        }

        public static PcapOfflineDevice GetPcapOfflineDevice(string pcapFileName)
        {
            return new PcapOfflineDevice( pcapFileName );
        }

        /// <summary>
        /// Depreciated: Backwards compatability wrapper around Pcap.Devices[string Name].  Don't use this.
        /// </summary>
        /// <param name="pcapDeviceName">The name of a device.</param>
        /// <returns>A PcapDevice</returns>
        public static PcapDevice GetPcapDevice( string pcapDeviceName )
        {
            System.Diagnostics.Debug.WriteLine("GetPcapDevoce(string pcapDeviceName) is depreciated.  Use Pcap.Devices[pcapDeviceName] instead.");
            return PcapDeviceList.Instance[pcapDeviceName];
        }
    }
}
