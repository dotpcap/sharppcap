// Copyright 2005 Tamir Gal <tamir@tamirgal.com>
// Copyright 2009 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// managed version of struct pcap_if
    /// NOTE: we can't use pcap_if directly because the class contains
    ///       a pointer to pcap_if that will be freed when the
    ///       device memory is freed, so instead convert the unmanaged structure
    ///       to a managed one to avoid this issue
    /// </summary>
    public class PcapInterface
    {
        /// <value>
        /// Name of the interface. Used internally when passed to pcap_open_live()
        /// </value>
        public string Name { get; internal set; }

        /// <value>
        /// Human readable interface name derived from System.Net.NetworkInformation.NetworkInterface.Name
        /// </value>
        public string FriendlyName { get; internal set; }

        /// <value>
        /// Text description of the interface as given by pcap/npcap
        /// </value>
        public string Description { get; internal set; }

        /// <value>
        /// Gateway address of this device
        /// NOTE: May only be available on Windows
        /// </value>
        public List<IPAddress> GatewayAddresses { get; internal set; }

        /// <value>
        /// Addresses associated with this device
        /// </value>
        public List<PcapAddress> Addresses { get; internal set; }

        /// <summary>
        /// Credentials to use in case of remote pcap
        /// </summary>
        internal RemoteAuthentication Credentials { get; }

        /// <value>
        /// Pcap interface flags
        /// </value>
        public uint Flags { get; internal set; }

        /// <summary>
        /// MacAddress of the interface
        /// </summary>
        public PhysicalAddress MacAddress { get; }

        internal PcapInterface(pcap_if pcapIf, NetworkInterface networkInterface, RemoteAuthentication credentials)
        {
            Name = pcapIf.Name;
            Description = pcapIf.Description;
            Flags = pcapIf.Flags;
            Addresses = new List<PcapAddress>();
            GatewayAddresses = new List<IPAddress>();
            Credentials = credentials;

            // retrieve addresses
            var address = pcapIf.Addresses;
            while (address != IntPtr.Zero)
            {
                // Marshal memory pointer into a sockaddr struct
                var addr = Marshal.PtrToStructure<pcap_addr>(address);

                PcapAddress newAddress = new PcapAddress(addr);
                Addresses.Add(newAddress);

                // is this a hardware address?
                // if so we should set our MacAddress
                if (newAddress.Addr?.type == Sockaddr.AddressTypes.HARDWARE)
                {
                    if (MacAddress == null)
                    {
                        MacAddress = newAddress.Addr.hardwareAddress;
                    }
                    else if (!MacAddress.Equals(newAddress.Addr.hardwareAddress))
                    {
                        throw new InvalidOperationException("found multiple hardware addresses, existing addr "
                                                                   + MacAddress.ToString() + ", new address " + newAddress.Addr.hardwareAddress.ToString());
                    }
                }

                address = addr.Next; // move to the next address
            }

            // attempt to populate the mac address,
            // friendly name etc of this device
            if (networkInterface != null)
            {
                var ipProperties = networkInterface.GetIPProperties();
                int gatewayAddressCount = ipProperties.GatewayAddresses.Count;
                if (gatewayAddressCount != 0)
                {
                    foreach (GatewayIPAddressInformation gatewayInfo in ipProperties.GatewayAddresses)
                    {
                        GatewayAddresses.Add(gatewayInfo.Address);
                    }
                }
                FriendlyName = networkInterface.Name;

                PhysicalAddress mac = networkInterface.GetPhysicalAddress();
                if (MacAddress == null && mac != null)
                {
                    PcapAddress pcapAddress = new PcapAddress();
                    pcapAddress.Addr = new Sockaddr(mac);
                    Addresses.Add(pcapAddress);
                    if (pcapAddress.Addr.hardwareAddress.GetAddressBytes().Length != 0)
                    {
                        MacAddress = pcapAddress.Addr.hardwareAddress;
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FriendlyName = WindowsNativeMethods.GetInterfaceAlias(Name);
            }
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Name: {0}\n", Name);
            if (FriendlyName != null)
            {
                sb.AppendFormat("FriendlyName: {0}\n", FriendlyName);
            }

            if (GatewayAddresses != null)
            {
                sb.AppendFormat("GatewayAddresses:\n");
                int i = 0;
                foreach (IPAddress gatewayAddr in GatewayAddresses)
                {
                    sb.AppendFormat("{0}) {1}\n", i + 1, gatewayAddr);
                    i++;
                }
            }

            sb.AppendFormat("Description: {0}\n", Description);
            foreach (PcapAddress addr in Addresses)
            {
                sb.AppendFormat("Addresses:\n{0}\n", addr);
            }
            sb.AppendFormat("Flags: {0}\n", Flags);
            return sb.ToString();
        }

        static public IReadOnlyList<PcapInterface> GetAllPcapInterfaces(IPEndPoint source, RemoteAuthentication credentials)
        {
            return GetAllPcapInterfaces("rpcap://" + source, credentials);
        }

        static public IReadOnlyList<PcapInterface> GetAllPcapInterfaces(string source, RemoteAuthentication credentials)
        {
            var devicePtr = IntPtr.Zero;
            var errorBuffer = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE);
            var auth = RemoteAuthentication.CreateAuth(credentials);

            try
            {
                var result = LibPcapSafeNativeMethods.pcap_findalldevs_ex(source, ref auth, ref devicePtr, errorBuffer);
                if (result < 0)
                {
                    throw new PcapException(errorBuffer.ToString());
                }
            }
            catch (TypeLoadException ex)
            {
                throw new PlatformNotSupportedException(
                    "Operation is not supported on this platform.",
                    ex
                );
            }

            var pcapInterfaces = GetAllPcapInterfaces(devicePtr, credentials);

            // Free unmanaged memory allocation
            LibPcapSafeNativeMethods.pcap_freealldevs(devicePtr);

            return pcapInterfaces;
        }

        static public IReadOnlyList<PcapInterface> GetAllPcapInterfaces()
        {
            var devicePtr = IntPtr.Zero;
            var errorBuffer = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE);

            int result = LibPcapSafeNativeMethods.pcap_findalldevs(ref devicePtr, errorBuffer);
            if (result < 0)
            {
                throw new PcapException(errorBuffer.ToString());
            }
            var pcapInterfaces = GetAllPcapInterfaces(devicePtr, null);

            // Free unmanaged memory allocation
            LibPcapSafeNativeMethods.pcap_freealldevs(devicePtr);

            return pcapInterfaces;
        }
        static private IReadOnlyList<PcapInterface> GetAllPcapInterfaces(IntPtr devicePtr, RemoteAuthentication credentials)
        {
            var list = new List<PcapInterface>();
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            var nextDevPtr = devicePtr;
            while (nextDevPtr != IntPtr.Zero)
            {
                // Marshal pointer into a struct
                var pcap_if_unmanaged = Marshal.PtrToStructure<pcap_if>(nextDevPtr);
                NetworkInterface networkInterface = null;
                foreach (var nic in nics)
                {
                    // if the name and id match then we have found the NetworkInterface
                    // that matches the PcapDevice
                    if (pcap_if_unmanaged.Name.EndsWith(nic.Id))
                    {
                        networkInterface = nic;
                    }
                }
                var pcap_if = new PcapInterface(pcap_if_unmanaged, networkInterface, credentials);
                list.Add(pcap_if);
                nextDevPtr = pcap_if_unmanaged.Next;
            }

            return list;
        }

        #region Timestamp
        /// <summary>
        /// Timestamps supported by this device
        /// </summary>
        ///
        /// <remarks>
        /// Note: Live devices can have supported timestamps but offline devices
        /// (such as file readers etc) do not. See https://www.tcpdump.org/manpages/pcap-tstamp.7.html
        /// </remarks>
        public System.Collections.Generic.List<PcapClock> TimestampsSupported
        {
            get
            {
                StringBuilder errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors
                using (var handle = LibPcapSafeNativeMethods.pcap_create(Name, errbuf))
                {

                    IntPtr typePtr = IntPtr.Zero;

                    // Note: typePtr must be freed with pcap_free_tstamp_types()
                    var typeCount = LibPcapSafeNativeMethods.pcap_list_tstamp_types(handle, ref typePtr);

                    var timestampTypes = new System.Collections.Generic.List<PcapClock>();

                    for (var i = 0; i < typeCount; i++)
                    {
                        var value = Marshal.ReadInt32(typePtr, i * sizeof(int));
                        var tsValue = (TimestampType)value;
                        timestampTypes.Add(new PcapClock(tsValue));
                    }

                    // Free unmanaged memory allocation
                    LibPcapSafeNativeMethods.pcap_free_tstamp_types(typePtr);

                    // per https://www.tcpdump.org/manpages/pcap_list_tstamp_types.3pcap.html
                    // PCAP_TSTAMP_HOST is the only supported version
                    if (typeCount == 0)
                    {
                        var tsValue = TimestampType.Host;
                        timestampTypes.Add(new PcapClock(tsValue));
                    }

                    return timestampTypes;
                }
            }
        }
        #endregion
    }
}
