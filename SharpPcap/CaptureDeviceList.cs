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
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace SharpPcap
{
    /// <summary>
    /// List of available capture devices
    /// </summary>
    public class CaptureDeviceList : ReadOnlyCollection<ICaptureDevice>
    {
        private static CaptureDeviceList instance;

        /// <summary>
        /// Method to retrieve this classes singleton instance
        /// </summary>
        public static CaptureDeviceList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CaptureDeviceList();
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
        public static CaptureDeviceList New()
        {
            return new CaptureDeviceList();
        }

        /// <summary>
        /// Represents a strongly typed, read-only list of PcapDevices.
        /// </summary>
        private CaptureDeviceList()
            : base(new List<ICaptureDevice>())
        {
            Refresh();
        }

        /// <summary>
        /// Retrieve a list of the current devices
        /// </summary>
        /// <returns>
        /// A <see cref="List&lt;ICaptureDevice&gt;"/>
        /// </returns>
        private static List<ICaptureDevice> GetDevices()
        {
            List<ICaptureDevice> deviceList = new List<ICaptureDevice>();

            // windows
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) ||
               (Environment.OSVersion.Platform == PlatformID.Win32Windows))
            {
                var dl = WinPcap.WinPcapDeviceList.Instance;
                foreach (var c in dl)
                {
                    deviceList.Add(c);
                }
            }
            else // not windows
            {
                var dl = LibPcap.LibPcapLiveDeviceList.Instance;
                foreach (var c in dl)
                {
                    deviceList.Add(c);
                }
            }

            return deviceList;
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
                            // TODO: copy things from the new item to the existing item

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

                    // add the device we didn't see in the new list
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
        public ICaptureDevice this[string Name]
        {
            get
            {
                // lock to prevent issues with multi-threaded access
                // with other methods
                lock (this)
                {
                    var devices = (List<ICaptureDevice>)base.Items;
                    var dev = devices.Find(delegate(ICaptureDevice i) { return i.Name == Name; });
                    var result = dev ?? devices.Find(delegate(ICaptureDevice i) { return i.Description == Name; });

                    if (result == null)
                        throw new IndexOutOfRangeException();
                    return result;
                }
            }
        }
        #endregion
    }
}
