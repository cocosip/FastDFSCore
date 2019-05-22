namespace FastDFSCore.Client
{
    public class SetMetaDataResonse : FDFSResponse
    {
        public bool Success { get { return Header.Status == 0; } }

        public SetMetaDataResonse()
        {

        }
    }
}
