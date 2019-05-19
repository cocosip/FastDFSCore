namespace FastDFSCore.Client
{
    /// <summary>查询Storage返回
    /// </summary>
    public class QueryStoreWithGroupResponse : FDFSResponse
    {
        /// <summary>Group名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>StorePathIndex
        /// </summary>
        public byte StorePathIndex { get; set; }

        
         
    }
}
