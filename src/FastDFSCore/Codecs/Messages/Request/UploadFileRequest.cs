using FastDFSCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace FastDFSCore.Codecs.Messages
{
    /// <summary>
    /// 上传文件
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_UPLOAD_FILE 11
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
    public class UploadFileRequest : FDFSRequest<UploadFileResponse>
    {
        /// <summary>StorePathIndex
        /// </summary>
        public byte StorePathIndex { get; set; }

        /// <summary>文件扩展名
        /// </summary>
        public string FileExt { get; set; }

        /// <summary>Ctor
        /// </summary>
        public UploadFileRequest()
        {
            Header = new FDFSHeader(Consts.STORAGE_PROTO_CMD_UPLOAD_FILE);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="storePathIndex">StorePathIndex</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="stream">文件流</param>
        public UploadFileRequest(byte storePathIndex, string fileExt, Stream stream) : this()
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
        public UploadFileRequest(byte storePathIndex, string fileExt, byte[] contentBytes) : this()
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            RequestStream = new MemoryStream(contentBytes);
        }


        /// <summary>使用流传输
        /// </summary>
        public override bool StreamRequest => true;

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(FDFSOption option)
        {
            //1.StorePathIndex

            //2.文件长度
            var fileSizeBuffer = EndecodeUtil.EncodeLong(RequestStream.Length);
            //3.扩展名
            byte[] extBuffer = EndecodeUtil.EncodeFileExt(FileExt, option.Charset);
            //4.文件数据,这里不写入
            //int lenth = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN;

            var bodyBuffer = new List<byte>
            {
                StorePathIndex
            };
            bodyBuffer.AddRange(fileSizeBuffer);
            bodyBuffer.AddRange(extBuffer);
            return bodyBuffer.ToArray();
        }


    }
}
