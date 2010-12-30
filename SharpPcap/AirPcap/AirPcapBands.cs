using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPcap.AirPcap
{
    [Flags]
    public enum AirPcapBands : int
    {
        /// <summary>2.4 GHz band</summary>
        _2GHZ = 1,

        /// <summary>5 GHz band</summary>
        _5GHZ = 2,
    };
}
