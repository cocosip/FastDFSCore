namespace FastDFSCore.Protocols
{
    /// <summary>设置MetaData返回
    /// </summary>
    public class SetMetaDataResp : FastDFSResp
    {
        /// <summary>是否成功
        /// </summary>
        public bool Success { get { return Header.Status == 0; } }

        /// <summary>Ctor
        /// </summary>
        public SetMetaDataResp()
        {

        }
    }
}
