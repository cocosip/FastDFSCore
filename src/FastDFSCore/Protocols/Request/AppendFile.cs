using FastDFSCore.Utility;
using System.IO;

namespace FastDFSCore.Protocols
{
    /// <summary>
    /// 附加文件
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_APPEND_FILE 24
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file name length
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: append file body length
    ///     @ file name
    ///     @ append body 
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    /// </summary>
    public class AppendFile : FastDFSReq<AppendFileResp>
    {
        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public AppendFile()
        {
            Header = new FastDFSHeader(Consts.STORAGE_PROTO_CMD_APPEND_FILE);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="fileId">文件FileId</param>
        /// <param name="contentBytes">文件二进制数据</param>
        public AppendFile(string fileId, byte[] contentBytes) : this()
        {
            FileId = fileId;
            InputStream = new MemoryStream(contentBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="fileId">文件FileId</param>
        /// <param name="stream">文件流</param>
        public AppendFile(string fileId, Stream stream) : this()
        {
            FileId = fileId;
            InputStream = stream;
        }

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FastDFSOption option)
        {
            var fileIdLenBuffer = ByteUtil.LongToBuffer(FileId.Length);
            var fileSizeBuffer = ByteUtil.LongToBuffer(InputStream.Length);
            var fileIdBuffer = ByteUtil.StringToByte(FileId, option.Charset);

            //long length = Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + FileId.Length + RequestStream.Length;

            return ByteUtil.Combine(fileIdLenBuffer, fileSizeBuffer, fileIdBuffer);

        }
    }
}
