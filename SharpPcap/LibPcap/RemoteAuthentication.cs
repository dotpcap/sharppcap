using System;
using System.Net;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
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
    }
}
