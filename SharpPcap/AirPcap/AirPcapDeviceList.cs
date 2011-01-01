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

namespace SharpPcap.AirPcap
{
    public class AirPcapDeviceList
    {
        public static List<AirPcapDevice> GetDevices()
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
                deviceList.Add(new AirPcapDevice(managedDeviceDesc));
                nextDevPtr = deviceDescUnmanaged.next;
            }
            AirPcapSafeNativeMethods.AirpcapFreeDeviceList(devicePtr); // Free unmanaged memory

            return deviceList;
        }
    }
}
