using FastDFSCore.Utility;
using System.Buffers;
using System.Collections.Generic;
using System.IO;

namespace FastDFSCore.Protocols
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
    public class UploadAppendFile : FastDFSReq<UploadAppendFileResp>
    {
        /// <summary>StorePathIndex
        /// </summary>
        public byte StorePathIndex { get; set; }

        /// <summary>文件扩展名
        /// </summary>
        public string FileExt { get; set; }


        /// <summary>Ctor
        /// </summary>
        public UploadAppendFile()
        {
            Header = new FastDFSHeader(Consts.STORAGE_PROTO_CMD_UPLOAD_APPENDER_FILE);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="storePathIndex">StorePathIndex</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="stream">文件流</param>
        public UploadAppendFile(byte storePathIndex, string fileExt, Stream stream) : this()
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            InputStream = stream;

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="storePathIndex">StorePathIndex</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="contentBytes">文件二进制</param>
        public UploadAppendFile(byte storePathIndex, string fileExt, byte[] contentBytes) : this()
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            InputStream = new MemoryStream(contentBytes);
        }

        ///// <summary>是否流请求
        ///// </summary>
        //public override bool StreamRequest => true;

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(ClusterConfiguration configuration)
        {

            var fileSizeBuffer = ByteUtil.LongToBuffer(InputStream.Length);
            var extBuffer = EndecodeUtil.EncodeFileExt(FileExt, configuration.Charset);

            /*long lenth = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN;*/

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
