/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/* 
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 */
namespace SharpPcap.Packets
{
    /// <summary> Code constants for ip ports. </summary>
    public enum IPPorts : ushort
    {
        Echo = 7,
        DayTime = 13,
        FtpData = 20,
        Ftp = 21,
        Ssh = 22,
        Telnet = 23,
        Smtp = 25,
        Time = 37,
        Whois = 63,
        Tftp = 69,
        Gopher = 70,
        Finger = 79,
        Http = 80,
        Www = 80,
        Kerberos = 88,
        Pop3 = 110,
        Ident = 113,
        Auth = 113,
        Sftp = 115,
        Ntp = 123,
        Imap = 143,
        Snmp = 161,
        PrivilegedPortLimit = 1024
    }
}
