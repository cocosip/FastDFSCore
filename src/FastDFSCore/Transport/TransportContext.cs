using System;

namespace FastDFSCore.Transport
{
    public class TransportContext
    {
        public Type ReqType { get; set; }

        public Type RespType { get; set; }

        public bool IsInputStream { get; set; }

        public bool IsOutputStream { get; set; }

        public string OutputFilePath { get; set; }
    }
}
