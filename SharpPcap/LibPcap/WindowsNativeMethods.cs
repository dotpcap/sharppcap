using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpPcap.LibPcap
{
    [SuppressUnmanagedCodeSecurity]
    static class WindowsNativeMethods
    {

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/netioapi/nf-netioapi-convertinterfaceguidtoluid
        /// </summary>
        /// <param name="InterfaceGuid"></param>
        /// <param name="InterfaceLuid"></param>
        /// <returns></returns>
        [DllImport("Iphlpapi.dll")]
        private static extern uint ConvertInterfaceGuidToLuid(ref Guid InterfaceGuid, out ulong InterfaceLuid);


        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/netioapi/nf-netioapi-convertinterfaceluidtoalias
        /// </summary>
        /// <param name="InterfaceLuid"></param>
        /// <param name="InterfaceAlias"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        [DllImport("Iphlpapi.dll")]
        private static extern uint ConvertInterfaceLuidToAlias(
            ref ulong InterfaceLuid,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder InterfaceAlias,
            IntPtr Length
        );

        private const int NDIS_IF_MAX_STRING_SIZE = 255;

        /// <summary>
        /// See https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions
        /// </summary>
        private static readonly Version WindowsVistaVersion = new Version(6, 0);

        /// <summary>
        /// Fix for https://github.com/chmorgan/sharppcap/issues/57
        /// </summary>
        /// <param name="pcapName"></param>
        /// <returns></returns>
        internal static string GetInterfaceAlias(string pcapName)
        {
            if (Environment.OSVersion.Version < WindowsVistaVersion)
            {
                // Windows Vista or later required
                return null;
            }
            var guidIndex = pcapName.LastIndexOf('{');
            if (guidIndex < 0 || !Guid.TryParse(pcapName.Substring(guidIndex), out var guid))
            {
                return null;
            }
            uint retval;
            retval = ConvertInterfaceGuidToLuid(ref guid, out var luid);
            if (retval != 0)
            {
                return null;
            }

            var alias = new StringBuilder(NDIS_IF_MAX_STRING_SIZE + 1);
            retval = ConvertInterfaceLuidToAlias(ref luid, alias, (IntPtr)alias.Capacity);
            if (retval != 0)
            {
                return null;
            }
            return alias.ToString();
        }
    }
}
