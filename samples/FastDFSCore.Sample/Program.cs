using Microsoft.Extensions.DependencyInjection;
using System;
using FastDFSCore.Client;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Logging.Console;

namespace FastDFSCore.Sample
{
    class Program
    {
        static IServiceProvider _provider;
        static IFDFSClient _fdfsClinet;
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddFastDFSCore(c =>
            {
                c.TrackerMaxConnection = 10;
                c.StorageMaxConnection = 20;
                c.Trackers = new List<IPEndPoint>()
                {
                    new IPEndPoint(IPAddress.Parse("192.168.0.129"),22122)
                };
            });
#pragma warning disable CS0618 // 类型或成员已过时
            InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));
#pragma warning restore CS0618 // 类型或成员已过时

            _provider = services.BuildServiceProvider();

            _fdfsClinet = _provider.GetService<IFDFSClient>();

            UploadFileSample().Wait();

            Console.ReadLine();
        }

        /// <summary>上传单个文件
        /// </summary>
        public static async Task UploadFileSample()
        {
            var storageNode = await _fdfsClinet.GetStorageNodeAsync("group1");
            var filename = @"D:\Pictures\1.jpg";
            var fileId = await _fdfsClinet.UploadFileAsync(storageNode, filename);
            Console.WriteLine("上传文件位置:{0},路径:{1}", filename, fileId);
        }




    }
}
