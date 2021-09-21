using System;
using System.Runtime.InteropServices;

namespace SharpPcap.WinpkFilter
{
    /// <summary>
    /// Used for setting adapter mode.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct AdapterMode
    {
        internal IntPtr AdapterHandle;
        internal AdapterModes Flags;
    }

}