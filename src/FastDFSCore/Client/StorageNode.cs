using System.Net;

namespace FastDFSCore.Client
{
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

        public StorageNode()
        {

        }

        public StorageNode(string groupName, IPEndPoint endPoint, byte storePathIndex)
        {
            GroupName = groupName;
            EndPoint = endPoint;
            StorePathIndex = storePathIndex;
        }
    }
}
