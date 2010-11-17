using System;
using System.Runtime.InteropServices;

namespace SharpPcap.WinPcap
{
    /// <summary>
    /// Remote authentication type and parameters
    /// </summary>
    public class RemoteAuthentication
    {
        public AuthenticationTypes Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public RemoteAuthentication (AuthenticationTypes Type,
                                     string Username,
                                     string Password)
        {
            this.Type = Type;
            this.Username = Username;
            this.Password = Password;
        }

        /// <summary>
        /// Converts this structure to an unmanaged IntPtr. Should be
        /// freed with Marshal.FreeHGlobal(IntPtr);
        /// </summary>
        /// <returns>
        /// A <see cref="IntPtr"/>
        /// </returns>
        internal IntPtr GetUnmanaged()
        {
            UnmanagedStructures.pcap_rmtauth rmauth;
            rmauth.type = (IntPtr)Type;
            rmauth.username = Username;
            rmauth.password = Password;

            // Initialize unmanged memory to hold the struct.
            IntPtr rmAuthPointer = Marshal.AllocHGlobal(Marshal.SizeOf(rmauth));

            // marshal pcap_rmtauth
            Marshal.StructureToPtr(rmauth, rmAuthPointer, false);

            return rmAuthPointer;
        }
    }
}
