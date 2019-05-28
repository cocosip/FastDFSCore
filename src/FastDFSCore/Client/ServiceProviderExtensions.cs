using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;

namespace FastDFSCore.Client
{
    public static class ServiceProviderExtensions
    {
        public static object CreateInstance(this IServiceProvider provider, Type type, params object[] args)
        {
            return ActivatorUtilities.CreateInstance(provider, type, args);
        }

        public static T CreateInstance<T>(this IServiceProvider provider, params object[] args)
        {
            return (T)ActivatorUtilities.CreateInstance(provider, typeof(T), args);
        }


        public static Connection CreateConnection(this IServiceProvider provider, ConnectionSetting setting, Action<Connection> closeAction)
        {
            return provider.CreateInstance<Connection>(setting, closeAction);
        }


        public static Pool CreatePool(this IServiceProvider provider, IPEndPoint endPoint, int maxConnection, int connectionLifeTime)
        {
            return provider.CreateInstance<Pool>(endPoint, maxConnection, connectionLifeTime);
        }


        public static IServiceProvider ConfigureFastDFSCore(this IServiceProvider provider, string file = "")
        {
            if (file != "")
            {
                var translator = provider.GetService<IFDFSOptionTranslator>();
                var option = provider.GetService<FDFSOption>();
                var readOption = translator.TranslateToOption(file);
                option.SelfCopy(readOption);

            }
            return provider;
        }
    }
}
