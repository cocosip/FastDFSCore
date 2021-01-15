using System.Collections.Generic;

namespace FastDFSCore
{
    /// <summary>配置信息
    /// </summary>
    public class FastDFSOptions
    {
        public List<ClusterConfiguration> ClusterConfigurations { get; set; }

        public FastDFSOptions()
        {
            ClusterConfigurations = new List<ClusterConfiguration>();
        }

    }
}
