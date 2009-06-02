using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SharpPcap;

namespace SharpPcap.Containers
{
    // managed version of pcap_if
    // NOTE: we can't use pcap_if directly because the class contains
    //       a pointer to pcap_if that will be freed when the
    //       device memory is freed, so instead convert the unmanaged structure
    //       to a managed one to avoid this issue
    public class PcapInterface
    {
        public string            Name;         /* name to hand to "pcap_open_live()" */              
        public string            FriendlyName; /* Human readable interface name from System.Net.NetworkInformation.NetworkInterface.Name */
        public string            Description;  /* textual description of interface */
        public List<PcapAddress> Addresses;
        public uint              Flags;        /* PCAP_IF_ interface flags */

        private PcapAddress m_macAddress;
        public System.Net.NetworkInformation.PhysicalAddress MacAddress
        {
            get
            {
                return m_macAddress.Addr.hardwareAddress;
            }

            set
            {
                // do we already have a hardware address for this device?
                if(m_macAddress != null)
                {
#if false
                    Console.WriteLine("Overwriting hardware address "
                                      + m_macAddress.Addr.hardwareAddress.ToString()
                                      + " with " +
                                      value.ToString());
#endif
                    // overwrite the value with the new value
                    m_macAddress.Addr.hardwareAddress = value;
                } else
                {
#if false
                    Console.WriteLine("Creating new PcapAddress entry for this hardware address");
#endif
                    // create a new entry for the mac address
                    PcapAddress newAddress = new PcapAddress();
                    newAddress.Addr = new Sockaddr(value);

                    // add the address to our addresses list
                    Addresses.Add(newAddress);

                    // m_macAddress should point to this hardware address
                    m_macAddress = newAddress;
                }
            }
        }

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

                PcapAddress newAddress = new PcapAddress(addr);
                Addresses.Add(newAddress);

                // is this a hardware address?
                // if so we should set our internal m_macAddress member variable
                if(newAddress.Addr.type == Sockaddr.Type.HARDWARE)
                {
                    if(m_macAddress == null)
                    {
                        m_macAddress = newAddress;
                    } else
                    {
                        throw new System.InvalidOperationException("found multiple hardware addresses, existing addr "
                                                                   + MacAddress.ToString() + ", new address " + newAddress.Addr.hardwareAddress.ToString());
                    }
                }

                address = addr.Next; // move to the next address
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Name: {0}\n", Name);
            if(FriendlyName != null)
            {
                sb.AppendFormat("FriendlyName: {0}\n", FriendlyName);
            }

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
