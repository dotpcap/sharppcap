using System;
using PacketDotNet;
using SharpPcap;
using SharpPcap.WinpkFilter;

namespace WinpkFilterExample
{
    /// <summary>
    /// Example showing packet manipulation
    /// </summary>
    public class Program
    {
        static void Main()
        {
            var api = WinpkFilterDriver.Open();
            if (api.Handle.IsInvalid)
            {
                throw new ApplicationException("Cannot load driver.");
            }
            foreach (var device in api.GetNetworkDevices())
            {
                PassThruThread(device);
            }
            Console.ReadLine();
        }

        private static void PassThruThread(WinpkFilterDevice device)
        {
            if (!device.IsValid)
            {
                Console.WriteLine($"Skipped {device.FriendlyName}.");
                return;
            }
            try
            {
                device.OnPacketArrival += Device_OnPacketArrival;
                device.AdapterMode = AdapterModes.Tunnel
                    | AdapterModes.LoopbackFilter
                    | AdapterModes.LoopbackBlock;

                device.StartCapture();
                Console.WriteLine($"Added {device.FriendlyName}.");
            }
            catch (PcapException ex)
            {
                Console.WriteLine($"Failed {device.FriendlyName}: " + ex.Message);
            }
        }

        private static void Device_OnPacketArrival(object sender, PacketCapture e)
        {
            var device = (WinpkFilterDevice)sender;
            var packet = e.GetPacket().GetPacket();
            if (packet.PayloadPacket is IPPacket ip)
            {
                Console.WriteLine(ip.ToString(StringOutputType.Colored));
            }
            device.SendPacket(e.Data, e.Header);
        }
    }
}
