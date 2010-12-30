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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.NetworkInformation;

namespace SharpPcap.AirPcap
{
    public class AirPcapDevice : ICaptureDevice
    {
        protected void ThrowIfNotOpen()
        {
            ThrowIfNotOpen("");
        }

        /// <summary>
        /// Helper method for checking that the adapter is open, throws an
        /// exception with a string of ExceptionString if the device isn't open
        /// </summary>
        /// <param name="ExceptionString">
        /// A <see cref="System.String"/>
        /// </param>
        protected void ThrowIfNotOpen(string ExceptionString)
        {
            if (!Opened)
            {
                throw new DeviceNotReadyException(ExceptionString);
            }
        }

        protected AirPcapDeviceDescription DeviceDescription { get; set; }

        /// <summary>
        /// Handle to the device
        /// </summary>
        internal IntPtr DeviceHandle { get; set; }

        internal AirPcapDevice(AirPcapDeviceDescription desc)
        {
            DeviceDescription = desc;
        }

        public override string ToString()
        {
            return string.Format("DeviceDescription: {0}", DeviceDescription.ToString());
        }

        public string Name
        {
            get { return DeviceDescription.Name; }
        }

        public string Description
        {
            get { return DeviceDescription.Description; }
        }

        /// <summary>
        /// Retrieve the last error string for a given pcap_t* device
        /// </summary>
        /// <param name="deviceHandle">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        internal static string GetLastError(IntPtr deviceHandle)
        {
            IntPtr err_ptr = AirPcapSafeNativeMethods.AirpcapGetLastError(deviceHandle);
            return Marshal.PtrToStringAnsi(err_ptr);
        }

        /// <summary>
        /// The last pcap error associated with this pcap device
        /// </summary>
        public string LastError
        {
            get { return GetLastError(DeviceHandle); }
        }

        ///FIXME: Consider making Opened a property of ICaptureDevice since
        ///all adapters so far have some way of indicating whether they are open and its a useful
        ///feature to have
        /// <summary>
        /// Return a value indicating if this adapter is opened
        /// </summary>
        public virtual bool Opened
        {
            get { return (DeviceHandle != IntPtr.Zero); }
        }

        /// <summary>
        /// Fires whenever a new packet is processed, either when the packet arrives
        /// from the network device or when the packet is read from the on-disk file.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Capture" mode.
        /// </summary>
        public event PacketArrivalEventHandler OnPacketArrival;

        /// <summary>
        /// Fired when the capture process of this pcap device is stopped
        /// </summary>
        public event CaptureStoppedEventHandler OnCaptureStopped;

        /// <summary>A delegate for AirPcap specific packet Arrival events</summary>
        public delegate void AirPcapPacketArrivalEventHandler(object sender, AirPcapCaptureEventArgs e);

        /// <summary>
        /// AirPcap specific packet arrival event
        /// </summary>
        public event AirPcapPacketArrivalEventHandler OnAirPcapPacketArrival;

        public void Open()
        {
            StringBuilder errbuf = new StringBuilder( AIRPCAP_ERRBUF_SIZE ); //will hold errors
            DeviceHandle = AirPcapSafeNativeMethods.AirpcapOpen(Name, errbuf);

            if (DeviceHandle == IntPtr.Zero)
            {
                string err = "Unable to open the adapter (" + Name + "). " + errbuf.ToString();
                throw new PcapException(err);
            }
        }

        public void Close()
        {
            if (!Opened)
                return;

            AirPcapSafeNativeMethods.AirpcapClose(DeviceHandle);
            DeviceHandle = IntPtr.Zero;
        }

        /// <summary>
        /// Device capabilities, whether the device can transmit, its id, model name etc
        /// </summary>
        public AirPcapDeviceCapabilities Capabilities
        {
            get
            {
                ThrowIfNotOpen();

                IntPtr capablitiesPointer;
                if(!AirPcapSafeNativeMethods.AirpcapGetDeviceCapabilities(DeviceHandle, out capablitiesPointer))
                {
                    throw new InvalidOperationException("error retrieving device capabilities");
                }

                return new AirPcapDeviceCapabilities(capablitiesPointer);
            }
        }

        public uint Channel
        {
            get
            {
                ThrowIfNotOpen();
                UInt32 channel;
                if (!AirPcapSafeNativeMethods.AirpcapGetDeviceChannel(DeviceHandle, out channel))
                {
                    throw new System.InvalidOperationException("Failed to retrieve channel");
                }
                return channel;
            }

            set
            {
                ThrowIfNotOpen();
                if (!AirPcapSafeNativeMethods.AirpcapSetDeviceChannel(DeviceHandle, value))
                {
                    throw new System.InvalidOperationException("Failed to set channel");
                }
            }
        }

        public AirPcapChannelInfo ChannelInfo
        {
            get
            {
                ThrowIfNotOpen();

                AirPcapUnmanagedStructures.AirpcapChannelInfo channelInfo;
                if(!AirPcapSafeNativeMethods.AirpcapGetDeviceChannelEx(DeviceHandle, out channelInfo))
                {
                    throw new System.InvalidOperationException("Failed to get channel ex");
                }

                return new AirPcapChannelInfo(channelInfo);
            }

            set
            {
                ThrowIfNotOpen();

                var channelInfo = value.UnmanagedInfo;
                if (!AirPcapSafeNativeMethods.AirpcapSetDeviceChannelEx(DeviceHandle, channelInfo))
                {
                    throw new System.InvalidOperationException("Failed to set channel ex");
                }
            }
        }

        /// <summary>
        /// Size in bytes of a key collection with a given count of keys
        /// </summary>
        /// <param name="keyCount"></param>
        /// <returns></returns>
        private static int KeyCollectionSize(int keyCount)
        {
            int memorySize = (int)(Marshal.SizeOf(typeof(AirPcapUnmanagedStructures.AirpcapKeysCollection)) +
                                   (Marshal.SizeOf(typeof(AirPcapUnmanagedStructures.AirpcapKey)) * keyCount));
            return memorySize;
        }

        /// <summary>
        /// Convert a AirpcapKeysCollection unmanaged buffer to a list of managed keys
        /// </summary>
        /// <param name="pKeysCollection"></param>
        /// <returns></returns>
        private static List<AirPcapKey> IntPtrToKeys(IntPtr pKeysCollection)
        {
            var retval = new List<AirPcapKey>();

            // marshal the memory into a keys collection
            var keysCollection = (AirPcapUnmanagedStructures.AirpcapKeysCollection)Marshal.PtrToStructure(pKeysCollection,
                                                    typeof(AirPcapUnmanagedStructures.AirpcapKeysCollection));

            // go through the keys, offset from the start of the collection to the first key 
            IntPtr pKeys = new IntPtr(pKeysCollection.ToInt64() + Marshal.SizeOf(typeof(AirPcapUnmanagedStructures.AirpcapKeysCollection)));

            for (int x = 0; x < keysCollection.nKeys; x++)
            {
                // convert the key entry from unmanaged memory to managed memory
                var airpcapKey = (AirPcapUnmanagedStructures.AirpcapKey)Marshal.PtrToStructure(pKeys, typeof(AirPcapUnmanagedStructures.AirpcapKey));

                // convert the now managed key into the key representation we want to see
                retval.Add(new AirPcapKey(airpcapKey));

                // advance the pointer to the next key in the collection
                pKeys = new IntPtr(pKeys.ToInt64() + Marshal.SizeOf(typeof(AirPcapUnmanagedStructures.AirpcapKey)));
            }

            return retval;
        }

        /// <summary>
        /// Convert an array of keys into unmanaged memory
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IntPtr KeysToIntPtr(List<AirPcapKey> value)
        {
            // allocate memory for the entire collection
            IntPtr pKeyCollection = Marshal.AllocHGlobal(AirPcapDevice.KeyCollectionSize(value.Count));
            var pKeyCollectionPosition = pKeyCollection;

            // build the collection struct
            var collection = new AirPcapUnmanagedStructures.AirpcapKeysCollection();
            collection.nKeys = (uint)value.Count;

            // convert this collection to unmanaged memory
            Marshal.StructureToPtr(collection, pKeyCollectionPosition, false);

            // advance the pointer
            pKeyCollectionPosition = new IntPtr(pKeyCollectionPosition.ToInt64() +
                                        Marshal.SizeOf(typeof(AirPcapUnmanagedStructures.AirpcapKeysCollection)));

            // write the keys to memory
            for (int x = 0; x < value.Count; x++)
            {
                var key = new AirPcapUnmanagedStructures.AirpcapKey();
                key.KeyType = value[x].Type;
                key.KeyLen = (uint)value[x].Data.Length;

                // make sure we have the right size byte[], the fields in the structure passed to Marshal.StructureToPtr()
                // have to match the specified sizes or an exception will be thrown
                key.KeyData = new byte[AirPcapUnmanagedStructures.WepKeyMaxSize];
                Array.Copy(value[x].Data, key.KeyData, value[x].Data.Length);

                // copy the managed memory into the unmanaged memory
                Marshal.StructureToPtr(key, pKeyCollectionPosition, false);

                // advance the pointer
                pKeyCollectionPosition = new IntPtr(pKeyCollectionPosition.ToInt64() + Marshal.SizeOf(typeof(AirPcapUnmanagedStructures.AirpcapKey)));
            }

            return pKeyCollection;
        }

        /// <summary>
        /// Decryption keys that are currently associated with the specified device
        /// </summary>
        public List<AirPcapKey> DeviceKeys
        {
            get
            {
                ThrowIfNotOpen();

                // Request the key collection size
                uint keysCollectionSize = 0;
                if (AirPcapSafeNativeMethods.AirpcapGetDeviceKeys(DeviceHandle, IntPtr.Zero,
                                                              ref keysCollectionSize))
                {
                    // return value of true with an input size of zero indicates there are no
                    // device keys
                    return null;
                }

                // now that we have the desired collection size, allocate the appropriate memory
                //var memorySize = AirPcapDevice.KeyCollectionSize(keysCollectionSize);
                var pKeysCollection = Marshal.AllocHGlobal((int)keysCollectionSize);

                try
                {
                    if (!AirPcapSafeNativeMethods.AirpcapGetDeviceKeys(DeviceHandle, pKeysCollection,
                                                                      ref keysCollectionSize))
                    {
                        throw new System.InvalidOperationException("Unexpected false from AirpcapGetDeviceKeys()");
                    }

                    // convert the unmanaged memory to an array of keys
                    return IntPtrToKeys(pKeysCollection);
                }
                finally
                {
                    Marshal.FreeHGlobal(pKeysCollection);
                }
            }

            set
            {
                ThrowIfNotOpen();

                var pKeyCollection = KeysToIntPtr(value);
                try
                {
                    if (!AirPcapSafeNativeMethods.AirpcapSetDeviceKeys(DeviceHandle, pKeyCollection))
                    {
                        throw new System.InvalidOperationException("Unable to set device keys");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(pKeyCollection);
                }
            }
        }

        /// <summary>
        /// Global list of decryption keys that AirPcap is using with all the devices.
        /// </summary>
        public List<AirPcapKey> DriverKeys
        {
            get
            {
                ThrowIfNotOpen();

                // Request the key collection size
                uint keysCollectionSize = 0;
                if (AirPcapSafeNativeMethods.AirpcapGetDriverKeys(DeviceHandle, IntPtr.Zero,
                                                                  ref keysCollectionSize))
                {
                    // return value of true with an input size of zero indicates there are no
                    // device keys
                    return null;
                }

                // now that we have the desired collection size, allocate the appropriate memory
                //var memorySize = AirPcapDevice.KeyCollectionSize(keysCollectionSize);
                var pKeysCollection = Marshal.AllocHGlobal((int)keysCollectionSize);

                try
                {
                    if (!AirPcapSafeNativeMethods.AirpcapGetDriverKeys(DeviceHandle, pKeysCollection,
                                                                       ref keysCollectionSize))
                    {
                        throw new System.InvalidOperationException("Unexpected false from AirpcapGetDriverKeys()");
                    }

                    // convert the unmanaged memory to an array of keys
                    return IntPtrToKeys(pKeysCollection);
                }
                finally
                {
                    Marshal.FreeHGlobal(pKeysCollection);
                }
            }

            set
            {
                ThrowIfNotOpen();

                var pKeyCollection = KeysToIntPtr(value);
                try
                {
                    if (!AirPcapSafeNativeMethods.AirpcapSetDriverKeys(DeviceHandle, pKeyCollection))
                    {
                        throw new System.InvalidOperationException("Unable to set driver keys");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(pKeyCollection);
                }
            }
        }

        /// <summary>
        /// Tells if decryption of the incoming frames with the <b>device-specific</b> keys.
        /// </summary>
        public AirPcapDecryptionState DecryptionState
        {
            get
            {
                ThrowIfNotOpen();

                AirPcapDecryptionState state;
                if (!AirPcapSafeNativeMethods.AirpcapGetDecryptionState(DeviceHandle, out state))
                {
                    throw new System.InvalidOperationException("Failed to get decryption state");
                }
                return state;
            }

            set
            {
                ThrowIfNotOpen();

                if (!AirPcapSafeNativeMethods.AirpcapSetDecryptionState(DeviceHandle, value))
                {
                    throw new System.InvalidOperationException("Failed to set decryption state");
                }
            }
        }

        /// <summary>
        /// Tells if this open instance is configured to perform the decryption of the incoming frames with the <b>global</b> set of keys.
        /// </summary>
        public AirPcapDecryptionState DriverDecryptionState
        {
            get
            {
                ThrowIfNotOpen();

                AirPcapDecryptionState state;
                if (!AirPcapSafeNativeMethods.AirpcapGetDriverDecryptionState(DeviceHandle, out state))
                {
                    throw new System.InvalidOperationException("Failed to get driver decryption state");
                }
                return state;
            }

            set
            {
                ThrowIfNotOpen();

                if(AirPcapSafeNativeMethods.AirpcapSetDriverDecryptionState(DeviceHandle, value))
                {
                    throw new System.InvalidOperationException("Failed to set decryption state");
                }
            }
        }

        /// <summary>
        /// Configures the adapter on whether to include the MAC Frame Check Sequence in the captured packets.
        /// </summary>
        public bool FcsPresence
        {
            get
            {
                ThrowIfNotOpen();

                bool isFcsPresent;
                if (!AirPcapSafeNativeMethods.AirpcapGetFcsPresence(DeviceHandle, out isFcsPresent))
                {
                    throw new System.InvalidOperationException("Failed to get fcs presence");
                }
                return isFcsPresent;
            }

            set
            {
                ThrowIfNotOpen();

                if (!AirPcapSafeNativeMethods.AirpcapSetFcsPresence(DeviceHandle, value))
                {
                    throw new System.InvalidOperationException("Failed to set fcs presence");
                }
            }
        }

        /// <summary>
        /// The kinds of frames that the device will capture
        /// By default all frames are captured
        /// </summary>
        public AirPcapValidationType FcsValidation
        {
            get
            {
                ThrowIfNotOpen();

                AirPcapValidationType validationType;
                if (!AirPcapSafeNativeMethods.AirpcapGetFcsValidation(DeviceHandle, out validationType))
                {
                    throw new System.InvalidOperationException("Failed to get fcs validation");
                }
                return validationType;
            }

            set
            {
                ThrowIfNotOpen();

                if (!AirPcapSafeNativeMethods.AirpcapSetFcsValidation(DeviceHandle, value))
                {
                    throw new System.InvalidOperationException("failed to set fcs validation");
                }
            }
        }

        /// <summary>
        /// Kernel packet buffer size for this adapter in bytes
        /// </summary>
        public uint KernelBufferSize
        {
            get
            {
                ThrowIfNotOpen();

                uint kernelBufferSize;
                if(!AirPcapSafeNativeMethods.AirpcapGetKernelBufferSize(DeviceHandle, out kernelBufferSize))
                {
                    throw new System.InvalidOperationException("failed to get kernel buffer size");
                }
                return kernelBufferSize;
            }

            set
            {
                ThrowIfNotOpen();

                if (!AirPcapSafeNativeMethods.AirpcapSetKernelBuffer(DeviceHandle, value))
                {
                    throw new System.InvalidOperationException("failed to set kernel buffer size");
                }
            }
        }

        public int LedCount
        {
            get
            {
                ThrowIfNotOpen();

                UInt32 numberOfLeds;
                AirPcapSafeNativeMethods.AirpcapGetLedsNumber(DeviceHandle, out numberOfLeds);
                return (int)numberOfLeds;
            }
        }

        public enum LedState
        {
            On,
            Off
        };

        public void Led(int ledIndex, LedState newLedState)
        {
            ThrowIfNotOpen();

            if (newLedState == LedState.On)
            {
                AirPcapSafeNativeMethods.AirpcapTurnLedOn(DeviceHandle, (UInt32)ledIndex);
            }
            else if (newLedState == LedState.Off)
            {
                AirPcapSafeNativeMethods.AirpcapTurnLedOff(DeviceHandle, (UInt32)ledIndex);
            }
        }

        public AirPcapLinkType LinkType
        {
            get
            {
                ThrowIfNotOpen("Requires an open device");

                AirPcapLinkType linkType;

                AirPcapSafeNativeMethods.AirpcapGetLinkType(DeviceHandle,
                                                            out linkType);


                return linkType;
            }

            set
            {
                ThrowIfNotOpen("Requires an open device");

                if (!AirPcapSafeNativeMethods.AirpcapSetLinkType(DeviceHandle,
                                                                value))
                {
                    throw new InvalidOperationException("Setting link type failed");
                }
            }
        }

        /// <summary>
        /// TODO: Get this from packet.net or another place in System.Net.xxx?
        /// </summary>
        private const int MacAddressSizeInBytes = 6;

        public PhysicalAddress MacAddress
        {
            get
            {
                ThrowIfNotOpen();

                var address = new byte[MacAddressSizeInBytes];
                IntPtr addressUnmanaged = Marshal.AllocHGlobal(MacAddressSizeInBytes);
                try
                {
                    if (!AirPcapSafeNativeMethods.AirpcapGetMacAddress(DeviceHandle, addressUnmanaged))
                    {
                        throw new System.InvalidOperationException("Unable to get mac address");
                    }

                    Marshal.Copy(addressUnmanaged, address, 0, address.Length);

                    return new PhysicalAddress(address);
                }
                finally
                {
                    Marshal.FreeHGlobal(addressUnmanaged);
                }
            }

            set
            {
                ThrowIfNotOpen();

                var address = value.GetAddressBytes();
                var addressUnmanaged = Marshal.AllocHGlobal(address.Length);
                try
                {
                    if (!AirPcapSafeNativeMethods.AirpcapSetMacAddress(DeviceHandle, addressUnmanaged))
                    {
                        throw new System.InvalidOperationException("Unable to set mac address");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(addressUnmanaged);
                }
            }
        }

        public AirPcapMacFlags MacFlags
        {
            get
            {
                ThrowIfNotOpen();

                AirPcapMacFlags macFlags;
                if (!AirPcapSafeNativeMethods.AirpcapGetDeviceMacFlags(DeviceHandle, out macFlags))
                {
                    throw new System.InvalidOperationException("Failed to get device mac flags");
                }
                return macFlags;
            }

            set
            {
                ThrowIfNotOpen();

                if (!AirPcapSafeNativeMethods.AirpcapSetDeviceMacFlags(DeviceHandle, value))
                {
                    throw new System.InvalidOperationException("Failed to set device mac flags");
                }
            }
        }

        public List<AirPcapChannelInfo> SupportedChannels
        {
            get
            {
                ThrowIfNotOpen();

                var retval = new List<AirPcapChannelInfo>();
                IntPtr pChannelInfo;
                uint numChannelInfo;

                if (!AirPcapSafeNativeMethods.AirpcapGetDeviceSupportedChannels(DeviceHandle, out pChannelInfo, out numChannelInfo))
                {
                    throw new System.InvalidOperationException("Failed to get device supported channels");
                }

                for (int x = 0; x < numChannelInfo; x++)
                {
                    var unmanagedChannelInfo = (AirPcapUnmanagedStructures.AirpcapChannelInfo)Marshal.PtrToStructure(pChannelInfo,
                                                                                                            typeof(AirPcapUnmanagedStructures.AirpcapChannelInfo));

                    var channelInfo = new AirPcapChannelInfo(unmanagedChannelInfo);

                    retval.Add(channelInfo);

                    // advance the pointer to the next address
                    pChannelInfo = new IntPtr(pChannelInfo.ToInt64() + Marshal.SizeOf(typeof(AirPcapUnmanagedStructures.AirpcapChannelInfo)));
                }

                return retval;
            }
        }

        public uint TxPower
        {
            get
            {
                ThrowIfNotOpen();

                uint power;
                if (!AirPcapSafeNativeMethods.AirpcapGetTxPower(DeviceHandle, out power))
                {
                    throw new System.NotSupportedException("Unable to retrieve the tx power for this adapter");
                }
                return power;
            }

            set
            {
                ThrowIfNotOpen();

                if (!AirPcapSafeNativeMethods.AirpcapSetTxPower(DeviceHandle, value))
                {
                    throw new System.NotSupportedException("Unable to set the tx power for this adapter");
                }
            }
        }

        public AirPcapDeviceTimestamp Timestamp
        {
            get
            {
                ThrowIfNotOpen();

                var pTimestamp = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(AirPcapUnmanagedStructures.AirpcapDeviceTimestamp)));
                try
                {
                    if (!AirPcapSafeNativeMethods.AirpcapGetDeviceTimestamp(DeviceHandle, pTimestamp))
                    {
                        throw new System.NotSupportedException("Failed to get device timestamp");
                    }

                    var timestamp = (AirPcapUnmanagedStructures.AirpcapDeviceTimestamp)Marshal.PtrToStructure(pTimestamp,
                                                        typeof(AirPcapUnmanagedStructures.AirpcapDeviceTimestamp));

                    return new AirPcapDeviceTimestamp(timestamp);
                }
                finally
                {
                    Marshal.FreeHGlobal(pTimestamp);
                }
            }
        }

        /// <summary>
        /// Notify the OnPacketArrival delegates about a newly captured packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="PacketDotNet.RawPacket"/>
        /// </param>
        protected void SendPacketArrivalEvent(AirPcapRawPacket p)
        {
            // invoke the packet arrival handle from the base class
            var arrivalHandler = OnPacketArrival;
            if (arrivalHandler != null)
            {
                arrivalHandler(this, new SharpPcap.CaptureEventArgs(p, this));
            }

            // invoke the airpcap specific arrival handler
            var airpcapArrivalHandle = OnAirPcapPacketArrival;
            if (airpcapArrivalHandle != null)
            {
                airpcapArrivalHandle(this, new AirPcapCaptureEventArgs(p, this));
            }
        }

        /// <summary>
        /// Notify the delegates that are subscribed to the capture stopped event
        /// </summary>
        /// <param name="status">
        /// A <see cref="CaptureStoppedEventStatus"/>
        /// </param>
        private void SendCaptureStoppedEvent(CaptureStoppedEventStatus status)
        {
            var handler = OnCaptureStopped;
            if (handler != null)
            {
                handler(this, status);
            }
        }

        /// <summary>
        /// Return a value indicating if the capturing process of this adapter is started
        /// </summary>
        public virtual bool Started
        {
            get { return (captureThread != null); }
        }

        // time we give the capture thread to stop before we assume that
        // there is an error
        private TimeSpan stopCaptureTimeout = new TimeSpan(0, 0, 1);

        /// <summary>
        /// Maximum time within which the capture thread must join the main thread (on 
        /// <see cref="StopCapture"/>) or else the thread is aborted and an exception thrown.
        /// </summary>
        public TimeSpan StopCaptureTimeout
        {
            get { return stopCaptureTimeout; }
            set { stopCaptureTimeout = value; }
        }

        private Thread captureThread;
        private bool shouldCaptureThreadStop;

        public void StartCapture()
        {
            if(!Started)
            {
                if (!Opened)
                    throw new DeviceNotReadyException("Can't start capture, AirPcap device is not open");

                if ((OnPacketArrival == null) && (OnAirPcapPacketArrival == null))
                    throw new DeviceNotReadyException("No delegates assigned to OnPacketArrival or OnAirPcapPacketArrival, no where for captured packets to go.");

                shouldCaptureThreadStop = false;
                captureThread = new Thread(new ThreadStart(this.CaptureThread));
                captureThread.Start();
            }
        }

        public void StopCapture()
        {
            if (Started)
            {
                shouldCaptureThreadStop = true;
                if(!captureThread.Join(StopCaptureTimeout))
                {
                    captureThread.Abort();
                    captureThread = null;
                    string error;

                     error = string.Format("captureThread was aborted after {0}",
                                           StopCaptureTimeout.ToString());

                    throw new PcapException(error);
                }

                captureThread = null; // otherwise we will always return true from PcapDevice.Started
            }
        }

        private void CaptureThread()
        {
            IntPtr ReadEvent;
            IntPtr WaitIntervalMilliseconds = (IntPtr)500;

            //
            // Get the read event
            //
            if (!AirPcapSafeNativeMethods.AirpcapGetReadEvent(DeviceHandle, out ReadEvent))
            {
                SendCaptureStoppedEvent(CaptureStoppedEventStatus.ErrorWhileCapturing);
                Console.WriteLine("Error getting the read event: %s\n", LastError);
                Close();
                return;
            }

            // allocate a packet bufer in unmanaged memory
            var packetBufferSize = 256000;
            var packetBuffer = Marshal.AllocHGlobal(packetBufferSize);

            UInt32 BytesReceived;

            List<AirPcapRawPacket> packets;

            while (!shouldCaptureThreadStop)
            {
                // capture the packets
                if (!AirPcapSafeNativeMethods.AirpcapRead(DeviceHandle,
                    packetBuffer,
                   (uint)packetBufferSize,
                    out BytesReceived))
                {
                    Console.WriteLine("Error receiving packets: %s\n", this.LastError);
                    Marshal.FreeHGlobal(packetBuffer);
                    Close();
                    SendCaptureStoppedEvent(CaptureStoppedEventStatus.ErrorWhileCapturing);
                    return;
                }

                Console.WriteLine("BytesReceived: {0}", (int)BytesReceived);

                var bufferEnd = new IntPtr(packetBuffer.ToInt64() + (long)BytesReceived);

                MarshalPackets(packetBuffer, bufferEnd, out packets);

                foreach (var p in packets)
                {
                    SendPacketArrivalEvent(p);
                }

                // wait until some packets are available. This prevents polling and keeps the CPU low. 
                Win32SafeNativeMethods.WaitForSingleObject(ReadEvent, WaitIntervalMilliseconds);
            }

            Marshal.FreeHGlobal(packetBuffer);
        }

        protected virtual void MarshalPackets(IntPtr packetsBuffer, IntPtr bufferEnd,
                                              out List<AirPcapRawPacket> packets)
        {
            AirPcapRawPacket p;

            packets = new List<AirPcapRawPacket>();

            IntPtr bufferPointer = packetsBuffer;

            Console.WriteLine("start");

            while (bufferPointer.ToInt64() < bufferEnd.ToInt64())
            {
                Console.WriteLine("bufferPointer {0}, bufferEnd {1}",
                                  bufferPointer.ToInt64(),
                                  bufferEnd.ToInt64());

                // marshal the header
                var header = new AirPcapPacketHeader(bufferPointer);

                Console.WriteLine("header {0}", header.ToString());

                bufferPointer = new IntPtr(bufferPointer.ToInt64() + header.Hdrlen);

                // marshal the radio header
                var radioHeader = new AirPcapRadioHeader(bufferPointer);

                Console.WriteLine("radioHeader {0}", radioHeader.ToString());

                // advance the pointer past the radio header to point at the packet data
                bufferPointer = new IntPtr(bufferPointer.ToInt64() + radioHeader.Length);

                var packetDataLength = header.Caplen - radioHeader.Length;

                var pkt_data = new byte[packetDataLength];
                Marshal.Copy(bufferPointer, pkt_data, 0, (int)packetDataLength);

                //FIXME: not sure what the link layer value should be here...
                p = new AirPcapRawPacket(radioHeader,
                                         PacketDotNet.LinkLayers.Ieee80211,
                                         new PacketDotNet.PosixTimeval(header.TsSec,
                                                                       header.TsUsec),
                                         pkt_data);

                packets.Add(p);

                // advance the pointer by the captured data less the radio header length
                int alignment = 4;
                var pointer = bufferPointer.ToInt64() + packetDataLength;
                pointer = AirPcapDevice.RoundUp(pointer, alignment);
                bufferPointer = new IntPtr(pointer);
            }
        }

        private static long RoundUp(long num, int multiple)
        {
            if (multiple == 0)
                return 0;
            int add = multiple / Math.Abs(multiple);
            return ((num + multiple - add) / multiple) * multiple;
        }
        internal static int AIRPCAP_ERRBUF_SIZE = 512;
    }
}
