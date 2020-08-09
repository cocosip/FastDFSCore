using FastDFSCore.Utility;
using System.Buffers;
using System.Collections.Generic;
using System.IO;

namespace FastDFSCore.Protocols
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
    public class UploadFile : FastDFSReq<UploadFileResp>
    {
        /// <summary>StorePathIndex
        /// </summary>
        public byte StorePathIndex { get; set; }

        /// <summary>文件扩展名
        /// </summary>
        public string FileExt { get; set; }

        public byte[] Content { get; set; }

        /// <summary>Ctor
        /// </summary>
        public UploadFile()
        {
            Header = new FastDFSHeader(Consts.STORAGE_PROTO_CMD_UPLOAD_FILE);
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="storePathIndex">StorePathIndex</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="stream">文件流</param>
        public UploadFile(byte storePathIndex, string fileExt, Stream stream) : this()
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            InputStream = stream;
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="storePathIndex">StorePathIndex</param>
        /// <param name="fileExt">文件扩展名</param>
        /// <param name="content">文件二进制</param>
        public UploadFile(byte storePathIndex, string fileExt, byte[] content) : this()
        {
            StorePathIndex = storePathIndex;
            FileExt = fileExt;
            Content = content;
        }


        ///// <summary>使用流传输
        ///// </summary>
        //public override bool StreamRequest => true;

        /// <summary>EncodeBody
        /// </summary>
        public override byte[] EncodeBody(ClusterConfiguration configuration)
        {
            //1.StorePathIndex

            //2.文件长度

            //var fileSizeBuffer = EndecodeUtil.EncodeLong(InputStream.Length);
            byte[] fileSizeBuffer;
            if (InputStream != null)
            {
                fileSizeBuffer = EndecodeUtil.EncodeLong(InputStream.Length);
            }
            else
            {
                fileSizeBuffer = EndecodeUtil.EncodeLong(Content.Length);
            }


            //3.扩展名
            byte[] extBuffer = EndecodeUtil.EncodeFileExt(FileExt, configuration.Charset);
            //4.文件数据,这里不写入
            //int lenth = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN;

            var bodyBuffer = new List<byte>
            {
                StorePathIndex
            };
            bodyBuffer.AddRange(fileSizeBuffer);
            bodyBuffer.AddRange(extBuffer);
            if (InputStream == null && Content != null)
            {
                bodyBuffer.AddRange(Content);
            }

            return bodyBuffer.ToArray();
        }

    }
}
