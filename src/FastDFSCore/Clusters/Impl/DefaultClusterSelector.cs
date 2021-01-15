using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace FastDFSCore
{
    public class DefaultClusterSelector : IClusterSelector
    {
        private readonly FastDFSOptions _options;

        public DefaultClusterSelector(IOptions<FastDFSOptions> options)
        {
            _options = options.Value;
        }

        public virtual ClusterConfiguration Get(string name)
        {
            var configuration = _options.ClusterConfigurations.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (configuration == null)
            {
                //Get default
                if (_options.ClusterConfigurations.Count == 1)
                {
                    configuration = _options.ClusterConfigurations.FirstOrDefault();
                }
            }

            if (configuration == null)
            {
                throw new ArgumentNullException($"Could not find any cluster configuration by name '{name}'");
            }
            return configuration;
        }
    }
}
