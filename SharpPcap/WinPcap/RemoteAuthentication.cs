using System;

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
    }
}
