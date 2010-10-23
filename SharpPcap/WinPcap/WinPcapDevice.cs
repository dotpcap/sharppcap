using System;
using System.Text;

namespace SharpPcap.WinPcap
{
    public class WinPcapDevice : LivePcapDevice
    {
        private CaptureMode    m_pcapMode          = CaptureMode.Packets;

        /// <summary>
        /// Fires whenever a new pcap statistics is available for this Pcap Device.<br/>
        /// For network captured packets this event is invoked only when working in "PcapMode.Statistics" mode.
        /// </summary>
        public event StatisticsModeEventHandler OnPcapStatistics;

        public override void Open()
        {
            base.Open();
        }

        /// <value>
        /// WinPcap specific property
        /// </value>
        public virtual CaptureMode Mode
        {
            get
            {
                return m_pcapMode;
            }

            set
            {
                ThrowIfNotWinPcap();
                ThrowIfNotOpen("Mode");

                m_pcapMode = value;
                int result = WinPcap.SafeNativeMethods.pcap_setmode(this.PcapHandle , (int)m_pcapMode);
                if (result < 0)
                    throw new PcapException("Error setting PcapDevice mode. : " + LastError);
            }
        }

        /// <summary>
        /// Open a device with specific flags
        /// WinPcap extension - Use of this method will exclude your application
        ///                     from working on Linux or Mac
        /// </summary>
        public virtual void Open(OpenFlags flags, int read_timeout)
        {
            ThrowIfNotWinPcap();

            if(!Opened)
            {
                var errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE);

                PcapHandle = SafeNativeMethods.pcap_open
                    (   Name,                   // name of the device
                        Pcap.MAX_PACKET_SIZE,   // portion of the packet to capture.
                                                // MAX_PACKET_SIZE (65536) grants that the whole packet will be captured on all the MACs.
                        (short)flags,           // one or more flags
                        (short)read_timeout,    // read timeout
                        IntPtr.Zero,            // no authentication right now
                        errbuf );               // error buffer

                if ( PcapHandle == IntPtr.Zero)
                {
                    string err = "Unable to open the adapter ("+Name+"). "+errbuf.ToString();
                    throw new PcapException( err );
                }
            }
        }

        public override void Close()
        {

            if ( OnPcapStatistics != null)
            {
                foreach(StatisticsModeEventHandler pse in OnPcapStatistics.GetInvocationList())
                {
                    OnPcapStatistics -= pse;
                }
            }
        }

        /// <summary>
        /// Notify the OnPacketArrival delegates about a newly captured packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="PacketDotNet.RawPacket"/>
        /// </param>
        protected virtual void SendPacketArrivalEvent(PacketDotNet.RawPacket p)
        {
            if(Mode == CaptureMode.Packets)
            {
                base.SendPacketArrivalEvent(p);
            }
            else if(Mode == CaptureMode.Statistics)
            {
                var handler = OnPcapStatistics;
                if(handler != null)
                {
                    //Invoke the pcap statistics event
                    handler(this, new StatisticsModeEventArgs(p, this));
                }
            }
        }

        /// <summary>
        /// Sends all packets in a 'PcapSendQueue' out this pcap device
        /// </summary>
        /// <param name="q">
        /// A <see cref="SendQueue"/>
        /// </param>
        /// <param name="transmitMode">
        /// A <see cref="SendQueueTransmitModes"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public int SendQueue( WinPcap.SendQueue q, SendQueueTransmitModes transmitMode )
        {
            return q.Transmit( this, transmitMode);
        }

        /// <value>
        /// Set the kernel value buffer size in bytes
        /// WinPcap extension
        /// </value>
        public int KernelBufferSize
        {
            set
            {
                ThrowIfNotWinPcap();
                ThrowIfNotOpen("Can't set kernel buffer size, the device is not opened");

                int retval = WinPcap.SafeNativeMethods.pcap_setbuff(this.m_pcapAdapterHandle,
                                                            value);
                if(retval != 0)
                {
                    throw new System.InvalidOperationException("pcap_setbuff() failed");
                }
            }   
        }

        /// <value>
        /// Set the minumum amount of data (in bytes) received by the kernel in a single call. 
        /// WinPcap extension
        /// </value>
        public int MinToCopy
        {
            set
            {
                ThrowIfNotWinPcap();
                ThrowIfNotOpen("Can't set MinToCopy size, the device is not opened");

                int retval = WinPcap.SafeNativeMethods.pcap_setmintocopy(this.m_pcapAdapterHandle,
                                                                 value);
                if (retval != 0)
                {
                    throw new System.InvalidOperationException("pcap_setmintocopy() failed");
                }
            }
        }

        /// <summary>
        /// Helper method for ensuring we are running in winpcap. Throws
        /// a PcapWinPcapRequiredException() if not on a windows platform
        /// </summary>
        internal static void ThrowIfNotWinPcap()
        {
            if((Environment.OSVersion.Platform != PlatformID.Win32NT) &&
               (Environment.OSVersion.Platform != PlatformID.Win32Windows))
            {
                throw new WinPcapRequiredException("only supported in winpcap");
            }
        }
    }
}

