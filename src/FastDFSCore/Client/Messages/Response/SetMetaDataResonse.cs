namespace FastDFSCore.Client
{
    /// <summary>设置MetaData返回
    /// </summary>
    public class SetMetaDataResonse : FDFSResponse
    {
        /// <summary>是否成功
        /// </summary>
        public bool Success { get { return Header.Status == 0; } }

        /// <summary>Ctor
        /// </summary>
        public SetMetaDataResonse()
        {

        }
    }
}
