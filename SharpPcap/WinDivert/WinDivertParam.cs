using System;

namespace SharpPcap.WinDivert
{
    /// <summary>
    /// Generic configuration params for an opened WinDivert handle.
    /// </summary>
    [Flags]
    public enum WinDivertParam : uint
    {
        QueueLen = 0,
        QueueTime = 1,
        QueueSize = 2,
    }
}