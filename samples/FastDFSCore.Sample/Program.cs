using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace FastDFSCore.Sample
{
    class Program
    {
        private static ISampleAppService _sampleAppService;
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(l =>
            {
                l.SetMinimumLevel(LogLevel.Trace);
                l.AddConsole();
            });
            services
                .AddFastDFSCore(c =>
                {
                    //c.Trackers = new List<Tracker>()
                    //{
                    //    new Tracker("192.168.0.98",22122)
                    //};
                })
                //.AddFastDFSDotNetty()
                .AddFastDFSSuperSocket()
                .AddSingleton<ISampleAppService, SampleAppService>();
            var provider = services.BuildServiceProvider();

            var option = provider.GetRequiredService<IOptions<FastDFSOption>>().Value;
            option.ClusterConfigurations.Add(new ClusterConfiguration()
            {
                Name = "Cluster1",
                Trackers = new List<Tracker>()
                {
                    new Tracker("192.168.0.98", 22122)
                }
            });

            _sampleAppService = provider.GetService<ISampleAppService>();

            RunAsync();

            Console.ReadLine();
        }

        public static async void RunAsync()
        {
            var groupName = "group1";
            var uploadFileIds = await _sampleAppService.BatchUploadAsync(groupName, @"D:\DicomTests");
            var downloadFiles = await _sampleAppService.BatchDownloadAsync(groupName, uploadFileIds, @"G:\Download");

        }



    }
}
