using System.IO;

namespace FastDFSCore.Protocols
{
    /// <summary>FastDFSCore通讯的请求
    /// </summary>
    public abstract class FastDFSReq
    {
        /// <summary>头部
        /// </summary>
        public FastDFSHeader Header { get; set; }

        /// <summary>请求数据流
        /// </summary>
        public Stream InputStream { get; set; }

        /// <summary>对当前body进行编码
        /// </summary>
        public abstract byte[] EncodeBody(FastDFSOption option);
    }

    /// <summary>FDFSRequest, <seealso cref="FastDFSCore.Protocols.FastDFSReq"/>
    /// </summary>
    public abstract class FastDFSReq<T> : FastDFSReq where T : FastDFSResp
    {


    }
}
