using System;
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

        public override bool StreamTransfer => true;

        public override byte[] EncodeBody(FDFSOption option)
        {
            //扩展名数组
            if (FileExt.Length > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
                throw new ArgumentException("文件扩展名过长");
            byte[] extBuffer = new byte[Consts.FDFS_FILE_EXT_NAME_MAX_LEN];
            byte[] bse = option.Charset.GetBytes(FileExt);
            int ext_name_len = bse.Length;
            if (ext_name_len > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
            {
                ext_name_len = Consts.FDFS_FILE_EXT_NAME_MAX_LEN;
            }
            Array.Copy(bse, 0, extBuffer, 0, ext_name_len);

            long headerLength = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN;
            byte[] bodyBuffer = new byte[headerLength];
            bodyBuffer[0] = StorePathIndex;

            byte[] fileSizeBuffer = BitConverter.GetBytes(Stream.Length);
            Array.Copy(fileSizeBuffer, 0, bodyBuffer, 1, fileSizeBuffer.Length);
            Array.Copy(extBuffer, 0, bodyBuffer, 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE, extBuffer.Length);

            //头部
            Header = new FDFSHeader(headerLength + Stream.Length, Consts.STORAGE_PROTO_CMD_UPLOAD_APPENDER_FILE, 0);

            return bodyBuffer;
        }
    }
}
