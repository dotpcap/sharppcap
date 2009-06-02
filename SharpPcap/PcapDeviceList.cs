using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using SharpPcap.Containers;

namespace SharpPcap
{
    /// <summary>
    /// List of available Pcap Interfaces.
    /// </summary>
    public class PcapDeviceList : ReadOnlyCollection<PcapDevice> {

        /// <summary>
        /// Represents a strongly typed, read-only list of PcapDevices.
        /// </summary>
        public PcapDeviceList() : base(new List<PcapDevice>())
        {
            IntPtr devicePtr = IntPtr.Zero;
            StringBuilder errorBuffer = new StringBuilder(256);

            int result = SafeNativeMethods.pcap_findalldevs(ref devicePtr, errorBuffer);
            if (result < 0)
                throw new PcapException(errorBuffer.ToString());

            IntPtr nextDevPtr = devicePtr;

            while (nextDevPtr != IntPtr.Zero)
            {
                // Marshal pointer into a struct
                PcapUnmanagedStructures.pcap_if pcap_if_unmanaged =
                    (PcapUnmanagedStructures.pcap_if)Marshal.PtrToStructure(nextDevPtr,
                                                    typeof(PcapUnmanagedStructures.pcap_if));
                PcapInterface pcap_if = new PcapInterface(pcap_if_unmanaged);
                base.Items.Add(new PcapDevice(pcap_if));
                nextDevPtr = pcap_if_unmanaged.Next;
            }
            SafeNativeMethods.pcap_freealldevs(devicePtr);  // Free unmanaged memory allocation.

            // go through the network interfaces to populate the mac address
            // for each of the devices
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach(PcapDevice device in Items)
            {
//                Console.WriteLine("device.Name is {0}", device.Name);

                foreach(NetworkInterface adapter in nics)
                {
//                    Console.WriteLine("adapter.Id of {0}", adapter.Id);
//                    Console.WriteLine("adapter.GetPhysicalAddress() {0}", adapter.GetPhysicalAddress().ToString());

                    // if the name and id match then we have found the NetworkInterface
                    // that matches the PcapDevice
                    if(device.Name.EndsWith(adapter.Id))
                    {
                        device.Interface.MacAddress = adapter.GetPhysicalAddress();
                        device.Interface.FriendlyName = adapter.Name;
                    }
                }
            }
        }

        #region PcapDevice Indexers
        /// <param name="Name">The name or description of the pcap interface to get.</param>
        public PcapDevice this[string Name]
        {
            get
            {
                List<PcapDevice> devices = (List<PcapDevice>)base.Items;
                PcapDevice dev = devices.Find(delegate(PcapDevice i) { return i.Name == Name; });
                PcapDevice result = dev ?? devices.Find(delegate(PcapDevice i) { return i.Description == Name; });

                if (result == null)
                    throw new IndexOutOfRangeException();
                return result;
            }
        }
        #endregion
    }
}
