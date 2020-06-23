using System.Net;

namespace FastDFSCore.Protocols
{
    public class StorageNode
    {
        public string GroupName { get; set; }

        public ConnectionAddress ConnectionAddress { get; set; }

        public byte StorePathIndex { get; set; }

        public StorageNode()
        {

        }

        public StorageNode(string groupName, string ipAddress, int port, byte storePathIndex)
        {
            GroupName = groupName;
            ConnectionAddress = new ConnectionAddress(ipAddress, port);
            StorePathIndex = storePathIndex;
        }
    }
}
