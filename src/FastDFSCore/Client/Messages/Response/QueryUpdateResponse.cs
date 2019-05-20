namespace FastDFSCore.Client
{
    public class QueryUpdateResponse : FDFSResponse
    {

        public string GroupName { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }

        public QueryUpdateResponse()
        {

        }

        public QueryUpdateResponse(string groupName, string iPAddress, int port)
        {
            GroupName = groupName;
            IPAddress = iPAddress;
            Port = port;
        }
    }
}
