/*
This file is part of SharpPcap.

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
 * Copyright 2010-2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// Type of frame validation the adapter performs.
    /// An adapter can be instructed to accept different kind of frames: correct frames only, frames with wrong Frame Check Sequence (FCS) only, all frames.
    /// </summary>
    public enum AirPcapValidationType : int
    {
        /// <summary>Accept all the frames the device captures</summary>
        ACCEPT_EVERYTHING = 1,
        /// <summary>Accept correct frames only, i.e. frames with correct Frame Check Sequence (FCS).</summary>
        ACCEPT_CORRECT_FRAMES = 2,
        /// <summary>Accept corrupt frames only, i.e. frames with worng Frame Check Sequence (FCS).</summary>
        ACCEPT_CORRUPT_FRAMES = 3,
        /// <summary>Unknown validation type. You should see it only in case of error.</summary>
        UNKNOWN = 4
    };
}
