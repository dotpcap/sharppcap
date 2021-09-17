using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpPcap.WinpkFilter
{
    public class WinpkFilterDriver : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WinpkFilterDriver" /> class.
        /// </summary>
        /// <param name="handle">The filter driver handle.</param>
        /// <param name="driverNameBytes">The driver name bytes.</param>
        protected WinpkFilterDriver(DriverHandle handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// Gets the handle to the filter driver.
        /// </summary>
        public DriverHandle Handle { get; protected set; }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            Handle?.Dispose();
        }

        /// <summary>
        /// Opens the filter driver.
        /// </summary>
        /// <param name="driverName">The name of the driver.</param>
        /// <returns><see cref="WinpkFilterDriver" />.</returns>
        /// <exception cref="Exception">Missing NDIS DLL</exception>
        public static WinpkFilterDriver Open(string driverName = "NDISRD")
        {
            var driverNameBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(driverName);
            var handle = NativeMethods.OpenFilterDriver(driverNameBytes);
            return new WinpkFilterDriver(handle);
        }

        /// <summary>
        /// Gets the native version of the filter driver.
        /// </summary>
        /// <returns>System.UInt32.</returns>
        public uint Version
        {
            get => NativeMethods.GetDriverVersion(Handle);
        }

        /// <summary>
        /// Gets the network adapters.
        /// </summary>
        /// <returns>The <see cref="WinpkFilterDevice" />s.</returns>
        public IEnumerable<WinpkFilterDevice> GetNetworkDevices()
        {
            var adapterList = new TcpAdapterList();
            NativeMethods.GetTcpipBoundAdaptersInfo(Handle, ref adapterList);

            for (var i = 0; i < adapterList.AdapterCount; i++)
            {
                var name = adapterList.AdapterNames.Skip(i * NativeMethods.ADAPTER_NAME_SIZE).Take(NativeMethods.ADAPTER_NAME_SIZE).ToArray();
                var address = adapterList.CurrentAddresses.Skip(i * NativeMethods.ETHER_ADDR_LENGTH).Take(NativeMethods.ETHER_ADDR_LENGTH).ToArray();
                yield return new WinpkFilterDevice(
                    Handle,
                    adapterList.AdapterHandles[i],
                    name,
                    adapterList.AdapterMediums[i],
                    address,
                    adapterList.MTUs[i]
                );
            }
        }

    }
}