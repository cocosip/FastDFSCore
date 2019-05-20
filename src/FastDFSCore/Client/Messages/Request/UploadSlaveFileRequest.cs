using System;
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

        public string MasterFileId { get; set; }

        /// <summary>从文件,文件前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>文件扩展名
        /// </summary>
        public string FileExt { get; set; }

        public UploadSlaveFileRequest()
        {

        }

        public UploadSlaveFileRequest(string masterFileId, string prefix, string fileExt, Stream stream)
        {
            MasterFileId = masterFileId;
            Prefix = prefix;
            FileExt = fileExt;
            Stream = stream;
        }

        public UploadSlaveFileRequest(string masterFileId, string prefix, string fileExt, byte[] contentBytes)
        {
            MasterFileId = masterFileId;

            Prefix = prefix;
            FileExt = fileExt;
            Stream = new MemoryStream(contentBytes);
        }

        public override bool StreamTransfer => true;

        public override byte[] EncodeBody(FDFSOption option)
        {
            //扩展名数组
            if (FileExt.Length > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
                throw new ArgumentException("文件扩展名过长");
            if (Prefix.Length > Consts.FDFS_FILE_PREFIX_MAX_LEN)
                throw new ArgumentException("从文件前缀名过长");

            byte[] extBuffer = new byte[Consts.FDFS_FILE_EXT_NAME_MAX_LEN];
            byte[] bse = option.Charset.GetBytes(FileExt);
            int ext_name_len = bse.Length;
            if (ext_name_len > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
            {
                ext_name_len = Consts.FDFS_FILE_EXT_NAME_MAX_LEN;
            }
            Array.Copy(bse, 0, extBuffer, 0, ext_name_len);

            //主文件数组
            byte[] masterFileIdBuffer = option.Charset.GetBytes(MasterFileId);
            //文件名长度数组
            byte[] masterFileIdLenBuffer = BitConverter.GetBytes((long)MasterFileId.Length);

            //文件长度数组
            byte[] fileSizeBuffer = BitConverter.GetBytes(Stream.Length);

            //从文件前缀名数据
            byte[] prefixBuffer = new byte[Consts.FDFS_FILE_PREFIX_MAX_LEN];
            byte[] prefixRealBytes = option.Charset.GetBytes(Prefix);
            Array.Copy(prefixRealBytes, 0, prefixBuffer, 0, prefixRealBytes.Length);

            //2个长度,主文件FileId数组长度,文件长度
            long length = 2 * Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_PREFIX_MAX_LEN + Consts.FDFS_FILE_EXT_NAME_MAX_LEN + masterFileIdBuffer.Length + Stream.Length;

            byte[] bodyBuffer = new byte[length];
            var offset = 0;
            //主文件FileId长度
            Array.Copy(masterFileIdLenBuffer, 0, bodyBuffer, offset, masterFileIdLenBuffer.Length);
            offset = masterFileIdLenBuffer.Length;
            //文件长度
            Array.Copy(fileSizeBuffer, 0, bodyBuffer, offset, fileSizeBuffer.Length);
            offset += fileSizeBuffer.Length;
            //Prefix前缀,固定16字节
            Array.Copy(prefixBuffer, 0, bodyBuffer, offset, prefixBuffer.Length);
            offset += prefixBuffer.Length;

            //扩展名
            Array.Copy(extBuffer, 0, bodyBuffer, offset, extBuffer.Length);
            offset += extBuffer.Length;

            //主文件Id
            Array.Copy(masterFileIdBuffer, 0, bodyBuffer, offset, masterFileIdBuffer.Length);
            offset += masterFileIdBuffer.Length;

            //文件内容
            //Array.Copy(ContentBytes, 0, bodyBuffer, offset, ContentBytes.Length);

            //头部
            Header = new FDFSHeader(length + Stream.Length, Consts.STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE, 0);

            return bodyBuffer;
        }
    }
}
