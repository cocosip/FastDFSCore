namespace FastDFSCore.Client
{
    public interface IFDFSOptionTranslator
    {
        /// <summary>从文件中读取配置信息
        /// </summary>
        FDFSOption TranslateToOption(string file);

        /// <summary>将配置文件转换成xml字符串
        /// </summary>
        string TranslateToXml(FDFSOption option);
    }
}
