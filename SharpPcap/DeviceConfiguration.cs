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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace SharpPcap
{
    public class DeviceConfiguration
    {
        /// <summary>
        /// Defines if the adapter mode. 
        /// </summary>
        public DeviceModes Mode { get; set; }

        public int ReadTimeout { get; set; } = 1000;

        // MAX_PACKET_SIZE (65536) grants that the whole packet will be captured on all the MACs.
        public int Snaplen { get; set; } = Pcap.MAX_PACKET_SIZE;

        public MonitorMode? Monitor { get; set; }

        public int? BufferSize { get; set; }

        public int? KernelBufferSize { get; set; }

        public RemoteAuthentication Credentials { get; set; }

        public bool? Immediate { get; set; }

        public int? MinToCopy { get; set; }

        #region File IO
        /// <summary>
        /// Writing capture files
        /// </summary>
        public PacketDotNet.LinkLayers LinkLayerType { get; set; }
        #endregion

        public TimestampResolution? TimestampResolution { get; set; }

        public TimestampType? TimestampType { get; set; }

        public event EventHandler<ConfigurationFailedEventArgs> ConfigurationFailed;

        internal void RaiseConfigurationFailed(string property, PcapError error, string message)
        {
            message = message ?? $"Failed to set {property}.";

            if (ConfigurationFailed is null)
            {
                var exception = error == PcapError.PlatformNotSupported ?
                    (Exception)new PlatformNotSupportedException() :
                    new PcapException(message, error);
                throw exception;
            }
            else
            {
                var args = new ConfigurationFailedEventArgs
                {
                    Property = property,
                    Error = error,
                    Message = message,
                };
                ConfigurationFailed.Invoke(this, args);
            }
        }
    }

    public class ConfigurationFailedEventArgs : EventArgs
    {
        public PcapError Error { get; internal set; }
        public string Property { get; internal set; }
        public string Message { get; internal set; }
    }
}
