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
        public static int RpcapdDefaultPort = 2002;

        public static List<WinPcapDevice> Devices(IPAddress address,
                                                  int port,
                                                  RemoteAuthentication remoteAuthentication)
        {
            var retval = new List<WinPcapDevice>();

            var devicePtr = IntPtr.Zero;
            var errorBuffer = new StringBuilder( Pcap.PCAP_ERRBUF_SIZE ); //will hold errors

            // build the remote string
            var rmStr = string.Format("rpcap://{0}:{1}",
                                      address,
                                      port);

            // convert the remote authentication structure to unmanaged memory if
            // one was specified
            IntPtr rmAuthPointer;
            if (remoteAuthentication == null)
                rmAuthPointer = IntPtr.Zero;
            else
                rmAuthPointer = remoteAuthentication.GetUnmanaged();

            int result = SafeNativeMethods.pcap_findalldevs_ex(rmStr,
                                                               rmAuthPointer,
                                                               ref devicePtr,
                                                               errorBuffer);
            // free the memory if any was allocated
            if(rmAuthPointer != IntPtr.Zero)
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
                retval.Add(new WinPcapDevice(pcap_if));
                nextDevPtr = pcap_if_unmanaged.Next;
            }

            SharpPcap.SafeNativeMethods.pcap_freealldevs(devicePtr);  // Free unmanaged memory allocation.

            return retval;
        }
    }
}

