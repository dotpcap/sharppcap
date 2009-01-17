using System;
using System.Collections;

namespace SharpPcap.Util
{
	public class IPAddressList:IPAddressRange
	{
		ArrayList list;
		public IPAddressList(ArrayList list)
		{
			this.list = list;
		}

		override public System.String CurrentIPAddress
		{
			get
			{
				return (String)list[index];
			}
			
			set
			{
				if(list!=null)
					list[index] = value;
			}
			
		}
		override public System.String FisrtIPAddress
		{
			get
			{
				return (String)list[0];
			}
			
			set
			{
				if(list!=null)
					list[0]= value;
			}
			
		}
		override public System.String LastIPAddress
		{
			get
			{
				return (String)list[list.Count - 1];
			}
			
			set
			{
				if(list!=null)
					list[list.Count- 1] = value;
			}
			
		}
		override public long CurrentNumber
		{
			get
			{
				return IPUtil.IpToLong(CurrentIPAddress);
			}
			
			set
			{
				this.CurrentIPAddress = IPUtil.IpToString(value);
			}
			
		}
		override public long Max
		{
			get
			{
				return IPUtil.IpToLong(LastIPAddress);
			}
			
			set
			{
				this.LastIPAddress = IPUtil.IpToString(value);
			}
			
		}
		override public long Min
		{
			get
			{
				return IPUtil.IpToLong(FisrtIPAddress);
			}
			
			set
			{
				this.FisrtIPAddress = IPUtil.IpToString(value);
			}
			
		}
		internal int index = 0;
		private bool first = true;
		
		public override System.String nextIPAddress()
		{
			if (isRandom())
			{				
				index = random.Next() % list.Count;
			}
			else
			{
				if(!first)
					index = ++index % list.Count;
				else
					first=false;
			}
			return CurrentIPAddress;
		}		
		
		public override long size()
		{
			return list.Count;
		}
		
		
		protected internal override long checkBoundries(long num, long _min, long _max)
		{
			return base.checkBoundries(num, _min, _max);
		}
		
		
		protected internal override long checkBoundries(long num)
		{
			return base.checkBoundries(num);
		}
		
		
		protected internal override long checkTotalBoundries(long num)
		{
			return base.checkTotalBoundries(num);
		}		
		
		public override long getStep()
		{
			return base.getStep();
		}
		
		
		public override bool isRandom()
		{
			return base.isRandom();
		}
		
		
		public override long nextNumber()
		{
			return IPUtil.IpToLong(nextIPAddress());
		}
		
		
		public override long nextRandom()
		{			
			index = random.Next() % list.Count;
			return CurrentNumber;
		}
	}
}
