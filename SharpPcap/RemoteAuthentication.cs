using System;
using System.Net;
using System.Runtime.InteropServices;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap
{
    /// <summary>
    /// Remote authentication type and parameters
    /// </summary>
    public class RemoteAuthentication
    {
        /// <summary>
        /// Type of authentication
        /// </summary>
        public AuthenticationTypes Type { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Type">
        /// A <see cref="AuthenticationTypes"/>
        /// </param>
        /// <param name="Username">
        /// A <see cref="string"/>
        /// </param>
        /// <param name="Password">
        /// A <see cref="string"/>
        /// </param>
        public RemoteAuthentication(AuthenticationTypes Type,
                                     string Username,
                                     string Password)
        {
            this.Type = Type;
            this.Username = Username;
            this.Password = Password;
        }

        internal static pcap_rmtauth CreateAuth(RemoteAuthentication credentials)
        {
            if (credentials == null)
            {
                return default;
            }

            var auth_type = (int)credentials.Type;

            return new pcap_rmtauth
            {
                type = new IntPtr(auth_type),
                username = credentials.Username,
                password = credentials.Password,
            };
        }
    }
}
