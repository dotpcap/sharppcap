using System;

namespace Test.Performance
{
    /// <summary>
    /// Compute a rate given a start and end DateTime and an event count
    /// </summary>
    public class Rate
    {
        private TimeSpan Elapsed;
        private string EventType;
        private int EventCount;

        public Rate(DateTime Start, DateTime End,
                    int EventCount, string EventType)
        {
            Elapsed = End - Start;
            this.EventCount = EventCount;
            this.EventType = EventType;
        }

        /// <value>
        /// Returns the rate in terms of events per second
        /// </value>
        public double RatePerSecond
        {
            get
            {
                return ((Double)EventCount / (Double)Elapsed.Ticks) * TimeSpan.TicksPerSecond;
            }
        }

        public override string ToString ()
        {
            return String.Format(" {0,10} {1} at a rate of {2,12} / second ({3} seconds elapsed)",
                                 EventCount,
                                 EventType,
                                 RatePerSecond.ToString("n"),
                                 Elapsed);
        }
    }
}
