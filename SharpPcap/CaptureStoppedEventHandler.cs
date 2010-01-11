using System;

namespace SharpPcap
{
        /// <summary>
        /// A delegate for notifying of a capture stopped event
        /// </summary>
        public delegate void CaptureStoppedEventHandler(object sender, bool error);
}
