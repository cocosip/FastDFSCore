namespace FastDFSCore
{
    /// <summary>配置信息转换器
    /// </summary>
    public interface IFDFSOptionTransformer
    {
        /// <summary>从文件中读取配置信息
        /// </summary>
        FDFSOption GetOptionFromFile(string file);

        /// <summary>将配置信息转换成Xml字符串
        /// </summary>
        string ToXml(FDFSOption option);
    }
}
