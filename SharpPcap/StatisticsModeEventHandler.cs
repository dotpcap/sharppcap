using System;

namespace SharpPcap
{
        /// <summary>
        /// A delegate for delivering network statistics when using winpcap in
        /// statistics mode
        /// </summary>
        public delegate void StatisticsModeEventHandler(object sender, PcapStatisticsModeEventArgs e);
}
