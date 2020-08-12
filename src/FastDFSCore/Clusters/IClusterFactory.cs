using System.Collections.Generic;

namespace FastDFSCore
{
    public interface IClusterFactory
    {
        /// <summary>Get cluster by name
        /// </summary>
        ICluster Get(string name);

        /// <summary>
        /// Get all clusters
        /// </summary>
        /// <returns></returns>
        List<ICluster> GetClusters();

        /// <summary>
        /// Release all clusters and cluster connection pool
        /// </summary>
        void Release();
    }
}
