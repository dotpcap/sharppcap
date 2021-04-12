namespace SharpPcap
{
    /// <summary>
    /// Information common to all captured packets
    /// </summary>
    public interface ICaptureHeader
    {
        /// <summary>
        /// Timestamp of this header instance
        /// </summary>
        PosixTimeval Timeval { get; }
    }
}
