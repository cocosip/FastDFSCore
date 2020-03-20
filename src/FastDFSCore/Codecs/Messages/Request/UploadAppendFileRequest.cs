using FastDFSCore.Utility;
using System.Collections.Generic;
using System.IO;

namespace FastDFSCore.Codecs.Messages
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

        /// <summary>Ctor
        /// </summary>
        public UploadAppendFileRequest()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="storePathIndex">StorePathIndex</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="stream">文件流</param>
        public UploadAppendFileRequest(byte storePathIndex, string fileExt, Stream stream)
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            RequestStream = stream;

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="storePathIndex">StorePathIndex</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="contentBytes">文件二进制</param>
        public UploadAppendFileRequest(byte storePathIndex, string fileExt, byte[] contentBytes)
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            RequestStream = new MemoryStream(contentBytes);
        }

        /// <summary>是否流请求
        /// </summary>
        public override bool StreamRequest => true;

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FDFSOption option)
        {

            byte[] fileSizeBuffer = ByteUtil.LongToBuffer(RequestStream.Length);
            byte[] extBuffer = EndecodeUtil.EncodeFileExt(FileExt, option.Charset);

            long lenth = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN;

            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.Add(StorePathIndex);
            bodyBuffer.AddRange(fileSizeBuffer);
            bodyBuffer.AddRange(extBuffer);

            //头部
            Header = new FDFSHeader(lenth + RequestStream.Length, Consts.STORAGE_PROTO_CMD_UPLOAD_APPENDER_FILE, 0);

            return bodyBuffer.ToArray();
        }
    }
}
