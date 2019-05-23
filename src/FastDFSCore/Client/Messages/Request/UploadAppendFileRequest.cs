using System.Collections.Generic;
using System.IO;

namespace FastDFSCore.Client
{
    /// <summary>
    /// 上传可附加的文件
    /// 
    /// Reqeust 
    ///     Cmd: UPLOAD_APPEND_FILE 23
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: filename size
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file bytes size
    ///     @ filename
    ///     @ file bytes: file content 
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename   
    /// </summary>
    public class UploadAppendFileRequest : FDFSRequest<UploadAppendFileResponse>
    {
        /// <summary>StorePathIndex
        /// </summary>
        public byte StorePathIndex { get; set; }

        /// <summary>文件扩展名
        /// </summary>
        public string FileExt { get; set; }


        public UploadAppendFileRequest()
        {

        }

        public UploadAppendFileRequest(byte storePathIndex, string fileExt, Stream stream)
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            Stream = stream;

        }


        public UploadAppendFileRequest(byte storePathIndex, string fileExt, byte[] contentBytes)
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            Stream = new MemoryStream(contentBytes);
        }

        public override bool StreamRequest => true;

        public override byte[] EncodeBody(FDFSOption option)
        {

            byte[] fileSizeBuffer = Util.LongToBuffer(Stream.Length);
            byte[] extBuffer = Util.CreateFileExtBuffer(option.Charset, FileExt);

            long lenth = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN;

            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.Add(StorePathIndex);
            bodyBuffer.AddRange(fileSizeBuffer);
            bodyBuffer.AddRange(extBuffer);

            //头部
            Header = new FDFSHeader(lenth + Stream.Length, Consts.STORAGE_PROTO_CMD_UPLOAD_APPENDER_FILE, 0);

            return bodyBuffer.ToArray();
        }
    }
}
