// Copyright 2011 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

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
