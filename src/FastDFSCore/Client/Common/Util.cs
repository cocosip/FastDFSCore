namespace FastDFSCore.Client
{
    public static class Util
    {
        /// <summary>去除扩展名开始的.
        /// </summary>
        public static string ParseExtWithOut(string fileExt)
        {
            return fileExt.TrimStart('.');
        }
    }
}
