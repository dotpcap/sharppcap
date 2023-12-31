// Copyright 2022 Hendrik Eckardt
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Read a pcap capture from a file handle (e.g., a pipe).
    ///
    /// NOTE: libpcap will take ownership of the handle. The handle will be closed when this device is closed.
    /// On non-Windows systems, the handle passed to this class MUST be opened by <c>fopen</c> or similar functions
    /// that return a <c>FILE*</c> (e.g., via Mono.Posix.NETStandard).
    /// </summary>
    public class CaptureHandleReaderDevice : CaptureReaderDevice
    {
        /// <value>
        /// File handle the object was created with.
        /// </value>
        public SafeHandle FileHandle { get; }

        /// <value>
        /// The name of the capture file.
        /// </value>
        public override string Name => "<file handle>";

        /// <value>
        /// Description of the device.
        /// </value>
        public override string Description => "Capture handle reader device";

        /// <summary>
        /// Creates a new <c>CaptureHandleReaderDevice</c>.
        /// </summary>
        /// <param name="handle">
        /// On Windows, a native Windows handle. On other systems, a pointer to a C runtime <c>FILE</c> object.
        /// </param>
        public CaptureHandleReaderDevice(SafeHandle handle)
        {
            FileHandle = handle;
        }

        /// <summary>
        /// Opens the device.
        /// </summary>
        public override void Open(DeviceConfiguration configuration)
        {
            // holds errors
            StringBuilder errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE);

            var resolution = configuration.TimestampResolution ?? TimestampResolution.Microsecond;
            PcapHandle adapterHandle;
            try
            {
                adapterHandle = LibPcapSafeNativeMethods.pcap_open_handle_offline_with_tstamp_precision(
                    FileHandle, (uint)resolution, errbuf);
            }
            catch (TypeLoadException ex)
            {
                throw new NotSupportedException("libpcap 1.5.0 or higher is required for opening captures by handle", ex);
            }

            // handle error
            if (adapterHandle.IsInvalid)
            {
                string err = "Unable to open offline adapter: " + errbuf;
                throw new PcapException(err);
            }

            // set the device handle
            Handle = adapterHandle;

            base.Open(configuration);
        }
    }
}

