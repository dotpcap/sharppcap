using System;
namespace SharpPcap.Util
{   

    /// <summary>
    /// NumberRange
    /// </summary>
    public abstract class NumberRange
    {
        virtual public long CurrentNumber
        {
            get
            {
                return current;
            }
            
            set
            {
                this.current = checkBoundries(value);
            }
            
        }
        virtual public long Max
        {
            get
            {
                return max;
            }
            
            set
            {
                this.max = checkTotalBoundries(value);
                if (current > value)
                    current = this.max;
            }
            
        }
        /// <summary> Sets the minimum value of this Range. This also sets the current value to the minimum.</summary>
        /// <param name="min">the minimum value
        /// </param>
        virtual public long Min
        {
            get
            {
                return min;
            }
            
            set
            {
                this.min = checkTotalBoundries(value);
                current = this.min;
            }
            
        }
        private long min;
        private long max;
        private long current;
        private long step = 1;
        private bool isRandom_Renamed_Field = true;
        //protected internal Rand random = Rand.Instance;
        protected Random random = new Random(Rand.Instance.GetInt()); 
        private long _totalMin = System.Int64.MinValue;
        private long _totalMax = System.Int64.MaxValue;
        
        
        protected internal NumberRange(long min, long max, long step, long totalMin, long totalMax)
        {
            //random = Rand.Instance;
            _totalMin = totalMin;
            _totalMax = totalMax;
            if (min > max)
            {
                long tmp = min;
                min = max;
                max = tmp;
            }
            this.Min = min;
            this.Max = max;
            this.step = step;
            this.isRandom_Renamed_Field = false;
        }
        
        protected internal NumberRange(long min, long max, bool isRandom, long totalMin, long totalMax)
        {
            //random = Rand.Instance;
            _totalMin = totalMin;
            _totalMax = totalMax;
            if (min > max)
            {
                long tmp = min;
                min = max;
                max = tmp;
            }
            this.Min = min;
            this.Max = max;
            this.isRandom_Renamed_Field = isRandom;
        }
        
        public virtual long size()
        {
            return max - min + 1;
        }
        
        public virtual System.Object next()
        {
            return nextNumber();
        }
        
        public virtual sbyte nextByte()
        {
            return (sbyte) nextNumber();
        }
        
        public virtual short nextShort()
        {
            return (short) nextNumber();
        }
        
        public virtual int nextInt()
        {
            return (int) nextNumber();
        }
        
        public virtual long nextLong()
        {
            return (long) nextNumber();
        }
        
        bool first = true;
        public virtual long nextNumber()
        {
            if (isRandom())
            {
                current=nextRandom();
                return current;
            }
            
            if(first)
            {
                first = false;
                return current;
            }
            //long _res = current;
            if (current + step > max)
                current = min + (max - current);
            else if (current + step < min)
                current = max - (current - min);
            else
                current += step;
            return current;
        }       
        
        public virtual long nextRandom()
        {
            double _min = min;
            double _max = max;
            double dif = (_max - _min + 1);
            double final = ((random.NextDouble() * dif) + _min);
            return (long) final;
        }
        
        protected internal virtual long checkBoundries(long num)
        {
            return checkBoundries(num, min, max);
        }
        
        protected internal virtual long checkTotalBoundries(long num)
        {
            return checkBoundries(num, _totalMin, _totalMax);
        }
        
        protected internal virtual long checkBoundries(long num, long _min, long _max)
        {
            if (num > _max)
                return _max;
            if (num < _min)
                return _min;
            return num;
        }
        
        public virtual long getStep()
        {
            return step;
        }
        
        public virtual void  setStep(long step)
        {
            this.step = step;
        }
        
        public virtual bool isRandom()
        {
            return isRandom_Renamed_Field;
        }
        
        public virtual void  setRandom(bool isRandom)
        {
            this.isRandom_Renamed_Field = isRandom;
        }
        
        public static Int64Range int64Range()
        {
            return new Int64Range();
        }
        
        public static Int64Range uint32Range()
        {
            return new Int64Range(0, 0xffffffffL);
        }
        
        public static Int64Range uint16Range()
        {
            return new Int64Range(0, 0xffff);
        }
        
        public static Int64Range ubyteRange()
        {
            return new Int64Range(0, 0xff);
        }
    }
}
