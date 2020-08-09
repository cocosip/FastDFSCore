using System.IO;

namespace FastDFSCore.Protocols
{
    public abstract class FastDFSReq
    {
        public FastDFSHeader Header { get; set; }

        public Stream InputStream { get; set; }

        public virtual bool IsOutputStream { get; set; } = false;

        public string OutputFilePath { get; set; }

        public abstract byte[] EncodeBody(ClusterConfiguration configuration);
    }

    /// <summary>FDFSRequest, <seealso cref="FastDFSCore.Protocols.FastDFSReq"/>
    /// </summary>
    public abstract class FastDFSReq<T> : FastDFSReq where T : FastDFSResp
    {


    }
}
