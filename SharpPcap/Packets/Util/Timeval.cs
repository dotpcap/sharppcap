/*
This file is part of SharpPcap

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
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
namespace SharpPcap.Packets.Util
{
    /// <summary> POSIX.4 timeval</summary>
    public class Timeval
    {
        private static readonly System.DateTime epochDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        internal long microsecondsPerMillisecond = 1000;

        virtual public ulong Seconds
        {
            get;
            set;
        }

        virtual public ulong MicroSeconds
        {
            get;
            set;
        }

        /// <summary> The timeval as a DateTime in Utc </summary>
        virtual public System.DateTime Date
        {
            get
            {
                return UnixTimeValToDateTime(Seconds, MicroSeconds);
            }
        }

        private static void DateTimeToUnixTimeVal(DateTime dateTime,
                                                  out UInt64 tvSec,
                                                  out UInt64 tvUsec)
        {
            // diff this with the dateTime value
            // NOTE: make sure the time is in universal time when performing
            //       the subtraction so we get the difference between epoch in utc
            //       which is the definition of the unix timeval
            TimeSpan timeSpan = dateTime.ToUniversalTime().Subtract(epochDateTime);

            tvSec = (UInt64)(timeSpan.TotalMilliseconds / 1000.0);
            // find the milliseconds remainder and convert to microseconds
            tvUsec = (UInt64)((timeSpan.TotalMilliseconds - (tvSec * 1000)) * 1000);
        }

        private static DateTime UnixTimeValToDateTime(UInt64 tvSec, UInt64 tvUsec)
        {
            // add the tvSec value
            DateTime dt = epochDateTime.AddSeconds(tvSec);
            dt = dt.AddMilliseconds(tvUsec / 1000); // convert microseconds to milliseconds

            return dt;
        }

        public Timeval(ulong Seconds, ulong MicroSeconds)
        {
            this.Seconds = Seconds;
            this.MicroSeconds = MicroSeconds;
        }

        public Timeval()
        {
            ulong seconds;
            ulong microseconds;

            DateTimeToUnixTimeVal(DateTime.UtcNow,
                                  out seconds,
                                  out microseconds);

            this.Seconds = seconds;
            this.MicroSeconds = microseconds;
        }

        public override System.String ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(Seconds);
            sb.Append('.');
            sb.Append(MicroSeconds);
            sb.Append('s');
            
            return sb.ToString();
        }
    }
}