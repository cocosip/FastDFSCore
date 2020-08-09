using FastDFSCore.Utility;
using System;

namespace FastDFSCore.Protocols
{
    /// <summary>上传附加文件返回
    /// </summary>
    public class UploadAppendFileResp : FastDFSResp
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public UploadAppendFileResp()
        {

        }

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(ClusterConfiguration configuration, byte[] data)
        {
            byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(data, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
            GroupName = ByteUtil.ByteToString(groupNameBuffer, configuration.Charset).TrimEnd('\0');

            byte[] fileNameBuffer = new byte[data.Length - Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(data, Consts.FDFS_GROUP_NAME_MAX_LEN, fileNameBuffer, 0, fileNameBuffer.Length);
            FileId = ByteUtil.ByteToString(fileNameBuffer, configuration.Charset).TrimEnd('\0');
        }
    }
}
