using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace FastDFSCore
{
    public class DefaultClusterSelector : IClusterSelector
    {
        private readonly FastDFSOption _option;

        public DefaultClusterSelector(IOptions<FastDFSOption> options)
        {
            _option = options.Value;
        }

        public virtual ClusterConfiguration Get(string name)
        {
            var configuration = _option.ClusterConfigurations.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (configuration == null)
            {
                //Get default
                if (_option.ClusterConfigurations.Count == 1)
                {
                    return _option.ClusterConfigurations.FirstOrDefault();
                }
            }
            return configuration;
        }
    }
}
