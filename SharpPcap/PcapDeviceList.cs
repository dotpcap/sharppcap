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
