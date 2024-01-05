// Copyright 2021 Chris Morgan <chmorgan@gmail.com>
// Copyright 2021 Ayoub Kaanich <kayoub5@live.com>
// SPDX-License-Identifier: MIT

using PacketDotNet;
using SharpPcap.LibPcap;
using System;

namespace SharpPcap
{
    public static class CaptureDeviceExtensions
    {
        /// <summary>
        /// Defined as extension method for easier migration, since this is the most used form of Open in SharpPcap 5.x
        /// </summary>
        /// <param name="device"></param>
        /// <param name="mode"></param>
        /// <param name="read_timeout"></param>
        public static void Open(this IPcapDevice device, DeviceModes mode = DeviceModes.None, int read_timeout = 1000)
        {
            var configuration = new DeviceConfiguration()
            {
                Mode = mode,
                ReadTimeout = read_timeout,
            };
            device.Open(configuration);
        }

        public static void Open(this CaptureFileWriterDevice device, ICaptureDevice captureDevice)
        {
            var configuration = new DeviceConfiguration()
            {
                LinkLayerType = captureDevice.LinkType,
            };
            device.Open(configuration);
        }

        public static void Open(this CaptureFileWriterDevice device, LinkLayers linkLayerType = LinkLayers.Ethernet)
        {
            var configuration = new DeviceConfiguration()
            {
                LinkLayerType = linkLayerType,
            };
            device.Open(configuration);
        }

        /// <summary>
        /// Sends a raw packet through this device
        /// </summary>
        /// <param name="p">The packet bytes to send</param>
        /// <param name="size">The number of bytes to send</param>
        public static void SendPacket(this IInjectionDevice device, byte[] p, int size)
        {
            device.SendPacket(new ReadOnlySpan<byte>(p, 0, size));
        }

        /// <summary>
        /// Sends a raw packet through this device
        /// </summary>
        /// <param name="p">The packet to send</param>
        public static void SendPacket(this IInjectionDevice device, Packet p)
        {
            device.SendPacket(p.Bytes);
        }

        /// <summary>
        /// Sends a raw packet through this device
        /// </summary>
        /// <param name="p">The packet to send</param>
        /// <param name="size">The number of bytes to send</param>
        public static void SendPacket(this IInjectionDevice device, Packet p, int size)
        {
            device.SendPacket(p.Bytes, size);
        }

        /// <summary>
        /// Send a raw packet through this device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="p"></param>
        /// <param name="header"></param>
        public static void SendPacket(this IInjectionDevice device, RawCapture p, ICaptureHeader header = null)
        {
            device.SendPacket(new ReadOnlySpan<byte>(p.Data), header);
        }
    }
}
