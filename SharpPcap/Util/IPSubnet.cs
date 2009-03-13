namespace SharpPcap.Util
{   
    public class IPSubnet:IPAddressRange
    {
        virtual public System.String NetworkAddress
        {
            get
            {
                return IPUtil.IpToString(getNetwork(net, mask));
            }
            
        }
        virtual public System.String BroadcastAddress
        {
            get
            {
                return IPUtil.IpToString(getBroadcast(net, mask));
            }
            
        }
        protected internal long net;
        protected internal long mask;
        
        private static long getBroadcast(long net, long mask)
        {
            long m = ~ mask;
            m = m & 0xffffffffL;
            return net | m;
        }
        
        private static long getNetwork(long net, long mask)
        {
            return net & mask;
        }
        
        public IPSubnet(long net, int maskBits):this(net, IPUtil.MaskToLong(maskBits))
        {
        }
        
        public IPSubnet(System.String net, int maskBits):this(IPUtil.IpToLong(net), IPUtil.MaskToLong(maskBits))
        {
        }
        
        public IPSubnet(System.String dottedNet, System.String dottedMask):this(IPUtil.IpToLong(dottedNet), IPUtil.MaskToLong(dottedMask))
        {
        }
        
        public IPSubnet(System.String ipAndMaskBits):this(IPUtil.ExtractIp(ipAndMaskBits), IPUtil.ExtractMaskBits(ipAndMaskBits))
        {
        }
        
        public IPSubnet(long net, long mask):this(getNetwork(net, mask) + 1, getBroadcast(net, mask) - 1, true, getNetwork(net, mask), getBroadcast(net, mask))
        {
            this.net = net;
            this.mask = mask;
        }
        
        private IPSubnet(long min, long max, bool isRandom, long totalMin, long totalMax):base(min, max, isRandom, totalMin, totalMax)
        {
        }
        
        public virtual void  includeBroadcastAddress(bool shouldInclude)
        {
            if (shouldInclude)
            {
                base.Max = getBroadcast(net, mask);
            }
            else
            {
                base.Max = getBroadcast(net, mask) + 1;
            }
        }
        
        public virtual void  includeNetworkAddress(bool shouldInclude)
        {
            if (shouldInclude)
            {
                base.Min = getNetwork(net, mask);
            }
            else
            {
                base.Min = getNetwork(net, mask) - 1;
            }
        }
    }
}
