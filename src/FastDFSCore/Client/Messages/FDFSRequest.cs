using System.IO;

namespace FastDFSCore.Client
{

    public abstract class FDFSRequest
    {
        /// <summary>头部
        /// </summary>
        public FDFSHeader Header { get; set; }

        /// <summary>是否是上传文件
        /// </summary>
        public virtual bool IsFileUpload { get; set; } = false;

        /// <summary>是否下载文件
        /// </summary>
        public virtual bool IsFileDownload { get; set; } = false;


        /// <summary>是否下载到路径
        /// </summary>
        public bool IsDownloadToPath { get; set; } = false;

        /// <summary>下载文件
        /// </summary>
        public virtual string FileDownloadPath { get; set; }

        /// <summary>对当前body进行编码
        /// </summary>
        public abstract byte[] EncodeBody(FDFSOption option);

        /// <summary>数据流
        /// </summary>
        public Stream Stream { get; set; }
    }

    public abstract class FDFSRequest<T> : FDFSRequest where T : FDFSResponse
    {


    }
}
