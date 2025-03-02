// Copyright 2020-2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

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
                Assert.That(list.Value, Is.Not.Empty, $"{list.Key} should not be empty");
            }
            return lists.SelectMany(l => l.Value)
                // The bluetooth-monitor break the tests on circleci
                // With "Return code: -1" during Open
                .Where(d => d.Name != "bluetooth-monitor")
                // Semaphore CI have this interface, and it's always down
                .Where(d => d.Name != "virbr0-nic")
                // TAP interfaces, usually down until being used
                .Where(d => !d.Name.StartsWith("tap"))
                // From AppVeyor
                .Where(d => !d.Name.StartsWith("dbus"))
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

