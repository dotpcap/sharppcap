/*
Copyright (c) 2005 Tamir Gal, http://www.tamirgal.com, All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice,
        this list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright 
        notice, this list of conditions and the following disclaimer in 
        the documentation and/or other materials provided with the distribution.

    3. The names of the authors may not be used to endorse or promote products
        derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
        public delegate void PcapStatisticsEvent(object sender, PcapStatisticsEventArgs e);

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
            return new List<PcapDevice>(new PcapDeviceList());
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
            return new PcapDeviceList()[pcapDeviceName];
        }
    }
}
