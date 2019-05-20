namespace FastDFSCore.Client
{
    public class DownloadFileResponse : FDFSResponse
    {
        public byte[] ContentBytes { get; set; }


        public DownloadFileResponse()
        {

        }

        public DownloadFileResponse(byte[] contentBytes)
        {
            ContentBytes = contentBytes;
        }
    }
}
