﻿using System.Collections.Generic;
using System.IO;

namespace FastDFSCore.Client
{

    /// <summary>
    /// 上传从文件
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE 21
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: master filename length
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file size
    ///     @ FDFS_FILE_PREFIX_MAX_LEN bytes: filename prefix
    ///     @ FDFS_FILE_EXT_NAME_MAX_LEN bytes: file ext name, do not include dot (.)
    ///     @ master filename bytes: master filename
    ///     @ file size bytes: file content
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// </summary>
    public class UploadSlaveFileRequest : FDFSRequest<UploadSlaveFileResponse>
    {
        /// <summary>主文件FileId
        /// </summary>
        public string MasterFileId { get; set; }

        /// <summary>从文件,文件前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>文件扩展名
        /// </summary>
        public string FileExt { get; set; }

        /// <summary>Ctor
        /// </summary>
        public UploadSlaveFileRequest()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="masterFileId">主文件FileId</param>
        /// <param name="prefix">前缀</param>
        /// <param name="fileExt">扩展名</param>
        /// <param name="stream">文件流</param>
        public UploadSlaveFileRequest(string masterFileId, string prefix, string fileExt, Stream stream)
        {
            MasterFileId = masterFileId;
            Prefix = prefix;
            FileExt = fileExt;
            RequestStream = stream;
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="masterFileId">主文件FileId</param>
        /// <param name="prefix">前缀</param>
        /// <param name="fileExt">扩展名</param>
        /// <param name="contentBytes">文件二进制</param>
        public UploadSlaveFileRequest(string masterFileId, string prefix, string fileExt, byte[] contentBytes)
        {
            MasterFileId = masterFileId;

            Prefix = prefix;
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
            //文件名长度数组
            byte[] masterFileIdLenBuffer = Util.LongToBuffer((long)MasterFileId.Length);
            //文件长度数组
            byte[] fileSizeBuffer = Util.LongToBuffer(RequestStream.Length);
            //从文件前缀名数据
            byte[] prefixBuffer = Util.CreatePrefixBuffer(option.Charset, Prefix);
            byte[] extBuffer = Util.CreateFileExtBuffer(option.Charset, FileExt);
            //主文件Id
            byte[] masterFileIdBuffer = Util.StringToByte(option.Charset, MasterFileId);




            //2个长度,主文件FileId数组长度,文件长度
            long length = 2 * Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_PREFIX_MAX_LEN + Consts.FDFS_FILE_EXT_NAME_MAX_LEN + masterFileIdBuffer.Length;

            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.AddRange(masterFileIdLenBuffer);
            bodyBuffer.AddRange(fileSizeBuffer);
            bodyBuffer.AddRange(prefixBuffer);
            bodyBuffer.AddRange(extBuffer);
            bodyBuffer.AddRange(masterFileIdBuffer);

            //文件内容
            //Array.Copy(ContentBytes, 0, bodyBuffer, offset, ContentBytes.Length);

            //头部
            Header = new FDFSHeader(length + RequestStream.Length, Consts.STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE, 0);

            return bodyBuffer.ToArray();
        }
    }
}
