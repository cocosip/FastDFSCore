using System.Collections.Generic;
using System.IO;

namespace FastDFSCore.Client
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
    public class AppendFileRequest : FDFSRequest<AppendFileResponse>
    {

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public AppendFileRequest()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="fileId">文件FileId</param>
        /// <param name="contentBytes">文件二进制数据</param>
        public AppendFileRequest(string fileId, byte[] contentBytes)
        {
            FileId = fileId;
            RequestStream = new MemoryStream(contentBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="fileId">文件FileId</param>
        /// <param name="stream">文件流</param>
        public AppendFileRequest(string fileId, Stream stream)
        {
            FileId = fileId;
            RequestStream = stream;
        }

        /// <summary>是否为流文件请求
        /// </summary>
        public override bool StreamRequest => true;

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FDFSOption option)
        {

            byte[] fileIdLenBuffer = Util.LongToBuffer(FileId.Length);
            byte[] fileSizeBuffer = Util.LongToBuffer(RequestStream.Length);
            byte[] fileIdBuffer = Util.StringToByte(option.Charset, FileId);

            long length = Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + FileId.Length + RequestStream.Length;

            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.AddRange(fileIdLenBuffer);
            bodyBuffer.AddRange(fileSizeBuffer);
            bodyBuffer.AddRange(fileIdBuffer);

            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_APPEND_FILE, 0);

            return bodyBuffer.ToArray();
        }
    }
}
