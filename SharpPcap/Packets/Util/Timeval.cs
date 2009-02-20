/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace SharpPcap.Packets.Util
{
    /// <summary> POSIX.4 timeval
    [Serializable]
    public class Timeval
    {
        /// <summary> Convert this timeval to a DateTime</summary>
        virtual public System.DateTime Date
        {
            get
            {
                long ticksAtEpoch = new DateTime(1970, 1, 1).Ticks;
                long microsecondsPerMillisecond = 1000;
                long tickOffsetFromEpoch = (long)(seconds * TimeSpan.TicksPerSecond) +
                                           (((long)microseconds * TimeSpan.TicksPerMillisecond) / microsecondsPerMillisecond);
                return new System.DateTime(ticksAtEpoch + tickOffsetFromEpoch);
            }
        }

        virtual public ulong Seconds
        {
            get
            {
                return seconds;
            }
            
        }

        virtual public ulong MicroSeconds
        {
            get
            {
                return microseconds;
            }
            
        }

        public Timeval(ulong seconds, ulong microseconds)
        {
            this.seconds = seconds;
            this.microseconds = microseconds;
        }
        
        public override System.String ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(seconds);
            sb.Append('.');
            sb.Append(microseconds);
            sb.Append('s');
            
            return sb.ToString();
        }
        
        internal ulong seconds;
        internal ulong microseconds;
    }
}