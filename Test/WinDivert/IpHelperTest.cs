// SPDX-FileCopyrightText: 2020 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using System.Net;
using NUnit.Framework;
using SharpPcap.WinDivert;

namespace Test.WinDivert
{
    [TestFixture]
    [Category("WinDivert")]
    [Platform("Win", Reason = "IpHelper.dll is only available for Windows")]
    public class IpHelperTest
    {
        [Test]
        public void Test()
        {
            var localhost = IPAddress.Parse("127.0.0.1");
            var bestInterface = IpHelper.GetBestInterface(localhost);
            Assert.IsNotNull(bestInterface);

            var external = IPAddress.Parse("8.8.8.8");
            var bestInterfaceIndex = IpHelper.GetBestInterfaceIndex(external);
            Assert.IsFalse(IpHelper.IsOutbound(bestInterfaceIndex, external, localhost));
            Assert.IsFalse(IpHelper.IsOutbound(bestInterfaceIndex, localhost, external));
        }
    }
}
