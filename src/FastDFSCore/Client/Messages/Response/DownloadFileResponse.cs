namespace FastDFSCore.Client
{
    public class DownloadFileResponse : FDFSResponse
    {
        public byte[] ContentBytes { get; set; }


        public DownloadFileResponse()
        {

        }

        public override void LoadContent(FDFSOption option, byte[] data)
        {
            ContentBytes = data;
        }
    }
}
