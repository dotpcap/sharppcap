using System;
using System.Collections;

namespace SharpPcap.Util
{
    public class Int64Range:NumberRange, IEnumerator, IEnumerable
    {
        //UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        
        public Int64Range():this(System.Int64.MinValue, System.Int64.MaxValue)
        {
        }
        
        /// <param name="min">
        /// </param>
        /// <param name="max">
        /// </param>
        /// <throws>  NoSuchAlgorithmException </throws>
        public Int64Range(long min, long max):this(min, max, true)
        {
        }
        
        /// <param name="min">
        /// </param>
        /// <param name="max">
        /// </param>
        /// <param name="isRandom">
        /// </param>
        /// <throws>  NoSuchAlgorithmException </throws>
        public Int64Range(long min, long max, bool isRandom):base(min, max, isRandom, System.Int64.MinValue, System.Int64.MaxValue)
        {
        }
        
        
        public Int64Range(long min, long max, long step):base(min, max, step, System.Int64.MinValue, System.Int64.MaxValue)
        {
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
                return base.CurrentNumber;
            }
        }

        public bool MoveNext()
        {
            base.next();
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
