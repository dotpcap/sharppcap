using System;
using NUnit.Framework;

namespace Tamir.IPLib.Util
{
	
	[TestFixture]
	public class Int64RangeTest
	{
		internal int max = 200;
		internal Int64Range range;

		[TestFixtureSetUp]
		public void  SetUp()
		{
			range = new Int64Range(- 50, 50);
		}
		[Test]
		public void  testRandomInRange()
		{
			int count = max;
			range.setRandom(true);
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			foreach(long l in range)
			{
				Assert.IsTrue(l <= range.Max,l + " should be <= " + range.Max);
				Assert.IsTrue(l >= range.Min, l + " should be >= " + range.Min);
				if (--count == 0)
					break;
			}
		}
		[Test]
		public void  testIncrementInRange()
		{
			int count = max;
			range.setRandom(false);
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			foreach(long l in range)
			{
				Assert.IsTrue(l <= range.Max, l + " should be <= " + range.Max);
				Assert.IsTrue(l >= range.Min,l + " should be >= " + range.Min);
				if (--count == 0)
					break;
			}
		}
		[Test]
		public virtual void  testFactory()
		{
			//		Int32Range uint16 = NumberRange.uint16Range();
			//		Int32Range ubyte = NumberRange.ubyteRange();
			//		uint16.setRandom(false);
			//		ubyte.setRandom(false);
			//		
			//		for(int i=0; i<uint16.size(); i++)
			//		{
			//			System.out.println(i);
			//		}
			//		System.out.println();
			//		for(int i=0; i<ubyte.size(); i++)
			//		{
			//			System.out.println(i);
			//		}
		}
		[Test]
		public virtual void  testRandom()
		{
			NumberRange range = NumberRange.int64Range();
			range.Max = System.Int64.MaxValue - 2;
			long last = 0, cur;
			for (int i = 0; i < max; i++)
			{
				cur = range.nextNumber();
				Assert.IsTrue(cur != last, "Next val shouldn't be as current val ("+cur+"/"+last+")");
				System.Console.WriteLine(cur);
				last = cur;
			}
		}
	}
}