/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
using Timeval = SharpPcap.Packets.Util.Timeval;
namespace SharpPcap.Packets
{
    /// <summary> A network packet.
    /// <p>
    /// This class currently contains no implementation because only ethernet 
    /// is supported. In other words, all instances of packets returned by 
    /// packet factory will always be at least as specific as EthernetPacket.
    /// <p>
    /// On large ethernet networks, I sometimes see packets which don't have 
    /// link-level ethernet headers. If and when I figure out what these are, 
    /// maybe this class will be the root node of a packet hierarchy derived 
    /// from something other than ethernet.
    /// 
    /// </summary>
    [Serializable]
    public class Packet
    {
        /// <summary> Fetch data portion of the packet.</summary>
        virtual public byte[] Header
        {
            get
            {
                return null;
            }           
        }

        virtual public System.String Color
        {
            get
            {
                return "";
            }           
        }

        virtual public Timeval Timeval
        {
            get
            {
                return null;
            }           
        }

        virtual public byte[] Bytes
        {
            get
            {
                return null;
            }
            protected set
            {
            }
        }

        virtual public PcapHeader PcapHeader
        {
            get
            {
                if(this.pcapHeader==null)
                    this.pcapHeader=new PcapHeader();
                return this.pcapHeader;
            }

            set
            {
                this.pcapHeader = value;
            }           
        }

        public virtual System.String ToColoredString(bool colored)
        {
            return String.Empty;
        }

        public virtual System.String ToColoredVerboseString(bool colored)
        {
            return String.Empty;
        }

        /// <summary> Fetch data portion of the packet.</summary>
        public virtual byte[] Data
        {
            get
            {
                return null;
            }
        }
        
        internal PcapHeader pcapHeader;
    }
}
