namespace FastDFSCore.Client
{
    public class UploadSlaveFileResponse : FDFSResponse
    {
        public string GroupName { get; set; }

        public string FileId { get; set; }

        public UploadSlaveFileResponse()
        {

        }

        public UploadSlaveFileResponse(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }
    }
}
