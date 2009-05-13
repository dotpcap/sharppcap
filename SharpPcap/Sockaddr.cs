using System;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

namespace SharpPcap.Containers
{
    public class Sockaddr
    {
        public enum Type
        {
            AF_INET_AF_INET6,
            HARDWARE,
            UNKNOWN
        }
        public Type type;

        public System.Net.IPAddress ipAddress;  // if type == AF_INET_AF_INET6
        public PhysicalAddress hardwareAddress; // if type == HARDWARE

        private int _sa_family;
        public int sa_family
        {
            get { return _sa_family; }
        }

        public Sockaddr(PhysicalAddress hardwareAddress)
        {
            this.type = Type.HARDWARE;
            this.hardwareAddress = hardwareAddress;
        }

        public Sockaddr(IntPtr sockaddrPtr)
        {
            // A sockaddr struct. We use this to determine the address family
            PcapUnmanagedStructures.sockaddr saddr;

            // Marshal memory pointer into a struct
            saddr = (PcapUnmanagedStructures.sockaddr)Marshal.PtrToStructure(sockaddrPtr,
                                                     typeof(PcapUnmanagedStructures.sockaddr));

            // record the sa_family for informational purposes
            _sa_family = saddr.sa_family;

            byte[] addressBytes;
            if(saddr.sa_family == Pcap.AF_INET)
            {
                type = Type.AF_INET_AF_INET6; 
                PcapUnmanagedStructures.sockaddr_in saddr_in = 
                    (PcapUnmanagedStructures.sockaddr_in)Marshal.PtrToStructure(sockaddrPtr,
                                                                                typeof(PcapUnmanagedStructures.sockaddr_in));
                ipAddress = new System.Net.IPAddress(saddr_in.sin_addr.s_addr);
            } else if(saddr.sa_family == Pcap.AF_INET6)
            {
                type = Type.AF_INET_AF_INET6;
                addressBytes = new byte[16];
                PcapUnmanagedStructures.sockaddr_in6 sin6 =
                    (PcapUnmanagedStructures.sockaddr_in6)Marshal.PtrToStructure(sockaddrPtr,
                                                         typeof(PcapUnmanagedStructures.sockaddr_in6));
                Array.Copy(sin6.sin6_addr, addressBytes, addressBytes.Length);
                ipAddress = new System.Net.IPAddress(addressBytes);
            } else if(saddr.sa_family == Pcap.AF_PACKET)
            {
                type = Type.HARDWARE;

                PcapUnmanagedStructures.sockaddr_ll saddr_ll =
                    (PcapUnmanagedStructures.sockaddr_ll)Marshal.PtrToStructure(sockaddrPtr,
                                                      typeof(PcapUnmanagedStructures.sockaddr_ll));

                byte[] hardwareAddressBytes = new byte[saddr_ll.sll_halen];
                for(int x = 0; x < saddr_ll.sll_halen; x++)
                {
                    hardwareAddressBytes[x] = saddr_ll.sll_addr[x];
                }
                hardwareAddress = new PhysicalAddress(hardwareAddressBytes); // copy into the PhysicalAddress class
            } else
            {
                type = Type.UNKNOWN;

                // place the sockaddr.sa_data into the hardware address just in case
                // someone wants access to the bytes
                byte[] hardwareAddressBytes = new byte[saddr.sa_data.Length];
                for(int x = 0; x < saddr.sa_data.Length; x++)
                {
                    hardwareAddressBytes[x] = saddr.sa_data[x];
                }
                hardwareAddress = new PhysicalAddress(hardwareAddressBytes);
            }
        }

        public override string ToString()
        {
            if(type == Type.AF_INET_AF_INET6)
            {
                return ipAddress.ToString();
            } else if(type == Type.HARDWARE)
            {
                return "HW addr: " + hardwareAddress.ToString();
            } else if(type == Type.UNKNOWN)
            {
                return String.Empty;
            }

            return String.Empty;
        }
    }
}
