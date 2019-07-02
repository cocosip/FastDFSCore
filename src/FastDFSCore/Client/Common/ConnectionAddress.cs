using System.Net;

namespace FastDFSCore.Client
{
    public class ConnectionAddress
    {
        public IPEndPoint ServerEndPoint { get; set; }

        public IPEndPoint LocalEndPoint { get; set; }


        public ConnectionAddress()
        {

        }
    }
}
