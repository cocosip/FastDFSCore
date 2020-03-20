using FastDFSCore.Transport.Download;
using System.IO;

namespace FastDFSCore.Codecs.Messages
{
    /// <summary>FastDFSCore通讯的请求
    /// </summary>
    public abstract class FDFSRequest
    {
        /// <summary>头部
        /// </summary>
        public FDFSHeader Header { get; set; }

        /// <summary>是否流请求
        /// </summary>
        public virtual bool StreamRequest { get; set; } = false;

        /// <summary>请求数据流
        /// </summary>
        public Stream RequestStream { get; set; }

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

    /// <summary>FDFSRequest, <seealso cref="FastDFSCore.Codecs.Messages.FDFSRequest"/>
    /// </summary>
    public abstract class FDFSRequest<T> : FDFSRequest where T : FDFSResponse
    {


    }
}
