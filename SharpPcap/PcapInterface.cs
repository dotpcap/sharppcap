using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SharpPcap.Containers;

namespace SharpPcap
{
    // managed version of pcap_if
    // NOTE: we can't use pcap_if directly because the class contains
    //       a pointer to pcap_if that will be freed when the
    //       device memory is freed, so instead convert the unmanaged structure
    //       to a managed one to avoid this issue
    public class PcapInterface
    {
        public string            Name;        /* name to hand to "pcap_open_live()" */              
        public string            Description; /* textual description of interface */
        public List<PcapAddress> Addresses;
        public uint              Flags;       /* PCAP_IF_ interface flags */

        internal PcapInterface(PcapUnmanagedStructures.pcap_if pcapIf)
        {
            Name = pcapIf.Name;
            Description = pcapIf.Description;
            Flags = pcapIf.Flags;

            // retrieve addresses
            Addresses = new List<PcapAddress>();
            IntPtr address = pcapIf.Addresses;
            while(address != IntPtr.Zero)
            {
                //A sockaddr struct
                PcapUnmanagedStructures.pcap_addr addr;

                //Marshal memory pointer into a struct
                addr = (PcapUnmanagedStructures.pcap_addr)Marshal.PtrToStructure(address,
                                                                                 typeof(PcapUnmanagedStructures.pcap_addr));

                Addresses.Add(new PcapAddress(addr));

                address = addr.Next; // move to the next address
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Name: {0}\n", Name);
            sb.AppendFormat("Description: {0}\n", Description);
            foreach(PcapAddress addr in Addresses)
            {
                sb.AppendFormat("Addresses:\n{0}\n", addr);
            }
            sb.AppendFormat("Flags: {0}\n", Flags);
            return sb.ToString();
        }
    }
}
