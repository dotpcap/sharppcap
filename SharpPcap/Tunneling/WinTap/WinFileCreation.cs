using System;

namespace SharpPcap.Tunneling.WinTap
{
    /// <summary>
    /// See https://docs.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew
    /// </summary>
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
