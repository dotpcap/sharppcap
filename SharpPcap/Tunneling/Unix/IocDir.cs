using System;

namespace SharpPcap.Tunneling.Unix
{
    [Flags]
    internal enum IocDir : uint
    {
        None = 1U,
        Read = 2U,
        Write = 4U,
    }
}
