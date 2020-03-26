using System;

namespace FastDFSCore
{
    /// <summary>Host
    /// </summary>
    public class FastDFSCoreHost : IFastDFSCoreHost
    {
        /// <summary>全局生命周期的ServiceProvider
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>设置DI
        /// </summary>
        public void SetupDI(IServiceProvider provider)
        {
            ServiceProvider = provider;
        }
    }
}
