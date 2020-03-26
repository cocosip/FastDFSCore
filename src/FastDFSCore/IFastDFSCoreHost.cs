using System;

namespace FastDFSCore
{
    /// <summary>Host
    /// </summary>
    public interface IFastDFSCoreHost
    {
        /// <summary>全局生命周期的
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>设置DI
        /// </summary>
        void SetupDI(IServiceProvider provider);
    }
}
