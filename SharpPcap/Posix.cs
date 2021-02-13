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
 * Copyright 2020-2021 Ayoub Kaanich <kayoub5@live.com>
 */

using System;
using System.Runtime.InteropServices;

namespace SharpPcap
{
	/// <summary>
	/// Posix code in here may seem complex but it serves an important
	/// purpose.
	///
	/// Under Unix, pcap_loop(), pcap_dispatch(), pcap_next() and pcap_next_ex()
	/// all perform blocking read() calls at the os level that have NO timeout.
	/// If the user wishes to stop capturing on an adapter they will need to wait
	/// until the next packet arrives for the capture loop to wake up and look to see
	/// if it has been asked to shut down. This may be never in the case of inactive
	/// adapters or far longer than what the user desires.
	///
	/// So, to avoid the issue we invoke the poll() system call. 
	/// The thread sleeps on the poll() and only when woken
	/// up and indicating that data is available do we call one of the pcap
	/// data retrieval routines. This is how we avoid blocking for long periods
	/// or forever.
	///
	/// Poll enables us to set a timeout. The timeout is chosen to be long
	/// enough to avoid a noticable performance impact from frequent looping
	/// but short enough to satisify the timing constraints of the Thread.Join() in
	/// the stop capture code.
	///
	/// </summary>
	internal static class Posix
    {
#pragma warning disable CS0649
		public struct Pollfd
		{
			public int fd;
			public PollEvents events;
			public PollEvents revents;
		}
#pragma warning restore CS0649

		[Flags]
		public enum PollEvents : short
		{
			POLLIN = 0x0001, // There is data to read
			POLLPRI = 0x0002, // There is urgent data to read
			POLLOUT = 0x0004, // Writing now will not block
			POLLERR = 0x0008, // Error condition
			POLLHUP = 0x0010, // Hung up
			POLLNVAL = 0x0020, // Invalid request; fd not open
		}

		[DllImport("libc", SetLastError = true, EntryPoint = "poll")]
		internal static extern int Poll([In, Out] Pollfd[] ufds, uint nfds, int timeout);
	}


}
