using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap.LibPcap
{
    internal static class RemotePcap
    {
        internal static pcap_rmtauth CreateAuth(string source, RemoteAuthentication credentials)
        {
            if (credentials == null)
            {
                return default;
            }

            int auth_type;

            switch(credentials.Type)
            {
                case AuthenticationTypes.Null:
                    auth_type = 0;
                    break;
                case AuthenticationTypes.Password:
                    auth_type = 1;
                    break;
                default:
                    throw new NotSupportedException("unknown credentials.Type");
            }

            return new pcap_rmtauth
            {
                type = new IntPtr(auth_type),
                username = credentials.Username,
                password = credentials.Password,
            };
        }
    }
}
