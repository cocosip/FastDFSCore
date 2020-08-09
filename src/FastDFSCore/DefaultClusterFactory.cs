using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace FastDFSCore
{
    public class DefaultClusterFactory : IClusterFactory
    {
        private readonly IClusterSelector _clusterSelector;
        public DefaultClusterFactory(IClusterSelector clusterSelector)
        {
            _clusterSelector = clusterSelector;
        }




    }
}
