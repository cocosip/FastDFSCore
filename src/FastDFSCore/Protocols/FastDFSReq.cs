using FastDFSCore.Transport.Download;
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

        /// <summary>是否流返回
        /// </summary>
        public virtual bool StreamResponse { get; set; } = false;

        /// <summary>下载器,返回类型为流的时候,才会使用下载器
        /// </summary>
        public virtual IDownloader Downloader { get; set; }

        /// <summary>对当前body进行编码
        /// </summary>
        public abstract byte[] EncodeBody(FDFSOption option);
    }

    /// <summary>FDFSRequest, <seealso cref="FastDFSCore.Protocols.FastDFSReq"/>
    /// </summary>
    public abstract class FastDFSReq<T> : FastDFSReq where T : FastDFSResp
    {


    }
}
