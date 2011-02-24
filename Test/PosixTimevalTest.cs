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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using NUnit.Framework;
using SharpPcap;

namespace Test.Misc
{
    [TestFixture]
    public class PosixTimevalTest
    {
        // Test posix timeval comparison operators
        [Test]
        public void OperatorTest ()
        {
            var p1 = new PosixTimeval(100, 50);
            var p2 = new PosixTimeval(100, 100);
            var p3 = new PosixTimeval(200, 20);

            var p4 = new PosixTimeval(100, 50);

            Assert.IsTrue(p1 < p2, "p1 < p2");
            Assert.IsTrue(p2 < p3, "p2 < p3");
            Assert.IsTrue(p1 < p3, "p1 < p3");

            Assert.IsTrue(p2 > p1, "p2 > p1");
            Assert.IsTrue(p3 > p2, "p3 > p2");
            Assert.IsTrue(p3 > p1, "p3 > p1");

            Assert.IsTrue(p1 != p2, "p1 != p2");

            Assert.IsTrue(p1 == p4, "p1 == p4");

            Assert.IsTrue(p1 <= p2, "p1 <= p2");
            Assert.IsTrue(p2 >= p1, "p2 >= p1");
        }
    }
}
