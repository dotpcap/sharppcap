using System;

namespace SharpPcap.WinTap
{
    [Flags]
    enum WinFileCreation : uint
    {
        CreateNew = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting = 5,
    }
}
