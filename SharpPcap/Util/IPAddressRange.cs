using System;
using System.Collections;

namespace SharpPcap.Util
{
    public class IPAddressRange:NumberRange, IEnumerator, IEnumerable
    {
        virtual public System.String FisrtIPAddress
        {
            get
            {
                return IPUtil.IpToString(Min);
            }
            
            set
            {
                Min = (IPUtil.IpToLong(value));
            }
            
        }
        virtual public System.String LastIPAddress
        {
            get
            {
                return IPUtil.IpToString(Max);
            }
            
            set
            {
                Max = (IPUtil.IpToLong(value));
            }
            
        }
        virtual public System.String CurrentIPAddress
        {
            get
            {
                return IPUtil.IpToString(CurrentNumber);
            }
            
            set
            {
                CurrentNumber = IPUtil.IpToLong(value);
            }
            
        }
        private const long MIN_IP = 0;
        private const long MAX_IP = 0xffffffffL;
        
        public IPAddressRange():this(MIN_IP, MAX_IP)
        {
        }
        
        public IPAddressRange(long min, long max):this(min, max, true)
        {
        }
        
        public IPAddressRange(long min, long max, bool isRandom):this(min, max, isRandom, MIN_IP, MAX_IP)
        {
        }
        
        public IPAddressRange(long min, long max, long step):this(min, max, step, MIN_IP, MAX_IP)
        {
        }
        
        protected internal IPAddressRange(long min, long max, long step, long totalMin, long totalMax):base(min, max, step, totalMin, totalMax)
        {
        }
        
        protected internal IPAddressRange(long min, long max, bool isRandom, long totalMin, long totalMax):base(min, max, isRandom, totalMin, totalMax)
        {
        }
        
        // String
        
        public IPAddressRange(System.String min, System.String max):this(min, max, true)
        {
        }
        
        public IPAddressRange(System.String min, System.String max, bool isRandom):this(min, max, isRandom, MIN_IP, MAX_IP)
        {
        }
        
        public IPAddressRange(System.String min, System.String max, long step):this(min, max, step, MIN_IP, MAX_IP)
        {
        }
        
        protected internal IPAddressRange(System.String min, System.String max, long step, long totalMin, long totalMax):this(IPUtil.IpToLong(min), IPUtil.IpToLong(max), step, totalMin, totalMax)
        {
        }
        
        protected internal IPAddressRange(System.String min, System.String max, bool isRandom, long totalMin, long totalMax):this(IPUtil.IpToLong(min), IPUtil.IpToLong(max), isRandom, totalMin, totalMax)
        {
        }
        
        public static IPAddressRange fromString(System.String range)
        {
            if (IPUtil.IsRange(range))
            {
                try
                {
                    return new IPSubnet(IPUtil.ExtractIp(range), IPUtil.ExtractMaskBits(range));
                }
                catch (System.Exception e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                }
            }
            return new IPAddressRange(range, range);
        }
        
        // </Constructors>
        
        public override long size()
        {
            return base.size();
        }
        
        public virtual System.String nextIPAddress()
        {
            return IPUtil.IpToString(nextNumber());
        }

        #region IEnumerator Members

        public void Reset()
        {
            throw new Exception("Reset(): Not implemented");
        }

        public object Current
        {
            get
            {
                return CurrentIPAddress;
            }
        }

        public bool MoveNext()
        {
            this.nextIPAddress();
            return true;
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion
    }
}
