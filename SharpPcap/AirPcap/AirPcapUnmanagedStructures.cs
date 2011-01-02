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
 * Copyright 2010-2011 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using System.Runtime.InteropServices;

namespace SharpPcap.AirPcap
{
    internal class AirPcapUnmanagedStructures
    {
        /// <summary>
        /// Channel information
        /// Used by \ref AirpcapSetDeviceChannelEx(), \ref AirpcapGetDeviceChannelEx(), \ref AirpcapGetDeviceSupportedChannels()
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct AirpcapChannelInfo
        {
            ///<summary>
            ///Channel frequency, in MHz
            ///UINT
            ///</summary>
            public UInt32 Frequency;

            /// <summary>
            /// 802.11n specific. Offset of the extension channel in case of 40MHz channels. 
            ///
            /// Possible values are -1, 0 +1:
            /// - -1 means that the extension channel should be below the control channel (e.g. Control = 5 and Extension = 1)
            /// - 0 means that no extension channel should be used (20MHz channels or legacy mode)
            /// - +1 means that the extension channel should be above the control channel (e.g. Control = 1 and Extension = 5)
            ///
            /// In case of 802.11a/b/g channels (802.11n legacy mode), this field should be set to 0.
            ///
            /// CHAR
            /// </summary>
            public sbyte ExtChannel;

            /// <summary>
            /// Channel Flags. The only flag supported at this time is \ref AIRPCAP_CIF_TX_ENABLED.
            /// UCHAR
            /// </summary>
            public AirPcapChannelInfoFlags Flags;

#pragma warning disable 0169
            /// <summary>
            /// Reserved. It should be set to {0,0}.
            /// </summary>
            byte Reserved1;
            byte Reserved2;
#pragma warning restore 0169
        };

        /// <summary>
        /// Capture statistics
        /// Returned by AirpcapGetStats()
        /// </summary>
        internal struct AirpcapStats
        {
            ///<summary>
            /// Number of packets that the driver received by the adapter 
            /// from the beginning of the current capture. This value includes the packets 
            /// dropped because of buffer full.
            ///</summary>
            public UInt32 /* UINT */ Recvs;

            ///<summary>
            /// Number of packets that the driver dropped from the beginning of a capture.
            /// A packet is lost when the the buffer of the driver is full. 
            /// </summary>
            public UInt32 /* UINT */ Drops;

            /// <summary>
            /// Packets dropped by the card before going to the USB bus. 
            /// Not supported at the moment.
            /// </summary>
            public UInt32 /* UINT */ IfDrops;

            /// <summary>
            /// Number of packets that pass the BPF filter, find place in the kernel buffer and
            /// therefore reach the application.
            /// </summary>
            public UInt32 /* UINT */ Capt;
        };

        /// <summary>
        /// Device capabilities
        /// Returned by AirpcapGetDeviceCapabilities()
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct AirpcapDeviceCapabilities
        {
            /// <summary>
            /// An id that identifies the adapter model
            /// </summary>
            public AirPcapAdapterId AdapterId;
            /// <summary>
            /// String containing a printable adapter model
            /// </summary>
            public string /* CHAR* */ AdapterModelName;
            /// <summary>
            /// The type of bus the adapter is plugged to
            /// </summary>
            public AirPcapAdapterBus AdapterBus;
            /// <summary>
            /// TRUE if the adapter is able to perform frame injection.
            /// </summary>
            public bool CanTransmit;
            /// <summary>
            /// TRUE if the adapter's transmit power is can be specified by the user application.
            /// </summary>
            public bool CanSetTransmitPower;
            /// <summary>
            /// TRUE if the adapter supports plugging one or more external antennas.
            /// </summary>
            public bool ExternalAntennaPlug;
            /// <summary>
            /// An OR combination of the media that the device supports. Possible values are: \ref AIRPCAP_MEDIUM_802_11_A,
            /// \ref AIRPCAP_MEDIUM_802_11_B, \ref AIRPCAP_MEDIUM_802_11_G or \ref AIRPCAP_MEDIUM_802_11_N.
            /// Not supported at the moment.
            /// </summary>
            public UInt32 /* UINT */ SupportedMedia;
            /// <summary>
            /// An OR combination of the bands that the device supports. Can be one of: \ref AIRPCAP_BAND_2GHZ, 
            /// \ref AIRPCAP_BAND_5GHZ.
            /// </summary>
            public UInt32 /* UINT */ SupportedBands;
        }


        internal const int WepKeyMaxSize = 32;

        /// <summary>
        /// WEB key container
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct AirpcapKey
        {
            /// <summary>
            /// Type of key, can be on of: \ref AIRPCAP_KEYTYPE_WEP, \ref AIRPCAP_KEYTYPE_TKIP, \ref AIRPCAP_KEYTYPE_CCMP. Only AIRPCAP_KEYTYPE_WEP is supported by the driver at the moment.
            /// </summary>
            public AirPcapKeyType /* UINT */ KeyType;

            /// <summary>
            /// Length of the key in bytes
            /// </summary>
            public UInt32 /* UINT */ KeyLen;

            /// <summary>
            /// Key data
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=WepKeyMaxSize)]
            public byte[] /* BYTE */ KeyData;
        }

        /// <summary>
        /// frequency Band.
        /// 802.11 adapters can support different frequency bands, the most important of which are: 2.4GHz (802.11b/g/n) 
        /// and 5GHz (802.11a/n).
        /// </summary>
        internal enum AirpcapChannelBand : int
        {
            ///<summary>Automatically pick the best frequency band</summary>
            AIRPCAP_CB_AUTO = 1,
            /// <summary>2.4 GHz frequency band</summary>
            AIRPCAP_CB_2_4_GHZ = 2,
            /// <summary>4 GHz frequency band</summary>
            AIRPCAP_CB_4_GHZ = 4,
            /// <summary>5 GHz frequency band</summary>
            AIRPCAP_CB_5_GHZ = 5
        };

        /// <summary>
        /// Entry in the list returned by \ref AirpcapGetDeviceList().
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct AirpcapDeviceDescription
        {
            ///<summary>
            ///Next element in the list
            ///struct _AirpcapDeviceDescription*
            ///</summary>
            public IntPtr next;

            ///<summary>
            ///Device name
            ///PCHAR
            ///</summary>
            public string Name;

            ///<summary>
            ///Device description
            ///PCHAR
            ///</summary>
            public string Description;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)] //Note: Pack =1 cause problems with Win 7 64b
        internal struct AirpcapKeysCollection
        {
            /// <summary>
            /// Number of keys in the collection
            /// </summary>
            public UInt32 /* UINT */ nKeys;

#if false
            /// <summary>
            /// Array of nKeys keys.
            /// </summary>
            /* AirpcapKey Keys[0]; */
            public AirpcapKey[] keys;
#endif
        };

        /// <summary>
        /// Packet header
        /// This structure defines the BPF that preceeds every packet delivered to the application
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct AirpcapBpfHeader
        {
            ///<summary>
            ///Timestamp associated with the captured packet. SECONDS.
            ///UINT
            ///</summary>
            public UInt32 TsSec;

            ///<summary>
            ///Timestamp associated with the captured packet. MICROSECONDS.
            ///UINT
            ///</summary>
            public UInt32 TsUsec;

            ///<summary>
            ///Length of captured portion. The captured portion <b>can be different</b> from the original packet, because it is possible (with a proper filter) to instruct the driver to capture only a portion of the packets.
            ///</summary>
            /* UINT */
            public UInt32 Caplen;

            ///<summary>
            ///Original length of packet
            ///UINT
            ///</summary>
            public UInt32 Originallen;

            ///<summary>
            ///Length of bpf header (this struct plus alignment padding). In some cases, a padding could be added between the end of this structure and the packet data for performance reasons. This field can be used to retrieve the actual data of the packet.
            ///USHORT
            ///</summary>
            /* USHORT */
            public UInt16 Hdrlen;
        };

        /// <summary>
        /// Structure used to read the free running counter on a device
        ///
        /// This structure contains the current value of the counter used by the device to timestamp packets (when the hardware supports hardware timestamps). 
        /// This structure also contains the value of the software counter (used to timestamp packets in software), before and after the hardware counter is read
        /// on the device.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct AirpcapDeviceTimestamp
        {
            /// <summary>Current value of the device counter, in microseconds.</summary>
	        public UInt64 DeviceTimestamp;
            /// <summary>Value of the software counter used to timestamp packets before reading the device counter, in microseconds.</summary>
            public UInt64 SoftwareTimestampBefore;
            /// <summary>Value of the software counter used to timestamp packets after reading the device counter, in microseconds.</summary>
            public UInt64 SoftwareTimestampAfter;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ieee80211_radiotap_header
        {
            public Byte it_version; /* Version 0. Only increases
                                     * for drastic changes,
                                     * introduction of compatible
                                     * new fields does not count.
                                     */
            public Byte it_pad;
            public UInt16 it_len;   /* length of the whole
                                     * header in bytes, including
                                     * it_version, it_pad,
                                     * it_len, and data fields.
                                     */
            public UInt32 it_present;  /* A bitmap telling which
                                        * fields are present. Set bit 31
                                        * (0x80000000) to extend the
                                        * bitmap by another 32 bits.
                                        * Additional extensions are made
                                        * by setting bit 31.
                                        */
        };
    }
}
