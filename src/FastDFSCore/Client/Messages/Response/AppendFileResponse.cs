namespace FastDFSCore.Client
{
    public class AppendFileResponse : FDFSResponse
    {
        public string GroupName { get; set; }
        public string FileId { get; set; }

        public AppendFileResponse()
        {

        }

        public AppendFileResponse(string groupName, string fileId)
        {
            GroupName = groupName;
            FileId = fileId;
        }
    }
}
