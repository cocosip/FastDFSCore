namespace FastDFSCore.Client
{
    public class DeleteFileResponse : FDFSResponse
    {
        public bool Success { get { return Header.Status == 0; } }

        public DeleteFileResponse()
        {

        }
    }
}
