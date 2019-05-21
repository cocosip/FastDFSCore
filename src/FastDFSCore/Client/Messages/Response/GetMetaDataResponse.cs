using System.Collections.Generic;

namespace FastDFSCore.Client
{
    public class GetMetaDataResponse : FDFSResponse
    {
        public IDictionary<string, string> MetaData { get; set; }
    }
}
