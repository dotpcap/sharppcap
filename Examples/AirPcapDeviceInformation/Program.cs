using System;
using System.Collections.Generic;
using SharpPcap.AirPcap;

namespace AirPcapDeviceInformation
{
    class Program
    {
        static void Main(string[] args)
        {
            var devices = AirPcapDeviceList.Instance;

            if (devices.Count == 0)
            {
                Console.WriteLine("No devices found, are you running as admin(if in Windows), or root(if in Linux/Mac)?");
                return;
            }

            Console.WriteLine("Available AirPcap devices:");
            for (var i = 0; i < devices.Count; i++)
            {
                Console.WriteLine("[{0}] - {1}", i, devices[i].Name);
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to retrieve the information for: ");
            var devIndex = int.Parse(Console.ReadLine());

            var device = (AirPcapDevice)devices[devIndex];
            device.Open();

            Console.WriteLine("Capabilities: {0}", device.Capabilities);
            Console.WriteLine("Channel: {0}", device.Channel);
            Console.WriteLine("ChannelInfo: {0}", device.ChannelInfo);

            // set some device keys to ensure that we can retrieve them
            var keyBytes = new byte[AirPcapKey.WepKeyMaxSize];
            for(int x = 0; x < keyBytes.Length; x++)
                keyBytes[x] = (byte)(x * 2);
            var keys = new List<AirPcapKey>();
            keys.Add(new AirPcapKey(AirPcapKeyType.Wep, keyBytes));
            device.DeviceKeys = keys;

            // display the device keys
            Console.WriteLine("DeviceKeys:");
            var deviceKeys = device.DeviceKeys;
            if (deviceKeys != null)
            {
                foreach (var k in deviceKeys)
                {
                    Console.WriteLine(k);
                }
            }
            else
            {
                Console.WriteLine("no device keys present");
            }

            // set some driver keys
            // set some device keys to ensure that we can retrieve them
            var driverKeyBytes = new byte[AirPcapKey.WepKeyMaxSize];
            for (int x = 0; x < keyBytes.Length; x++)
                driverKeyBytes[x] = (byte)(x * 3);
            var driverKeys = new List<AirPcapKey>();
            driverKeys.Add(new AirPcapKey(AirPcapKeyType.Wep, driverKeyBytes));
            device.DriverKeys = keys;

            // display the driver keys
            Console.WriteLine("DriverKeys:");
            var actualDriverKeys = device.DriverKeys;
            if (actualDriverKeys != null)
            {
                foreach (var k in actualDriverKeys)
                {
                    Console.WriteLine(k);
                }
            }
            else
            {
                Console.WriteLine("no driver keys present");
            }

            Console.WriteLine("DecryptionState: {0}", device.DecryptionState);
            Console.WriteLine("DriverDecryptionState: {0}", device.DriverDecryptionState);
            Console.WriteLine("FcsPresence: {0}", device.FcsPresence);
            Console.WriteLine("FcsValidation: {0}", device.FcsValidation);
            Console.WriteLine("KernelBufferSize: {0}", device.KernelBufferSize);
            Console.WriteLine("LedCount: {0}", device.LedCount);
            Console.WriteLine("LinkType: {0}", device.LinkType);
            Console.WriteLine("MacAddress: {0}", device.MacAddress);
            Console.WriteLine("MacFlags: {0}", device.MacFlags);

            Console.WriteLine("SupportedChannels:");
            var supportedChannels = device.SupportedChannels;
            foreach (var s in supportedChannels)
            {
                Console.WriteLine(s.ToString());
            }

            try
            {
                Console.WriteLine("TxPower: {0}", device.TxPower);
            }
            catch(System.NotSupportedException)
            {
                Console.WriteLine("TxPower: Unsupported by adapter");
            }

            try
            {
                Console.WriteLine("Timestamp: {0}", device.Timestamp);
            }
            catch (System.NotSupportedException)
            {
                Console.WriteLine("Timestamp: Unsupported by adapter");
            }

            device.Close();
        }
    }
}
