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
 * Copyright 2009-2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;
namespace SharpPcap
{
    /// <summary> POSIX.4 timeval</summary>
    public class PosixTimeval : IComparable<PosixTimeval>
    {
        private static readonly DateTime EpochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <value>
        /// Number of seconds in the timeval
        /// </value>
        public ulong Seconds
        {
            get => (ulong)Value;
            set => Value = (Value % 1) + value;
        }

        /// <value>
        /// Number of microseconds in the timeval
        /// </value>
        public ulong MicroSeconds
        {
            get => (ulong)((Value % 1) * 1e6M);
            set => Value = Seconds + (value * 1e-6M);
        }

        /// <summary>
        /// Seconds since epoch
        /// </summary>
        public decimal Value { get; set; }

        /// <summary> The timeval as a DateTime in Utc </summary>
        public DateTime Date
        {
            get => EpochDateTime.AddTicks((long)(Value * TimeSpan.TicksPerSecond));
        }

        /// <summary>
        /// Operator &lt; overload
        /// </summary>
        /// <param name="a">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="b">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public static bool operator <(PosixTimeval a, PosixTimeval b)
        {
            return a.Value < b.Value;
        }

        /// <summary>
        /// Operator &gt; overload
        /// </summary>
        /// <param name="a">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="b">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public static bool operator >(PosixTimeval a, PosixTimeval b)
        {
            return a.Value > b.Value;
        }

        /// <summary>
        /// Operator &lt;=
        /// </summary>
        /// <param name="a">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="b">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public static bool operator <=(PosixTimeval a, PosixTimeval b)
        {
            return a.Value <= b.Value;
        }

        /// <summary>
        /// Operator &gt;=
        /// </summary>
        /// <param name="a">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="b">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public static bool operator >=(PosixTimeval a, PosixTimeval b)
        {
            return a.Value >= b.Value;
        }

        /// <summary>
        /// Operator ==
        /// </summary>
        /// <param name="a">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="b">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public static bool operator ==(PosixTimeval a, PosixTimeval b)
        {
            // object.Equals() checks for null and then calls a.Equals(b)
            return Equals(a, b);
        }

        /// <summary>
        /// Operator !=
        /// </summary>
        /// <param name="a">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="b">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public static bool operator !=(PosixTimeval a, PosixTimeval b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">
        /// A <see cref="object"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            var pt = (PosixTimeval)obj;

            return Value == pt.Value;
        }

        /// <summary>
        /// GetHashCode override
        /// </summary>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        private static decimal GetUnixTimeVal(DateTime datetime)
        {
            // diff this with the dateTime value
            // NOTE: make sure the time is in universal time when performing
            //       the subtraction so we get the difference between epoch in utc
            //       which is the definition of the unix timeval
            decimal ticks = datetime.ToUniversalTime().Subtract(EpochDateTime).Ticks;
            return ticks / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// Constructor with seconds and microSeconds fields
        /// </summary>
        /// <param name="seconds">
        /// A <see cref="ulong"/>
        /// </param>
        /// <param name="microseconds">
        /// A <see cref="ulong"/>
        /// </param>
        public PosixTimeval(ulong seconds, ulong microseconds)
        {
            this.Seconds = seconds;
            this.MicroSeconds = microseconds;
        }

        /// <summary>
        /// Constructor with seconds and fractions fields
        /// </summary>
        /// <param name="seconds">
        /// A <see cref="ulong"/>
        /// </param>
        /// <param name="fractions">
        /// A <see cref="ulong"/>
        /// </param>
        /// <param name="resolution">
        /// A <see cref="TimestampResolution"/>
        /// </param>
        public PosixTimeval(ulong seconds, ulong fractions, TimestampResolution resolution)
        {
            var unit = resolution == TimestampResolution.Nanosecond ? 1e-9M : 1e-6M;
            Value = seconds + (fractions * unit);
        }

        /// <summary>
        /// Construct a PosixTimeval with DateTime object
        /// </summary>
        /// <param name="time">
        /// The DateTime object to use
        /// </param>
        public PosixTimeval(DateTime time)
        {
            Value = GetUnixTimeVal(time);
        }
        /// <summary>
        /// Construct a PosixTimeval using the current UTC time
        /// </summary>
        public PosixTimeval()
        {
            Value = GetUnixTimeVal(DateTime.UtcNow);
        }

        /// <summary>
        /// Convert the timeval to a string like 'SECONDS.MICROSECONDSs'
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        public override string ToString()
        {
            return Value + "s";
        }

        /// <summary>
        /// Compare this to another
        /// </summary>
        /// <param name="that">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        public int CompareTo(PosixTimeval that)
        {
            return Value.CompareTo(that.Value);
        }
    }
}