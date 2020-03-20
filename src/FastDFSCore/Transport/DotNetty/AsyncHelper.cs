using Nito.AsyncEx;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FastDFSCore.Transport.DotNetty
{

    /// <summary>异步帮助类,用来解决DotNetty中的bug, bug地址: https://github.com/Azure/DotNetty/issues/492
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>是否为异步方法
        /// </summary>
        public static bool IsAsync(this MethodInfo method)
        {
            return method.ReturnType.IsTaskOrTaskOfT();
        }

        /// <summary>是否为Task
        /// </summary>
        public static bool IsTaskOrTaskOfT(this Type type)
        {
            return type == typeof(Task) || type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }

        /// <summary>包裹Task
        /// </summary>
        public static Type UnwrapTask(Type type)
        {
            if (type == typeof(Task))
            {
                return typeof(void);
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return type.GenericTypeArguments[0];
            }

            return type;
        }

        /// <summary>同步方式运行,返回结果
        /// </summary>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncContext.Run(func);
        }

        /// <summary>同步方式运行
        /// </summary>
        public static void RunSync(Func<Task> action)
        {
            AsyncContext.Run(action);
        }
    }
}
