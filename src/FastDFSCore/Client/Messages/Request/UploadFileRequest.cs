using System;
using System.Net;

namespace FastDFSCore.Client
{
    /// <summary>使用二进制上传文件
    /// </summary>
    public class UploadFileRequest : FDFSRequest<UploadFileResponse>
    {
        /// <summary>StorePathIndex
        /// </summary>
        public byte StorePathIndex { get; set; }

        /// <summary>内容长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>文件扩展名
        /// </summary>
        public string FileExt { get; set; }

        /// <summary>内容bytes
        /// </summary>
        public byte[] ContentBytes { get; set; }

        public override void SetHeader(long length, byte command, byte status)
        {
            Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_UPLOAD_FILE, 0);
        }

        /// 1,Byte           StorePathIndex
        /// 2,long           FileSize
        /// 3,string         File Ext
        /// 4,byte[FileSize] File Content or FileStream
        public override byte[] EncodeBody(FDFSOption option)
        {
            var body = new byte[1];

            //StorePathIndex
            body[0] = StorePathIndex;

            //文件长度
            var fileSizeBuffer = BitConverter.GetBytes(Length);

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



            return body;
        }


    }
}
