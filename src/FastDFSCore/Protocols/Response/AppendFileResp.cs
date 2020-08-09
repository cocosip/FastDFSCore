using FastDFSCore.Utility;
using System;

namespace FastDFSCore.Protocols
{
    /// <summary>附加文件返回
    /// </summary>
    public class AppendFileResp : FastDFSResp
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public AppendFileResp()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件FileId</param>
        public AppendFileResp(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(ClusterConfiguration configuration, byte[] data)
        {
            var span = data.AsSpan();
            var groupNameSpan = span.Slice(0, Consts.FDFS_GROUP_NAME_MAX_LEN);
            GroupName = EndecodeUtil.DecodeString(groupNameSpan.ToArray(), configuration.Charset);

            var fileNameSpan = span.Slice(Consts.FDFS_GROUP_NAME_MAX_LEN, data.Length - Consts.FDFS_GROUP_NAME_MAX_LEN);
            FileId = EndecodeUtil.DecodeString(fileNameSpan.ToArray(), configuration.Charset);
        }
    }
}
