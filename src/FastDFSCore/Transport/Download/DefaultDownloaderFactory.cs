using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastDFSCore.Transport.Download
{

    public class DefaultDownloaderFactory : IDownloaderFactory
    {
        private readonly IServiceProvider _serviceProvider;

 
        public DefaultDownloaderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDownloader CreateDownloader(DownloaderOption option)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var injectDownloaderOption = scope.ServiceProvider.GetService<DownloaderOption>();
                injectDownloaderOption.FilePath = option.FilePath;
                return scope.ServiceProvider.GetService<IDownloader>();
            }
        }
    }
}
