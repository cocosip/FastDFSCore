using System;
using System.IO;

namespace FastDFSCore.Client
{
    /// <summary>
    /// 附加文件
    /// 
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

        public string FileId { get; set; }


        public AppendFileRequest()
        {

        }

        public AppendFileRequest(string fileId, byte[] contentBytes)
        {
            FileId = fileId;
            Stream = new MemoryStream(contentBytes);
        }

        public AppendFileRequest(string fileId, Stream stream)
        {
            FileId = fileId;
            Stream = stream;
        }

        public override bool StreamTransfer => true;

        public override byte[] EncodeBody(FDFSOption option)
        {
            long length = Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + FileId.Length + Stream.Length;
            byte[] bodyBuffer = new byte[length];

            byte[] fileIdLenBuffer = BitConverter.GetBytes(FileId.Length);
            Array.Copy(fileIdLenBuffer, 0, bodyBuffer, 0, fileIdLenBuffer.Length);

            byte[] fileSizeBuffer = BitConverter.GetBytes(Stream.Length);
            Array.Copy(fileSizeBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE, fileSizeBuffer.Length);

            byte[] fileIdBuffer = option.Charset.GetBytes(FileId);
            Array.Copy(fileIdBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE, fileIdBuffer.Length);


            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_APPEND_FILE, 0);

            return bodyBuffer;
        }
    }
}
