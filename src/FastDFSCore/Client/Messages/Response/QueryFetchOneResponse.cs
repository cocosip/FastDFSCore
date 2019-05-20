namespace FastDFSCore.Client
{
    public class QueryFetchOneResponse : FDFSResponse
    {

        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>端口号
        /// </summary>
        public int Port { get; set; }

        public QueryFetchOneResponse()
        {

        }

        public QueryFetchOneResponse(string groupName, string iPAddress, int port)
        {
            GroupName = groupName;
            IPAddress = iPAddress;
            Port = port;
        }


    }
}
