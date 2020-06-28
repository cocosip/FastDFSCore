using FastDFSCore.Protocols;

namespace FastDFSCore.Transport
{
    public class ReceivedPackage
    {
        public bool HasHeader { get; set; }
        public FastDFSHeader Header { get; set; }
        public byte[] Body { get; set; }
    }
}
