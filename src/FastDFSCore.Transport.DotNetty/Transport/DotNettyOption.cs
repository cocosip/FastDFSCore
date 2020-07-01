namespace FastDFSCore.Transport
{
    public class DotNettyOption
    {
        public bool TcpNodelay { get; set; } = true;
        public int WriteBufferHighWaterMark { get; set; } = 16777216;
        public int WriteBufferLowWaterMark { get; set; } = 8388608;
        public bool SoReuseaddr { get; set; } = true;
        public bool AutoRead { get; set; } = true;
    }
}
