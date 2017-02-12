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
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// List of available Pcap Interfaces.
    /// </summary>
    public class LibPcapLiveDeviceList : ReadOnlyCollection<LibPcapLiveDevice>
    {
        private static LibPcapLiveDeviceList instance;

        /// <summary>
        /// Method to retrieve this classes singleton instance
        /// </summary>
        public static LibPcapLiveDeviceList Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new LibPcapLiveDeviceList();
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
        /// A <see cref="LibPcapLiveDeviceList"/>
        /// </returns>
        public static LibPcapLiveDeviceList New()
        {
            return new LibPcapLiveDeviceList();
        }

        /// <summary>
        /// Represents a strongly typed, read-only list of PcapDevices.
        /// </summary>
        private LibPcapLiveDeviceList() : base(new List<LibPcapLiveDevice>())
        {
            Refresh();
        }

        /// <summary>
        /// Retrieve a list of the current PcapDevices
        /// </summary>
        /// <returns>
        /// A <see cref="List&lt;LibPcapLiveDevice&gt;"/>
        /// </returns>
        private static List<LibPcapLiveDevice> GetDevices()
        {
            var deviceList = new List<LibPcapLiveDevice>();

            var devicePtr = IntPtr.Zero;
            var errorBuffer = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE);

            int result = LibPcapSafeNativeMethods.pcap_findalldevs(ref devicePtr, errorBuffer);
            if (result < 0)
                throw new PcapException(errorBuffer.ToString());

            IntPtr nextDevPtr = devicePtr;

            while (nextDevPtr != IntPtr.Zero)
            {
                // Marshal pointer into a struct
                PcapUnmanagedStructures.pcap_if pcap_if_unmanaged =
                    (PcapUnmanagedStructures.pcap_if)Marshal.PtrToStructure(nextDevPtr,
                                                    typeof(PcapUnmanagedStructures.pcap_if));
                PcapInterface pcap_if = new PcapInterface(pcap_if_unmanaged);
                deviceList.Add(new LibPcapLiveDevice(pcap_if));
                nextDevPtr = pcap_if_unmanaged.Next;
            }
            LibPcapSafeNativeMethods.pcap_freealldevs(devicePtr);  // Free unmanaged memory allocation.

            return deviceList;
        }

        /// <summary>
        /// Refresh the device list
        /// </summary>
        public void Refresh()
        {
            lock(this)
            {
                // retrieve the current device list
                var newDeviceList = GetDevices();

                // update existing devices with values in the new list
                foreach(var newItem in newDeviceList)
                {
                    foreach(var existingItem in base.Items)
                    {
                        if(newItem.Name == existingItem.Name)
                        {
                            // copy the flags and addresses over
                            existingItem.Interface.Flags = newItem.Interface.Flags;
                            existingItem.Interface.Addresses = newItem.Interface.Addresses;

                            break; // break out of the foreach(existingItem)
                        }
                    }
                }

                // find items the current list is missing
                foreach(var newItem in newDeviceList)
                {
                    bool found = false;
                    foreach(var existingItem in base.Items)
                    {
                        if(existingItem.Name == newItem.Name)
                        {
                            found = true;
                            break;
                        }
                    }

                    // add items that we were missing
                    if(!found)
                    {
                        base.Items.Add(newItem);
                    }
                }

                // find items that we have that the current list is missing
                var itemsToRemove = new List<LibPcapLiveDevice>();
                foreach(var existingItem in base.Items)
                {
                    bool found = false;

                    foreach(var newItem in newDeviceList)
                    {
                        if(existingItem.Name == newItem.Name)
                        {
                            found = true;
                            break;
                        }
                    }

                    // add the PcapDevice we didn't see in the new list
                    if(!found)
                    {
                        itemsToRemove.Add(existingItem);
                    }
                }

                // remove the items outside of the foreach() to avoid
                // enumeration errors
                foreach(var itemToRemove in itemsToRemove)
                {
                    base.Items.Remove(itemToRemove);
                }
            }
        }

        #region PcapDevice Indexers
        /// <param name="Name">The name or description of the pcap interface to get.</param>
        public LibPcapLiveDevice this[string Name]
        {
            get
            {
                // lock to prevent issues with multi-threaded access
                // with other methods
                lock(this)
                {
                    var devices = (List<LibPcapLiveDevice>)base.Items;
                    var dev = devices.Find(delegate(LibPcapLiveDevice i) { return i.Name == Name; });
                    var result = dev ?? devices.Find(delegate(LibPcapLiveDevice i) { return i.Description == Name; });

                    if (result == null)
                        throw new IndexOutOfRangeException();
                    return result;
                }
            }
        }
        #endregion
    }
}
