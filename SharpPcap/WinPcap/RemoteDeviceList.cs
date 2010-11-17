using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;

namespace SharpPcap.WinPcap
{
    /// <summary>
    /// Remote adapter list
    /// </summary>
    public class RemoteDeviceList
    {
        List<string> DeviceNames(IPAddress address,
                                 RemoteAuthentication remoteAuthentication)
        {
            return DeviceNames(address, null, remoteAuthentication);
        }

        List<string> DeviceNames(IPAddress address,
                                 int? port,
                                 RemoteAuthentication remoteAuthentication)
        {
            var retval = new List<string>();

            var devicePtr = IntPtr.Zero;
            var errorBuffer = new StringBuilder( Pcap.PCAP_ERRBUF_SIZE ); //will hold errors

            // build the remote string
            string rmStr;
            if(port.HasValue)
            {
                rmStr = string.Format("rpcap://{0}:{1}",
                                      address,
                                      port.Value);
            } else
            {
                rmStr = string.Format("rpcap://{0}",
                                      address);
            }

            var rmAuthPointer = remoteAuthentication.GetUnmanaged();

            int result = SafeNativeMethods.pcap_findalldevs_ex(rmStr,
                                                               rmAuthPointer,
                                                               ref devicePtr,
                                                               errorBuffer);
            Marshal.FreeHGlobal(rmAuthPointer);

            if (result < 0)
                throw new PcapException(errorBuffer.ToString());

            IntPtr nextDevPtr = devicePtr;

            while (nextDevPtr != IntPtr.Zero)
            {
                // Marshal pointer into a struct
                var pcap_if_unmanaged =
                    (PcapUnmanagedStructures.pcap_if)Marshal.PtrToStructure(nextDevPtr,
                                                    typeof(PcapUnmanagedStructures.pcap_if));
                PcapInterface pcap_if = new PcapInterface(pcap_if_unmanaged);
                retval.Add(pcap_if.Name);
                nextDevPtr = pcap_if_unmanaged.Next;
            }

            SharpPcap.SafeNativeMethods.pcap_freealldevs(devicePtr);  // Free unmanaged memory allocation.

            return retval;
        }
    }
}

