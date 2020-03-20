using System.Net;

namespace FastDFSCore.Codecs.Messages
{
    /// <summary>Storage节点
    /// </summary>
    public class StorageNode
    {
        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>Storage IP地址
        /// </summary>
        public IPEndPoint EndPoint { get; set; }

        /// <summary>StorePathIndex
        /// </summary>
        public byte StorePathIndex { get; set; }

        /// <summary>Ctor
        /// </summary>
        public StorageNode()
        {

        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="endPoint">Storage IPEndPoint</param>
        /// <param name="storePathIndex">StorePathIndex</param>
        public StorageNode(string groupName, IPEndPoint endPoint, byte storePathIndex)
        {
            GroupName = groupName;
            EndPoint = endPoint;
            StorePathIndex = storePathIndex;
        }

        /// <summary>Ctor
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="storePathIndex">StorePathIndex</param>
        public StorageNode(string groupName, string ipAddress, int port, byte storePathIndex)
        {
            GroupName = groupName;
            EndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            StorePathIndex = storePathIndex;
        }
    }
}
