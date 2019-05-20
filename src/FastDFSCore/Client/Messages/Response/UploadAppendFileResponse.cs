namespace FastDFSCore.Client
{
    public class UploadAppendFileResponse : FDFSResponse
    {
        public string GroupName { get; set; }

        public string FileId { get; set; }

        public UploadAppendFileResponse()
        {

        }

        public UploadAppendFileResponse(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }
    }
}
