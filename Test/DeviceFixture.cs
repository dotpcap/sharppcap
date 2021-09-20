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
 * Copyright 2020-2021 Ayoub Kaanich <kayoub5@live.com>
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test
{

    public class DeviceFixture
    {
        private readonly LibPcapLiveDevice Device;
        public DeviceFixture(LibPcapLiveDevice device)
        {
            Device = device;
        }

        public LibPcapLiveDevice GetDevice() => Device;

        public override string ToString()
        {
            return Device.Name;
        }

        public static IEnumerable<ILiveDevice> GetDevices()
        {
            var lists = new Dictionary<string, IEnumerable<ILiveDevice>>
            {
                { nameof(CaptureDeviceList), CaptureDeviceList.Instance },
                { nameof(LibPcapLiveDeviceList), LibPcapLiveDeviceList.Instance }
            };
            foreach (var list in lists)
            {
                Assert.IsNotEmpty(list.Value, "{0} should not be empty", list.Key);
            }
            return lists.SelectMany(l => l.Value)
                // The bluetooth-monitor break the tests on circleci
                // With "Return code: -1" during Open
                .Where(d => d.Name != "bluetooth-monitor")
                // Semaphore CI have this interface, and it's always down
                .Where(d => d.Name != "virbr0-nic")
                .Distinct();
        }
    }
    class CaptureDevicesAttribute : NUnitAttribute, IParameterDataSource
    {
        public IEnumerable GetData(IParameterInfo parameter)
        {
            return DeviceFixture.GetDevices()
                .Cast<LibPcapLiveDevice>()
                .Select(d => new DeviceFixture(d))
                .ToArray();
        }
    }

}

