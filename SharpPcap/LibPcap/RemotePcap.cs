using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap.LibPcap
{
    internal static class RemotePcap
    {
        internal static pcap_rmtauth CreateAuth(string source, ICredentials credentials)
        {
            Uri.TryCreate(source, UriKind.Absolute, out var uri);
            var credential = credentials?.GetCredential(uri, null);
            if (credential == null)
            {
                return default;
            }
            int auth_type;
            switch (credential.Domain)
            {
                case "":
                case null:
                    // auto detect the type from presense of username
                    auth_type = string.IsNullOrEmpty(credential.UserName) ? 0 : 1;
                    break;
                case "null":
                    auth_type = 0;
                    break;
                case "pwd":
                    auth_type = 1;
                    break;
                default:
                    // if someone wants to force the auth type to something else, they can write the credential domain as an int
                    int.TryParse(credential.Domain, out auth_type);
                    break;
            }
            return new pcap_rmtauth
            {
                type = new IntPtr(auth_type),
                username = credential.UserName,
                password = credential.Password,
            };
        }
    }
}
