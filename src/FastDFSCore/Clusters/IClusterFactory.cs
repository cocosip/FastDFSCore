namespace FastDFSCore
{
    public interface IClusterFactory
    {
        /// <summary>Get cluster by name
        /// </summary>
        ICluster Get(string name);
    }
}
