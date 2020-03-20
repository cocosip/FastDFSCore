using System;

namespace FastDFSCore.Codecs.Messages
{
    /// <summary>附加文件返回
    /// </summary>
    public class AppendFileResponse : FDFSResponse
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Ctor
        /// </summary>
        public AppendFileResponse()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileId">文件FileId</param>
        public AppendFileResponse(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }

        /// <summary>LoadContent
        /// </summary>
        public override void LoadContent(FDFSOption option, byte[] data)
        {
            var span = data.AsSpan();
            var groupNameSpan = span.Slice(0, Consts.FDFS_GROUP_NAME_MAX_LEN);
            GroupName = EndecodeUtil.DecodeString(groupNameSpan.ToArray(), option.Charset);

            var fileNameSpan = span.Slice(Consts.FDFS_GROUP_NAME_MAX_LEN, data.Length - Consts.FDFS_GROUP_NAME_MAX_LEN);
            FileId = EndecodeUtil.DecodeString(fileNameSpan.ToArray(), option.Charset);
        }
    }
}
