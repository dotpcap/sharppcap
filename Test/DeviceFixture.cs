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
 * Copyright 2020 Ayoub Kaanich <kayoub5@live.com>
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Npcap;
using SharpPcap.WinPcap;

namespace Test
{

    public class DeviceFixture
    {
        private readonly ICaptureDevice Device;
        public DeviceFixture(ICaptureDevice device)
        {
            Device = device;
        }

        public ICaptureDevice GetDevice() => Device;

        public override string ToString()
        {
            return Device.Name;
        }

        public static IEnumerable<ICaptureDevice> GetDevices()
        {
            var lists = new Dictionary<string, IEnumerable<ICaptureDevice>>();
            lists.Add(nameof(CaptureDeviceList), CaptureDeviceList.Instance);
            lists.Add(nameof(LibPcapLiveDeviceList), LibPcapLiveDeviceList.Instance);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                lists.Add(nameof(WinPcapDeviceList), WinPcapDeviceList.Instance);
                lists.Add(nameof(NpcapDeviceList), NpcapDeviceList.Instance);
            }
            foreach (var list in lists)
            {
                Assert.IsNotEmpty(list.Value, "{0} should not be empty", list.Key);
            }
            return lists.SelectMany(l => l.Value).Distinct();
        }
    }
    class CaptureDevicesAttribute : NUnitAttribute, IParameterDataSource
    {
        public IEnumerable GetData(IParameterInfo parameter)
        {
            return DeviceFixture.GetDevices()
                .Select(d => new DeviceFixture(d))
                .ToArray();
        }
    }

    class PcapDevicesAttribute : NUnitAttribute, IParameterDataSource
    {
        public IEnumerable GetData(IParameterInfo parameter)
        {
            return DeviceFixture.GetDevices()
                .OfType<PcapDevice>()
                .Select(d => new DeviceFixture(d))
                .ToArray();
        }
    }

}

