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

namespace SharpPcap
{
    /// <summary>
    /// List of available capture devices
    /// </summary>
    public class CaptureDeviceList : ReadOnlyCollection<ILiveDevice>
    {
        private static CaptureDeviceList instance;

        private LibPcap.LibPcapLiveDeviceList libPcapDeviceList;

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
            var newCaptureDevice = new CaptureDeviceList();

            newCaptureDevice.libPcapDeviceList = LibPcap.LibPcapLiveDeviceList.New();

            // refresh the device list to flush the original devices and pull the
            // new ones into the newCaptureDevice
            newCaptureDevice.Refresh();

            return newCaptureDevice;
        }

        /// <summary>
        /// Represents a strongly typed, read-only list of PcapDevices.
        /// </summary>
        private CaptureDeviceList()
            : base(new List<ILiveDevice>())
        {
            libPcapDeviceList = LibPcap.LibPcapLiveDeviceList.Instance;
            Refresh();
        }

        /// <summary>
        /// Refresh the device list
        /// </summary>
        public void Refresh()
        {
            lock (this)
            {
                // clear out any items we might have
                base.Items.Clear();
                libPcapDeviceList.Refresh();

                foreach (var i in libPcapDeviceList)
                {
                    base.Items.Add(i);
                }
            }
        }

        #region Device Indexers
        /// <param name="Name">The name or description of the pcap interface to get.</param>
        public ILiveDevice this[string Name]
        {
            get
            {
                // lock to prevent issues with multi-threaded access
                // with other methods
                lock (this)
                {
                    return libPcapDeviceList[Name];
                }
            }
        }

        #endregion
    }
}
