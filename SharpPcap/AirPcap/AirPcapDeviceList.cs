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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// AirPcap device list
    /// </summary>
    public class AirPcapDeviceList : ReadOnlyCollection<ICaptureDevice>
    {
        private static AirPcapDeviceList instance;

        /// <summary>
        /// Method to retrieve this classes singleton instance
        /// </summary>
        public static AirPcapDeviceList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AirPcapDeviceList();
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
        public static AirPcapDeviceList New()
        {
            return new AirPcapDeviceList();
        }

        /// <summary>
        /// Represents a strongly typed, read-only list of PcapDevices.
        /// </summary>
        private AirPcapDeviceList()
            : base(new List<ICaptureDevice>())
        {
            Refresh();
        }

        /// <summary>
        /// Retrieve an array of AirPcapDevices
        /// </summary>
        /// <returns></returns>
        private static List<AirPcapDevice> GetDevices()
        {
            var airPcapDeviceList = new List<AirPcapDevice>();

            // using the list of winpcap devices build a list of
            // AirPcapDevice entries
            var winpcapDeviceList = WinPcap.WinPcapDeviceList.Instance;
            foreach (var d in winpcapDeviceList)
            {
                if (d.Name.Contains("airpcap"))
                {
                    airPcapDeviceList.Add(new AirPcapDevice(d));
                }
            }

            return airPcapDeviceList;
        }

        /// <summary>
        /// Refresh the device list
        /// </summary>
        public void Refresh()
        {
            lock (this)
            {
                // refresh the WinPcapDeviceList as this list is how we build
                // the airpcap device list
                var winpcapDeviceList = WinPcap.WinPcapDeviceList.Instance;
                winpcapDeviceList.Refresh();

                // retrieve the current device list
                var newDeviceList = GetDevices();

                // update existing devices with values in the new list
                foreach (var newItem in newDeviceList)
                {
                    foreach (var existingItem in base.Items)
                    {
                        if (newItem.Name == existingItem.Name)
                        {
                            // TODO: if we have entries to copy from new to existing, copy them here

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
                var itemsToRemove = new List<ICaptureDevice>();
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

        #region PcapDevice Indexers
        /// <param name="Name">The name or description of the pcap interface to get.</param>
        public AirPcapDevice this[string Name]
        {
            get
            {
                // lock to prevent issues with multi-threaded access
                // with other methods
                lock (this)
                {
                    var devices = (List<AirPcapDevice>)base.Items;
                    var dev = devices.Find(delegate(AirPcapDevice i) { return i.Name == Name; });
                    var result = dev ?? devices.Find(delegate(AirPcapDevice i) { return i.Description == Name; });

                    if (result == null)
                        throw new IndexOutOfRangeException();
                    return result;
                }
            }
        }
        #endregion

        /// <summary>
        /// Example showing how to retrieve a list of AirPcap devices
        /// We can't use this because AirPcap devices don't have a pcap handle
        /// but this code is worth keeping around
        /// </summary>
        /// <returns>
        /// A <see cref="List<AirPcapDevice>"/>
        /// </returns>
        private static List<AirPcapDevice> GetAirPcapDevices()
        {
            var deviceList = new List<AirPcapDevice>();

            var devicePtr = IntPtr.Zero;
            var errorBuffer = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE);

            int result = AirPcapSafeNativeMethods.AirpcapGetDeviceList(ref devicePtr, errorBuffer);
            if (result < 0)
                throw new PcapException(errorBuffer.ToString());

            IntPtr nextDevPtr = devicePtr;

            while (nextDevPtr != IntPtr.Zero)
            {
                // Marshal pointer into a struct
                AirPcapUnmanagedStructures.AirpcapDeviceDescription deviceDescUnmanaged =
                    (AirPcapUnmanagedStructures.AirpcapDeviceDescription)Marshal.PtrToStructure(nextDevPtr,
                                                    typeof(AirPcapUnmanagedStructures.AirpcapDeviceDescription));
                var managedDeviceDesc = new AirPcapDeviceDescription(deviceDescUnmanaged);
//                deviceList.Add(new AirPcapDevice(managedDeviceDesc));
                nextDevPtr = deviceDescUnmanaged.next;
            }
            AirPcapSafeNativeMethods.AirpcapFreeDeviceList(devicePtr); // Free unmanaged memory

            return deviceList;
        }
    }
}
