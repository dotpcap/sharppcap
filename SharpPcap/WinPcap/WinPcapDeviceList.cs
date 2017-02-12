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
 * Copyright 2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;

namespace SharpPcap.WinPcap
{
    /// <summary>
    /// Remote adapter list
    /// </summary>
    public class WinPcapDeviceList : ReadOnlyCollection<WinPcapDevice>
    {
        /// <summary>
        /// Port used by rpcapd by default
        /// </summary>
        public static int RpcapdDefaultPort = 2002;

        private static WinPcapDeviceList instance;

        /// <summary>
        /// Method to retrieve this classes singleton instance
        /// </summary>
        public static WinPcapDeviceList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WinPcapDeviceList();
                }

                return instance;
            }
        }

        /// <summary>
        /// Caution: Use the singlton instance unless you know why you need to call this.
        /// One use is for multiple filters on the same physical device. To apply multiple
        /// filters open the same physical device multiple times, one for each
        /// filter by calling this routine and picking the same device out of each list.
        /// </summary>
        /// <returns>
        /// A <see cref="CaptureDeviceList"/>
        /// </returns>
        public static WinPcapDeviceList New()
        {
            return new WinPcapDeviceList();
        }

        /// <summary>
        /// Represents a strongly typed, read-only list of PcapDevices.
        /// </summary>
        private WinPcapDeviceList() : base(new List<WinPcapDevice>())
        {
            Refresh();
        }

        /// <summary>
        /// Returns a list of devices
        /// </summary>
        /// <param name="address">
        /// A <see cref="IPAddress"/>
        /// </param>
        /// <param name="port">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="remoteAuthentication">
        /// A <see cref="RemoteAuthentication"/>
        /// </param>
        /// <returns>
        /// A <see cref="List<WinPcapDevice>"/>
        /// </returns>
        public static List<WinPcapDevice> Devices(IPAddress address,
                                                  int port,
                                                  RemoteAuthentication remoteAuthentication)
        {
            // build the remote string
            var rmStr = string.Format("rpcap://{0}:{1}",
                                      address,
                                      port);
            return Devices(rmStr,
                           remoteAuthentication);
        }

        private static List<WinPcapDevice> Devices(string rpcapString,
                                                   RemoteAuthentication remoteAuthentication)
        {
            var retval = new List<WinPcapDevice>();

            var devicePtr = IntPtr.Zero;
            var errorBuffer = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors

            // convert the remote authentication structure to unmanaged memory if
            // one was specified
            IntPtr rmAuthPointer;
            if (remoteAuthentication == null)
                rmAuthPointer = IntPtr.Zero;
            else
                rmAuthPointer = remoteAuthentication.GetUnmanaged();

            int result = SafeNativeMethods.pcap_findalldevs_ex(rpcapString,
                                                               rmAuthPointer,
                                                               ref devicePtr,
                                                               errorBuffer);
            // free the memory if any was allocated
            if(rmAuthPointer != IntPtr.Zero)
                Marshal.FreeHGlobal(rmAuthPointer);

            if (result < 0)
                throw new PcapException(errorBuffer.ToString());

            IntPtr nextDevPtr = devicePtr;

            while (nextDevPtr != IntPtr.Zero)
            {
                // Marshal pointer into a struct
                var pcap_if_unmanaged =
                    (LibPcap.PcapUnmanagedStructures.pcap_if)Marshal.PtrToStructure(nextDevPtr,
                                                    typeof(LibPcap.PcapUnmanagedStructures.pcap_if));
                LibPcap.PcapInterface pcap_if = new LibPcap.PcapInterface(pcap_if_unmanaged);

                // create an airpcap device if the device appears to be a
                // airpcap device
                var winpcapDevice = new WinPcapDevice(pcap_if);
                if (winpcapDevice.Name.Contains("airpcap"))
                {
                    retval.Add(new AirPcap.AirPcapDevice(winpcapDevice));
                }
                else
                {
                    retval.Add(new WinPcapDevice(pcap_if));
                }
                nextDevPtr = pcap_if_unmanaged.Next;
            }

            LibPcap.LibPcapSafeNativeMethods.pcap_freealldevs(devicePtr);  // Free unmanaged memory allocation.

            return retval;
        }

        /// <summary>
        /// Retrieve the local devices
        /// </summary>
        /// <returns></returns>
        private static List<WinPcapDevice> GetDevices()
        {
            var rpcapLocalDeviceAddress = "rpcap://";
            return WinPcapDeviceList.Devices(rpcapLocalDeviceAddress, null);
        }

        /// <summary>
        /// Refresh the device list
        /// </summary>
        public void Refresh()
        {
            lock (this)
            {
                // retrieve the current device list
                var newDeviceList = GetDevices();

                // update existing devices with values in the new list
                foreach (var newItem in newDeviceList)
                {
                    foreach (var existingItem in base.Items)
                    {
                        if (newItem.Name == existingItem.Name)
                        {
                            // copy the flags and addresses over
                            existingItem.Interface.Flags = newItem.Interface.Flags;
                            existingItem.Interface.Addresses = newItem.Interface.Addresses;

                            break; // break out of the foreach(existingItem)
                        }
                    }
                }

                // find items the current list is missing
                foreach (var newItem in newDeviceList)
                {
                    bool found = false;
                    foreach (var existingItem in base.Items)
                    {
                        if (existingItem.Name == newItem.Name)
                        {
                            found = true;
                            break;
                        }
                    }

                    // add items that we were missing
                    if (!found)
                    {
                        base.Items.Add(newItem);
                    }
                }

                // find items that we have that the current list is missing
                var itemsToRemove = new List<WinPcapDevice>();
                foreach (var existingItem in base.Items)
                {
                    bool found = false;

                    foreach (var newItem in newDeviceList)
                    {
                        if (existingItem.Name == newItem.Name)
                        {
                            found = true;
                            break;
                        }
                    }

                    // add the PcapDevice we didn't see in the new list
                    if (!found)
                    {
                        itemsToRemove.Add(existingItem);
                    }
                }

                // remove the items outside of the foreach() to avoid
                // enumeration errors
                foreach (var itemToRemove in itemsToRemove)
                {
                    base.Items.Remove(itemToRemove);
                }
            }
        }

        #region Device Indexers
        /// <param name="Name">The name or description of the pcap interface to get.</param>
        public WinPcapDevice this[string Name]
        {
            get
            {
                // lock to prevent issues with multi-threaded access
                // with other methods
                lock (this)
                {
                    var devices = (List<WinPcapDevice>)base.Items;
                    var dev = devices.Find(delegate(WinPcapDevice i) { return i.Name == Name; });
                    var result = dev ?? devices.Find(delegate(WinPcapDevice i) { return i.Description == Name; });

                    if (result == null)
                        throw new IndexOutOfRangeException();
                    return result;
                }
            }
        }
        #endregion
    }
}

