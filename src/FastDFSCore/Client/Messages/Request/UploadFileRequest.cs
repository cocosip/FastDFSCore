using System;
using System.Collections.Generic;
using System.IO;

namespace FastDFSCore.Client
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



        public UploadFileRequest()
        {

        }

        public UploadFileRequest(byte storePathIndex, string fileExt, Stream stream)
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            Stream = stream;
        }

        public UploadFileRequest(byte storePathIndex, string fileExt, byte[] contentBytes)
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            Stream = new MemoryStream(contentBytes);
        }


        /// <summary>使用流传输
        /// </summary>
        public override bool StreamTransfer => true;

        public override byte[] EncodeBody(FDFSOption option)
        {
            //1.StorePathIndex

            //2.文件长度
            byte[] fileSizeBuffer = BitConverter.GetBytes(Stream.Length);
            //3.扩展名
            if (FileExt.Length > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
                throw new ArgumentException("文件扩展名过长");
            byte[] extBuffer = Util.CreateFileExtBuffer(option.Charset, FileExt);
            //4.文件数据,这里不写入
            int lenth = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN;

            List<byte> bodyBuffer = new List<byte>(lenth);
            bodyBuffer.Add(StorePathIndex);
            bodyBuffer.AddRange(fileSizeBuffer);
            bodyBuffer.AddRange(extBuffer);

            //头部
            Header = new FDFSHeader(lenth + Stream.Length, Consts.STORAGE_PROTO_CMD_UPLOAD_FILE, 0);
            return bodyBuffer.ToArray();
        }


    }
}
