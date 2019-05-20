using System.Collections.Generic;

namespace FastDFSCore.Client
{
    public class QueryFetchAllResponse : FDFSResponse
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>IP地址集合
        /// </summary>
        public List<string> IPAddresses { get; set; }

        /// <summary>端口号
        /// </summary>
        public int Port { get; set; }


        public QueryFetchAllResponse()
        {

        }
    }
}
