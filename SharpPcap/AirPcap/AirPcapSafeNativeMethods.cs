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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// Per http://msdn.microsoft.com/en-us/ms182161.aspx 
    /// </summary>
    [SuppressUnmanagedCodeSecurityAttribute]
    internal static class AirPcapSafeNativeMethods
    {
        private const string AIRPCAP_DLL = "airpcap";

        #region AirPcap specific

        /// <summary>
        /// Sets variables to the particular version being used
        /// </summary>
        /// <param name="VersionMajor">Pointer to a variable that will be filled with the major version number</param>
        /// <param name="VersionMinor">Pointer to a variable that will be filled with the minor version number</param>
        /// <param name="VersionRev">Pointer to a variable that will be filled with the revision number</param>
        /// <param name="VersionBuild">Pointer to a variable that will be filled with the build number</param>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void AirpcapGetVersion(/* PUINT */ out UInt32 VersionMajor,
                                                      /* PUINT */ out UInt32 VersionMinor,
                                                      /* PUINT */ out UInt32 VersionRev,
                                                      /* PUINT */ out UInt32 VersionBuild);

        /// <summary>
        /// Returns the last error related to the specified handle
        /// </summary>
        /// <param name="AdapterHandle">Handle to an open adapter</param>
        /// <returns>String with the last error, a PCHAR</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr AirpcapGetLastError(/* PAirpcapHandle */ IntPtr AdapterHandle);

        /// <summary>
        /// Returns the list of available devices 
        /// </summary>
        /// <param name="PPAllDevs">Address to a caller allocated pointer. On success this pointer will receive the head of a list of available devices.</param>
        /// <param name="Ebuf">String that will contain error information if FALSE is returned. The size of the string must be AIRPCAP_ERRBUF_SIZE bytes.</param>
        /// <returns>TRUE on success. FALSE is returned on failure, in which case Ebuf is filled in with an appropriate error message.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int AirpcapGetDeviceList(ref IntPtr /* PAircapDeviceDescription* */ PPAllDevs,
                                                        StringBuilder /* PCHAR */ Ebuf);

        /// <summary>
        /// Frees a list of devices returned by AirpcapGetDeviceList()
        /// </summary>
        /// <param name="PAllDevs">Head of the list of devices returned by AirpcapGetDeviceList()</param>
        /// <returns></returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void AirpcapFreeDeviceList(IntPtr /* PAirpcapDeviceDescription */ PAllDevs);

        /// <summary>
        /// Opens an adapter
        /// </summary>
        /// <param name="DeviceName">Name of the device to open. Use AirpcapGetDeviceList() to get the list of devices.</param>
        /// <param name="Ebuf">String that will contain error information in case of failure. The size of the string must be AIRPCAP_ERRBUF_SIZE bytes.</param>
        /// <returns>A PAirpcapHandle handle on success. NULL is returned on failure, in which case Ebuf is filled in with an appropriate error message.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr AirpcapOpen(string /* PCHAR */ DeviceName, StringBuilder /* PCHAR */ Ebuf);

        /// <summary>
        /// Closes an adapter
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter to close.</param>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void AirpcapClose(/* PAirpcapHandle */ IntPtr AdapterHandle);

        /// <summary>
        /// Get the capabilities of a device
        /// NOTE: The PCapabilities structure returned by AirpcapGetDeviceCapabilities() must be considered invalid 
        /// after the adapter has been closed. 
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PCapabilities">Pointer to a library-allocated AirpcapDeviceCapabilities structure that contains
        /// the capabilities of the adapter</param>
        /// <returns>True on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDeviceCapabilities(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                                 /* PAirpcapDeviceCapabilities* */ out IntPtr PCapabilities);

        /// <summary>
        /// Sets the device's monitor mode and acknowledgment settings.
        ///
        /// When an adapter is plugged into the system, it's always configured with monitor mode ON and acknowledgment settings OFF.
        /// These values are not stored persistently, so if you want to turn monitor mode off, you will need to do it 
        /// every time you attach the adapter.
        ///
        /// \note currently, the AirPcap adapter supports frames acknowleging when the adapter is NOT in monitor mode. This means that
        /// the combinations in which the two flags have the same value will cause AirpcapSetDeviceMacFlags() to fail.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="AirpcapMacFlags">Flags word, that contains a bitwise-OR combination of the following flags: \ref AIRPCAP_MF_MONITOR_MODE_ON and \ref AIRPCAP_MF_ACK_FRAMES_ON .</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetDeviceMacFlags(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                             /* UINT */ AirPcapMacFlags AirpcapMacFlags);

        /// <summary>
        /// Gets the device's monitor mode and acknowledgement settings
        ///
        /// When an adapter is plugged into the system, it's always configured with monitor mode ON and acknowledgment settings OFF.
        /// These values are not stored persistently, so if you want to turn monitor mode off, you will need to do it 
        /// every time you attach the adapter.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PAirpcapMacFlags">User-provided flags word, that will be filled by the function with an OR combination of the 
        /// following flags: \ref AIRPCAP_MF_MONITOR_MODE_ON and \ref AIRPCAP_MF_ACK_FRAMES_ON.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDeviceMacFlags(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                             /* PUINT */ out AirPcapMacFlags PAirpcapMacFlags);

        /// <summary>
        /// Sets the link type of an adapter
        ///
        /// the "link type" determines how the driver will encode the packets captured from the network.
        ///  Aircap supports two link types:
        ///  - \ref AIRPCAP_LT_802_11, to capture 802.11 frames (including control frames) without any
        ///   power information. Look at the "Capture_no_radio" example application in the developer's pack 
        ///   for a reference on how to decode 802.11 frames with this link type.
        ///  - \ref AIRPCAP_LT_802_11_PLUS_RADIO, to capture 802.11 frames (including control frames) with a radiotap header
        ///  that contains power and channel information. More information about the radiotap header can be found in the
        ///  \ref radiotap section. Moreover, the "Capture_radio" example application in 
        ///  the developer's pack can be used as a reference on how to decode 802.11 frames with radiotap headers.
        ///  - \ref AIRPCAP_LT_802_11_PLUS_PPI, to capture 802.11 frames (including control frames) with a Per Packet Information (PPI)
        ///    header that contains per-packet meta information like channel and power information. More details on the PPI header can
        ///    be found in the PPI online documentation (TODO).
        /// </summary>
        /// <param name="AdapterHandle"></param>
        /// <param name="NewLinkType">the "link type", i.e. the format of the frames that will be received from the adapter.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetLinkType(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                       /* AirpcapLinkType */ AirPcapLinkTypes NewLinkType);

        /// <summary>
        /// Gets the link type of the specified adapter
        ///           the "link type" determines how the driver will encode the packets captured from the network.
        ///
        ///  Aircap supports two link types:
        ///  - \ref AIRPCAP_LT_802_11, to capture 802.11 frames (including control frames) without any
        ///   power information. Look at the "Capture_no_radio" example application in the developer's pack 
        ///   for a reference on how to decode 802.11 frames with this link type.
        ///  - \ref AIRPCAP_LT_802_11_PLUS_RADIO, to capture 802.11 frames (including control frames) with a radiotap header
        ///  that contains power and channel information. More information about the radiotap header can be found int the
        ///  \ref radiotap section. Moreover, the "Capture_radio" example application in 
        ///  the developer's pack can be used as a reference on how to decode 802.11 frames with radiotap headers.
        ///  - \ref AIRPCAP_LT_802_11_PLUS_PPI, to capture 802.11 frames (including control frames) with a Per Packet Information (PPI)
        ///    header that contains per-packet meta information like channel and power information. More details on the PPI header can
        ///    be found in the PPI online documentation (TODO).
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PLinkType">Pointer to a caller allocated AirpcapLinkType variable that will contain
        /// the link type of the adapter</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetLinkType(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                       /* PAirpcapLinkType */ out AirPcapLinkTypes PLinkType);

        /// <summary>
        /// Configures the adapter on whether to include the MAC Frame Check Sequence in the captured packets.
        ///
        /// In the default configuration, the adapter includes the FCS in the captured packets. The MAC Frame Check Sequence 
        ///  is 4 bytes and is located at the end of the 802.11 packet, with \ref AIRPCAP_LT_802_11, \ref AIRPCAP_LT_802_11_PLUS_RADIO and
        ///  \ref AIRPCAP_LT_802_11_PLUS_PPI link types.
        ///  When the FCS inclusion is turned on, and if the link type is \ref AIRPCAP_LT_802_11_PLUS_RADIO, the radiotap header 
        ///  that precedes each frame has two additional fields at the end: Padding and FCS. These two fields are not present 
        ///  when FCS inclusion is off.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="IsFcsPresent">TRUE if the packets should include the FCS, FALSE otherwise</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetFcsPresence(/* PAirpcapHandle */ IntPtr AdapterHandle, bool IsFcsPresent);

        /// <summary>
        /// PIsFcsPresent is tue if the specified adapter includes the MAC Frame Check Sequence in the captured packets
        ///
        /// In the default configuration, the adapter includes the FCS in the captured packets. The MAC Frame Check Sequence 
        /// is 4 bytes and is located at the end of the 802.11 packet, with \ref AIRPCAP_LT_802_11, \ref AIRPCAP_LT_802_11_PLUS_RADIO and
        /// \ref AIRPCAP_LT_802_11_PLUS_PPI link types.
        /// When the FCS inclusion is turned on, and if the link type is \ref AIRPCAP_LT_802_11_PLUS_RADIO, the radiotap header 
        /// that precedes each frame has two additional fields at the end: Padding and FCS. These two fields are not present 
        /// when FCS inclusion is off.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PIsFcsPresent">User-provided variable that will be set to true if the adapter is including the FCS</param>
        /// <returns>TRUE if the operation is successful. FALSE otherwise.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetFcsPresence(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                          /* PBOOL */ out bool PIsFcsPresent);

        /// <summary>
        /// Configures the adapter to accept or drop frames with an incorrect Frame Check sequence (FCS)
        ///
        /// NOTE: By default the driver is configured in \ref AIRPCAP_VT_ACCEPT_EVERYTHING mode
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="ValidationType">The type of validation the driver will perform. See the documentation of \ref AirpcapValidationType for details.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetFcsValidation(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                            /* AirpcapValidationType */ AirPcapValidationType ValidationType);

        /// <summary>
        /// Checks if the specified adapter is configured to capture frames with incorrect an incorrect Frame Check Sequence (FCS). 
        ///
        /// \note By default, the driver is configured in \ref AIRPCAP_VT_ACCEPT_EVERYTHING mode.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="ValidationType">Pointer to a user supplied variable that will contain the type of validation the driver will perform. See the documentation of \ref AirpcapValidationType for details.</param>
        /// <returns>TRUE if the operation is successful, FALSE otherwise</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetFcsValidation(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                            /* PAirpcapValidationType */ out AirPcapValidationType ValidationType);

        /// <summary>
        /// Sets the list of decryption keys that AirPcap is going to use with the specified device.
        ///
        /// AirPcap is able to use a set of decryption keys to decrypt the traffic transmitted on a specific SSID. If one of the
        /// keys corresponds to the one the frame has been encrypted with, the driver will perform decryption and return the cleartext frames
        /// to the application.
        ///
        /// This function allows to set the <b>device-specific</b> set of keys. These keys will be used by the specified device only,
        /// and will not be used by other airpcap devices besides the specified one. 
        ///
        /// At this time, the only supported decryption method is WEP.
        ///
        /// The keys are applied to the packets in the same order they appear in the KeysCollection structure until the packet is 
        /// correctly decrypted, therefore putting frequently used keys at the beginning of the structure improves performance.
        ///
        /// \note When you change the set of keys from an open capture instance, the change will be
        ///         immediately reflected on all the other capture instances on the same device.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="KeysCollection">Pointer to a \ref PAirpcapKeysCollection structure that contains the keys to be set in the device.</param>
        /// <returns>TRUE if the operation is successful. FALSE otherwise.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetDeviceKeys(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                         /* PAirpcapKeysCollection */ IntPtr KeysCollection);

        /// <summary>
        /// Returns the list of decryption keys that are currently associated with the specified device
        ///
        /// This function returns the <b>device-specific</b> set of keys. These keys are used by the specified device only,
        /// and not by other airpcap devices besides the specified one. 
        ///
        /// AirPcap is able to use a set of decryption keys to decrypt the traffic transmitted on a specific SSID. If one of the
        /// keys corresponds to the one the frame has been encrypted with, the driver will perform decryption and return the cleartext frames
        /// to the application. 
        /// AirPcap supports, for every device, multiple keys at the same time.
        ///
        /// The configured decryption keys are device-specific, therefore AirpcapGetDeviceKeys() will return a different set of keys
        /// when called on different devices.
        ///
        ///At this time, the only supported decryption method is WEP.
        /// </summary>
        /// <param name="AdapterHandle">Handle to an open adapter</param>
        /// <param name="KeysCollection">User-allocated PAirpcapKeysCollection structure that will be filled with the keys.</param>
        /// <param name="PKeysCollectionSize">- \b IN: pointer to a user-allocated variable that contains the length of the KeysCollection structure, in bytes.
        ///                                   - \b OUT: amount of data moved by AirPcap in the buffer pointed by KeysBuffer, in bytes.</param>
        /// <returns>TRUE if the operation is successful. If an error occurs, the return value is FALSE and KeysCollectionSize is zero. 
        /// If the provided buffer is too small to contain the keys, the return value is FALSE and KeysCollectionSize contains the
        /// needed KeysCollection length, in bytes. If the device doesn't have any decryption key configured, the return value is TRUE, and 
        /// KeysCollectionSize will be zero.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDeviceKeys(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                         /* PAirpcapKeysCollection */ IntPtr KeysCollection,
                                                         /* PUINT */ ref UInt32 PKeysCollectionSize);

        /// <summary>
        /// Set the global list of decryption keys that AirPcap is going to use with all the devices.
        ///
        /// The AirPcap driver is able to use a set of decryption keys to decrypt the traffic transmitted on a specific SSID. If one of the
        /// keys corresponds to the one the frame has been encrypted with, the driver will perform decryption and return the cleartext frames
        /// to the application.
        ///
        /// This function allows to set the <b>global</b> set of keys. These keys will be used by all the devices plugged in
        /// the machine. 
        ///
        /// At this time, the only supported decryption method is WEP.
        ///
        /// The keys are applied to the packets in the same order they appear in the KeysCollection structure until the packet is 
        /// correctly decrypted, therefore putting frequently used keys at the beginning of the structure improves performance.
        ///
        /// \note When you change the set of keys from an open capture instance, the change will be
        /// immediately reflected on all the other capture instances.
        /// </summary>
        /// <param name="AdapterHandle">Handle to an open adapter</param>
        /// <param name="KeysCollection">Pointer to a \ref PAirpcapKeysCollection structure that contains the keys to be set globally.</param>
        /// <returns>TRUE if the operation is successful. FALSE otherwise.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetDriverKeys(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                         /* PAirpcapKeysCollection */ IntPtr KeysCollection);

        /// <summary>
        /// Returns the global list of decryption keys that AirPcap is using with all the devices.
        ///
        /// This function returns the <b>global</b> set of keys. These keys will be used by all the devices plugged in
        /// the machine. 
        ///
        /// The AirPcap driver is able to use a set of decryption keys to decrypt the traffic transmitted on a specific SSID. If one of the
        /// keys corresponds to the one the frame has been encrypted with, the driver will perform decryption and return the cleartext frames
        /// to the application.
        ///
        /// At this time, the only supported decryption method is WEP.
        /// </summary>
        /// <param name="AdapterHandle">Handle to an adapter</param>
        /// <param name="KeysCollection">User-allocated PAirpcapKeysCollection structure that will be filled with the keys.</param>
        /// <param name="PKeysCollectionSize">- \b IN: pointer to a user-allocated variable that contains the length of the KeysCollection structure, in bytes.
        ///                                   - \b OUT: amount of data moved by AirPcap in the buffer pointed by KeysBuffer, in bytes.</param>
        /// <returns>TRUE if the operation is successful. If an error occurs, the return value is FALSE and KeysCollectionSize is zero. 
        /// If the provided buffer is too small to contain the keys, the return value is FALSE and KeysCollectionSize contains the
        /// needed KeysCollection length, in bytes. If no global decryption keys are configured, the return value is TRUE, and 
        /// KeysCollectionSize will be zero.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDriverKeys(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                         /* PAirpcapKeysCollection */ IntPtr KeysCollection,
                                                         /* PUINT */ ref UInt32 PKeysCollectionSize);

        /// <summary>
        /// Turns on or off the decryption of the incoming frames with the <b>device-specific</b> keys.
        ///
        /// The device-specific decryption keys can be configured with the \ref AirpcapSetDeviceKeys() function.
        /// \note By default, the driver is configured with \ref AIRPCAP_DECRYPTION_ON.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="Enable">Either AIRPCAP_DECRYPTION_ON or AIRPCAP_DECRYPTION_OFF</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetDecryptionState(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                              /* AirpcapDecryptionState */ AirPcapDecryptionState Enable);

        /// <summary>
        /// Tells if this open instance is configured to perform the decryption of the incoming frames with the <b>device-specific</b> keys.
        ///
        /// The device-specific decryption keys can be configured with the \ref AirpcapSetDeviceKeys() function.
        /// \note By default, the driver is configured with \ref AIRPCAP_DECRYPTION_ON.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PEnable">Pointer to a user supplied variable that will contain the decryption configuration. See \ref PAirpcapDecryptionState for details.</param>
        /// <returns>TRUE if the operation is successful, FALSE otherwise</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDecryptionState(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                              /* PAirpcapDecryptionState */ out AirPcapDecryptionState PEnable);

        /// <summary>
        /// Turns on or off the decryption of the incoming frames with the <b>global</b> set of keys.
        ///
        /// The global decryption keys can be configured with the \ref AirpcapSetDriverKeys() function.
        /// \note By default, the driver is configured with \ref AIRPCAP_DECRYPTION_ON.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="Enable">Either \ref AIRPCAP_DECRYPTION_ON or \ref AIRPCAP_DECRYPTION_OFF</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetDriverDecryptionState(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                                    /* AirpcapDecryptionState */ AirPcapDecryptionState Enable);

        /// <summary>
        /// Tells if this open instance is configured to perform the decryption of the incoming frames with the <b>global</b> set of keys.
        ///
        /// The global decryption keys can be configured with the \ref AirpcapSetDriverKeys() function.
        /// \note By default, the driver is configured with \ref AIRPCAP_DECRYPTION_ON.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PEnable">Pointer to a user supplied variable that will contain the decryption configuration. See \ref PAirpcapDecryptionState for details.</param>
        /// <returns>TRUE if the operation is successful. FALSE otherwise.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDriverDecryptionState(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                                    /* PAirpcapDecryptionState */ out AirPcapDecryptionState PEnable);

        /// <summary>
        /// Sets the radio channel of a device
        ///
        ///  The list of available channels can be retrieved with \ref AirpcapGetDeviceSupportedChannels(). The default channel setting is 6.
        ///
        /// \note This is a device-related function: when you change the channel from an open capture instance, the change will be
        /// immediately reflected on all the other capture instances.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="Channel">The new channel to set</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetDeviceChannel(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                            /* uint */ UInt32 Channel);

        /// <summary>
        /// Gets the radio channel of a device
        ///
        /// The list of available channels can be retrieved with \ref AirpcapGetDeviceSupportedChannels(). The default channel setting is 6.
        ///
        /// \note This is a device-related function: when you change the channel from an open capture instance, the change will be
        /// immediately reflected on all the other capture instances.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PChannel">Pointer to a user-supplied variable into which the function will copy the currently configured radio channel.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDeviceChannel(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                            /* PUINT */ out UInt32 PChannel);

        /// <summary>
        /// Sets the channel of a device through its radio frequency. In case of 802.11n enabled devices, it sets the extension channel, if used.
        ///
        /// \note This is a device-related function: when you change the channel from an open capture instance, the change will be
        /// immediately reflected on all the other capture instances.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="ChannelInfo">The new channel information to set</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetDeviceChannelEx(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                              /* AirpcapChannelInfo */ AirPcapUnmanagedStructures.AirpcapChannelInfo ChannelInfo);

        /// <summary>
        /// Gets the channel of a device through its radio frequency. In case of 802.11n enabled devices, it gets the extension channel, if in use.
        ///
        /// \note This is a device-related function: when you change the channel from an open capture instance, the change will be
        /// immediately reflected on all the other capture instances.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PChannelInfo">Pointer to a user-supplied variable into which the function will copy the currently configured channel information.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDeviceChannelEx(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                              /* PAirpcapChannelInfo */ out AirPcapUnmanagedStructures.AirpcapChannelInfo PChannelInfo);

        /// <summary>
        /// Gets the list of supported channels for a given device. In case of a 802.11n capable device, information related to supported extension channels is also reported. 
        ///
        /// Every control channel is listed multiple times, one for each different supported extension channel. For example channel 6 (2437MHz)  is usually listed three times:
        ///  - <b>Frequency 2437 Extension +1</b>. Control channel is 6, extension channel is 10.
        ///  - <b>Frequency 2437 Extension 0</b>. Control channel is 6, no extension channel is used (20MHz channel and legacy mode).
        ///  - <b>Frequency 2437 Extension -1</b>. Control channel is 6, extension channel is 2.
        ///
        /// \note The supported channels are not listed in any specific order.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="ppChannelInfo">Pointer to a user-supplied variable that will point to an array of supported channel. Such list must not be freed by the caller</param>
        /// <param name="pNumChannelInfo">Number of channels returned in the array</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDeviceSupportedChannels(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                                      /* PAirpcapChannelInfo* */ out IntPtr ppChannelInfo,
                                                                      /* PUINT */ out UInt32 pNumChannelInfo);

        /// <summary>
        /// Converts a frequency to the corresponding channel
        /// </summary>
        /// <param name="Frequency">Frequency of the channel in MHz</param>
        /// <param name="PChannel">Pointer to a user-supplied variable that will contain the channel number on success</param>
        /// <param name="PBand">Pointer to a user-supplied variable that will contain the band (a orb/g) of the given channel</param>
        /// <returns>TRUE on success, i.e. the frequency corresponds to a valid a or b/g channel</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapConvertFrequencyToChannel(/* UINT */ UInt32 Frequency,
                                                                     /* PUINT */ out UInt32 PChannel,
                                                                     /* PAirpcapChannelBand */ out UInt32 PBand);

        /// <summary>
        /// Converts a given channel to the corresponding frequency
        ///  Because of the overlap of channels with respect to 1-14BG and 1-14A, this function will give precidence to BG.
        ///  Thus, the channels are returned as follows:
        ///    - <b>Channel 0:</b> 5000MHz
        ///    - <b>Channels 1-14:</b> 2412MHz - 2484MHz
        ///    - <b>Channels 15-239:</b> 5005MHz - 6195MHz
        ///    - <b>Channels 240-255:</b> 4920MHz - 4995MHz
        /// </summary>
        /// <param name="Channel">Channel number to be converted</param>
        /// <param name="PFrequency">Pointer to a user-supplied variable that will contain the channel frequency in MHz on success></param>
        /// <returns></returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapConvertChannelToFrequency(/* UINT */ UInt32 Channel,
                                                                     /* PUINT */ ref UInt32 PFrequency);

        /// <summary>
        /// Sets the size of the kernel packet buffer for this adapter
        ///
        /// Every AirPcap open instance has an associated kernel buffer, whose default size is 1 Mbyte.
        /// This function can be used to change the size of this buffer, and can be called at any time.
        /// A bigger kernel buffer size decreases the risk of dropping packets during network bursts or when the
        /// application is busy, at the cost of higher kernel memory usage.
        ///
        /// \note Don't use this function unless you know what you are doing. Due to caching issues and bigger non-paged
        /// memory consumption, bigger buffer sizes can decrease the capture performace instead of improving it.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="BufferSize">New size in bytes</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetKernelBuffer(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                           /* UINT */ UInt32 BufferSize);

        /// <summary>
        /// Gets the size of the kernel packet buffer for this adapter
        ///
        /// Every AirPcap open instance has an associated kernel buffer, whose default size is 1 Mbyte.
        /// This function can be used to get the size of this buffer.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PSizeBytes">User-allocated variable that will be filled with the size of the kernel buffer.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetKernelBufferSize(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                               /* PUINT */ out UInt32 PSizeBytes);

        /// <summary>
        /// Sets the power of the frames transmitted by adapter
        ///
        /// The transmit power value is monotonically increasing with higher power levels. 1 is the minimum allowed transmit power.
        ///
        /// \note The maximum transmit power on each channel is limited by FCC regulations. Therefore, the maximum transmit power
        /// changes from channel to channel. When the channel is changed with \ref AirpcapSetDeviceChannel() or 
        /// \ref AirpcapSetDeviceChannelEx() the power is set to the maximum allowd value for that channel. You can read this
        /// value with \ref AirpcapGetTxPower(). Not all the AirPcap adapters support setting the transmit power; you can use
        ///  \ref AirpcapGetDeviceCapabilities() to find if the current adapter supports this feature.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="Power">The transmit power. Setting a zero power makes the adapter select the
        /// highest possible power for the current channel.</param>
        /// <returns>TRUE on success. False on failure or if the adapter doesn't support setting the transmit power.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetTxPower(/* PAirpcapHandle */ IntPtr AdapterHandle, /* UINT */ UInt32 Power);

        /// <summary>
        /// Returns the current transmit power level of the adapter
        ///
        /// The transmit power value is monotonically increasing with higher power levels. 0 is the minimum allowed power.
        /// \note The maximum transmit power on each channel is limited by FCC regulations. Therefore, the maximum transmit power
        /// changes from channel to channel. When the channel is changed with \ref AirpcapSetDeviceChannel() or 
        /// \ref AirpcapSetDeviceChannelEx() the power is set to the maximum allowd value for that channel. Not all the AirPcap 
        /// adapters support setting the transmit power; you can use \ref AirpcapGetDeviceCapabilities() to find if the current 
        ///adapter supports this feature.
        ///
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PPower">User-allocated variable that will be filled with the size of the transmit power</param>
        /// <returns>TRUE on success, false on failure or if the adapter doesn't support getting the transmit power</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetTxPower(/* PAirpcapHandle */ IntPtr AdapterHandle, /* PUINT */ out UInt32 PPower);

        /// <summary>
        /// Saves the configuration of the specified adapter in the registry, so that it becomes the default for this adapter.
        ///
        /// Almost all the AirPcap calls that modify the configuration (\ref AirpcapSetLinkType(), \ref AirpcapSetFcsPresence(), 
        /// \ref AirpcapSetFcsValidation(), \ref AirpcapSetKernelBuffer(), \ref AirpcapSetMinToCopy())
        /// affect only the referenced AirPcap open instance. This means that if you do another \ref AirpcapOpen() on the same
        /// adapter, the configuration changes will not be remembered, and the new adapter handle will have default configuration
        /// settings.
        ///
        /// Exceptions to this rule are the \ref AirpcapSetDeviceChannel() and \ref AirpcapSetDeviceKeys() functions: a channel change is 
        /// reflected on all the open instances, and remembered until the next call to \ref AirpcapSetDeviceChannel(), until the adapter 
        /// is unplugged, or until the machine is powered off. Same thing for the configuration of the WEP keys.
        ///
        /// AirpcapStoreCurConfigAsAdapterDefault() stores the configuration of the give open instance as the default for the adapter: 
        /// all the instances opened in the future will have the same configuration that this adapter currently has.
        /// The configuration is stored in the registry, therefore it is remembered even when the adapter is unplugged or the
        /// machine is turned off. However, an adapter doesn't bring its configuration with it from machine to machine.
        ///
        /// the configuration information saved in the registry includes the following parameters:
        ///  - channel
        ///  - kernel buffer size
        ///  - mintocopy
        ///  - link type
        ///  - CRC presence
        ///  - Encryption keys
        ///  - Encryption Enabled/Disabled state
        ///
        /// The configuration is device-specific. This means that changing the configuration of a device
        /// doesn't modify the one of the other devices that are currently used or that will be used in the future.
        ///
        /// \note AirpcapStoreCurConfigAsAdapterDefault() must have exclusive access to the adapter -- it 
        /// will fail if more than one AirPcap handle is opened at the same time for this device. 
        /// AirpcapStoreCurConfigAsAdapterDefault() needs administrator privileges. It will fail if the calling user
        /// is not a local machine administrator.
        /// </summary>
        /// <param name="AdapterHandle">Handle to an adapter</param>
        /// <returns>TRUE on success. FALSE on failure.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapStoreCurConfigAsAdapterDefault(/* PAirpcapHandle */ IntPtr AdapterHandle);

        /// <summary>
        /// Sets the BPF kernel filter for an adapter
        ///
        /// The AirPcap driver is able to perform kernel-level filtering using the standard BPF pseudo-machine format. You can read
        /// the WinPcap documentation at http://www.winpcap.org/devel.htm for more details on the BPF filtering mechaism.
        ///
        /// A filter can be automatically created by using the pcap_compile() function of the WinPcap API. This function 
        /// converts a human readable text expression with the tcpdump/libpcap syntax into a BPF program. 
        /// If your program doesn't link wpcap, but you need to generate the code for a particular filter, you can run WinDump 
        /// with the -d or -dd or -ddd flags to obtain the pseudocode.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="Instructions"> pointer to the first BPF instruction in the array. Corresponds to the  bf_insns 
        /// in a bpf_program structure (see the WinPcap documentation at http://www.winpcap.org/devel.htm).
        /// \param Len Number of instructions in the array pointed by the previous field. Corresponds to the bf_len in
        /// a a bpf_program structure (see the WinPcap documentation at http://www.winpcap.org/devel.htm).</param>
        /// <param name="Len"></param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetFilter(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                     /* PVOID */ IntPtr Instructions,
                                                     /* UINT */ UInt32 Len);

        /// <summary>
        /// Returns the MAC address of a device
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PMacAddress">Pointer to a user allocated \ref AirpcapMacAddress structure that will receive the MAC address on success. </param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetMacAddress(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                         /* PAirpcapMacAddress */ IntPtr PMacAddress);

        /// <summary>
        /// Sets the MAC address of a device
        ///
        /// Using this function, the programmer can change the MAC address of the device. This is useful when disabling monitor
        /// mode with \ref AirpcapSetDeviceMacFlags(), because the device will acknowledge the data frames sent to its MAC address.
        ///
        /// \note The address change is temporary: when the device is unplugged or when the host PC is turned off, the address is reset to the original
        /// value.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PMacAddress">Pointer to a user-initialized structure containing the MAC address</param>
        /// <returns>TRUE on success. FALSE on failure, or if the adapter doesn't support changing the address.</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetMacAddress(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                         /* PAirpcapMacAddress */ IntPtr PMacAddress);

        /// <summary>
        /// Sets the mintocopy parameter for an open adapter
        ///
        /// When the number of bytes in the kernel buffer changes from less than mintocopy bytes to greater than or equal to mintocopy bytes, 
        /// the read event is signalled (see \ref AirpcapGetReadEvent()). A high value for mintocopy results in poor responsiveness since the
        /// driver may signal the application "long" after the arrival of the packet. And a high value results in low CPU loading
        /// by minimizing the number of user/kernel context switches. 
        /// A low MinToCopy results in good responsiveness since the driver will signal the application close to the arrival time of
        ///  the packet. This has higher CPU loading over the first approach.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="MinToCopy">is the mintocopy size in bytes</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapSetMinToCopy(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                        /* UINT */ UInt32 MinToCopy);

        /// <summary>
        /// Gets an event that is signalled when packets are available in the kernel buffer (see \ref AirpcapSetMinToCopy()).
        ///  \note The event is signalled when at least mintocopy bytes are present in the kernel buffer (see \ref AirpcapSetMinToCopy()). 
        ///  This event can be used by WaitForSingleObject() and WaitForMultipleObjects() to create blocking behavior when reading 
        ///  packets from one or more adapters (see \ref AirpcapRead()).
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PReadEvent">Pointer to a user-supplied handle in which the read event will be copied.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetReadEvent(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                        /* HANDLE* */ out IntPtr PReadEvent);

        /// <summary>
        /// Fills a user-provided buffer with zero or more packets that have been captured on the referenced adapter.
        ///
        /// 802.11 frames are returned by the driver in buffers. Every 802.11 frame in the buffer is preceded by a \ref AirpcapBpfHeader structure.
        /// The suggested way to use an AirPcap adapter is through the pcap API exported by wpcap.dll. If this is not
        /// possible, the Capture_radio and Capture_no_radio examples in the AirPcap developer's pack show how to properly decode the 
        /// packets in the read buffer returned by AirpcapRead().
        ///
        /// \note This function is NOT blocking. Blocking behavior can be obtained using the event returned
        /// by \ref AirpcapGetReadEvent(). See also \ref AirpcapSetMinToCopy().
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="Buffer">pointer to the buffer that will be filled with captured packets.</param>
        /// <param name="BufSize">size of the input buffer that will contain the packets, in bytes.</param>
        /// <param name="PReceievedBytes">Pointer to a user supplied variable that will receive the number of bytes copied by AirpcapRead. 
        /// Can be smaller than BufSize.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapRead(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                /* PBYTE */ IntPtr Buffer,
                                                /* UINT */ UInt32 BufSize,
                                                /* PUINT */ out UInt32 PReceievedBytes);

        /// <summary>
        /// Transmits a packet
        ///
        /// The packet will be transmitted on the channel the device is currently set. To change the device adapter, use the 
        /// \ref AirpcapSetDeviceChannel() function.
        ///
        /// If the link type of the adapter is AIRPCAP_LT_802_11, the buffer pointed by TxPacket should contain just the 802.11
        /// packet, without additional information. The packet will be transmitted at 1Mbps.
        ///
        /// If the link type of the adapter is AIRPCAP_LT_802_11_PLUS_RADIO, the buffer pointed by TxPacket should contain a radiotap
        /// header followed by the 802.11 packet. AirpcapWrite will use the rate information in the radiotap header when
        /// transmitting the packet.
        ///
        /// If the link type of the adapter is AIRPCAP_LT_802_11_PLUS_PPI, the buffer pointed by TxPacket should contain a PPI header 
        /// followed by the 802.11 packet. AirpcapWrite will use the rate information in the PPI header when transmitting the packet.
        /// If the packet should be transmitted at a 802.11n rate, the packet must include a PPI 802.11n MAC+PHY Extension header, containing
        /// the rate expressed in terms of MCS, short/long guard interval (SGI/LGI) and 20MHz or 40MHz channel. When the MAC+PHY Extension header is present,
        /// the rate field in the PPI 802.11-Common header is ignored.
        /// By default on 802.11n-capable AirPcap adapters, packets are transmitted with no A-MPDU aggregation. A-MPDU aggregation is controlled by the
        /// adapter, but it's possible to give a hint to the hardware to aggregate some packets by setting the "Aggregate" and "More aggregates" flags in 
        /// the PPI 802.11n MAC+PHY extension header.
        ///
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="TxPacket">Pointer to a buffer that contains the packet to be transmitted.</param>
        /// <param name="PacketLen">Length of the buffer pointed by the TxPacket argument, in bytes</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapWrite(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                 /* PCHAR */ IntPtr TxPacket,
                                                 /* ULONG */ UInt32 PacketLen);

        /// <summary>
        /// Gets per-adapter WinPcap-compatible capture statistics.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PStats">Pointer to a user-allocated AirpcapStats structure that will be filled with statistical information.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetStats(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                    /* PAirpcapStats */ IntPtr PStats);

        /// <summary>
        /// Gets the number of LEDs the referenced adapter has available
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="NumberOfLeds">Number of LEDs available on this adapter</param>
        /// <returns></returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetLedsNumber(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                         /* PUINT */ out UInt32 NumberOfLeds);

        /// <summary>
        /// Turns on one of the adapter's LEDs.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="LedNumber">Zero-based identifier of the LED to turn on</param>
        /// <returns></returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapTurnLedOn(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                     /* UINT */ UInt32 LedNumber);

        /// <summary>
        /// Turns off one of the adapter's LEDs.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="LedNumber">Zero-based identifier of the LED to turn off.</param>
        /// <returns></returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapTurnLedOff(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                      /* UINT */ UInt32 LedNumber);

        /// <summary>
        /// Gets the current value of the device counter used to timestamp packets.
        /// </summary>
        /// <param name="AdapterHandle">Handle to the adapter</param>
        /// <param name="PTimestamp">Pointer to a caller allocated 64bit integer that will receive the device
        /// timestamp, in microseconds.</param>
        /// <returns>TRUE on success</returns>
        [DllImport(AIRPCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool AirpcapGetDeviceTimestamp(/* PAirpcapHandle */ IntPtr AdapterHandle,
                                                              /* PAirpcapDeviceTimestamp */ IntPtr PTimestamp);
        #endregion
    }
}