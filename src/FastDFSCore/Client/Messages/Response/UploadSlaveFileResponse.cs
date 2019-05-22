using System;

namespace FastDFSCore.Client
{
    public class UploadSlaveFileResponse : FDFSResponse
    {
        public string GroupName { get; set; }

        public string FileId { get; set; }

        public UploadSlaveFileResponse()
        {

        }

        public override void LoadContent(FDFSOption option, byte[] data)
        {
            byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(data, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
            GroupName = Util.ByteToString(option.Charset, groupNameBuffer).TrimEnd('\0');

            byte[] fileNameBuffer = new byte[data.Length - Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(data, Consts.FDFS_GROUP_NAME_MAX_LEN, fileNameBuffer, 0, fileNameBuffer.Length);
            FileId = Util.ByteToString(option.Charset, fileNameBuffer).TrimEnd('\0');
        }
    }
}
