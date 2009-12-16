using System;
using SharpPcap.Packets.Util;

namespace SharpPcap.Packets
{
    /// <summary>
    /// Raw packet as loaded from a pcap device or file
    /// </summary>
    public class RawPacket
    {
        public LinkLayers LinkLayerType
        {
            get; set;
        }

        /// <value>
        /// The unix timeval when the packet was created
        /// </value>
        private Timeval timeval;
        public Timeval Timeval
        {
            get
            {
                return timeval;
            }

            set
            {
                timeval = value;
            }
        }

        /// <summary> Fetch data portion of the packet.</summary>
        public virtual byte[] Data
        {
            get;
            set;
        }

        public RawPacket(LinkLayers LinkLayerType,
                         Timeval Timeval,
                         byte[] Data)
        {
            this.LinkLayerType = LinkLayerType;
            this.Timeval = Timeval;
            this.Data = Data;
        }

        public override string ToString ()
        {
            return string.Format("[RawPacket: LinkLayerType={0}, Timeval={1}, Data={2}]", LinkLayerType, Timeval, Data);
        }
    }
}
