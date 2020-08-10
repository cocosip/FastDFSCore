namespace FastDFSCore
{
    public interface IClusterSelector
    {
        ClusterConfiguration Get(string name);
    }
}
