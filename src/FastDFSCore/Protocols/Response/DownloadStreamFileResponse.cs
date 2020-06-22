namespace FastDFSCore.Protocols
{

    /// <summary>下载流文件返回
    /// </summary>
    public class DownloadStreamFileResponse : FDFSResponse
    {
        /// <summary>保存文件的路径
        /// </summary>
        public string FilePath { get; set; }
    }
}
