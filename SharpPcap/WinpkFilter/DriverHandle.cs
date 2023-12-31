// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using Microsoft.Win32.SafeHandles;

namespace SharpPcap.WinpkFilter
{
    public class DriverHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverHandle"/> class.
        /// </summary>
        public DriverHandle()
            : base(true)
        {
        }

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            NativeMethods.CloseFilterDriver(handle);

            return true;
        }
    }
}