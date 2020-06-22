namespace FastDFSCore.Protocols
{
    /// <summary>删除文件返回
    /// </summary>
    public class DeleteFileResp : FastDFSResp
    {
        /// <summary>是否删除成功
        /// </summary>
        public bool Success { get { return Header.Status == 0; } }

        /// <summary>Ctor
        /// </summary>
        public DeleteFileResp()
        {

        }
    }
}
