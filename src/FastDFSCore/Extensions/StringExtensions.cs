using System;

namespace FastDFSCore.Extensions
{
    /// <summary>String类型扩展
    /// </summary>
    public static class StringExtensions
    {
        

        /// <summary>判断字符串是否为空
        /// </summary>
        /// <param name="source">字符串</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>判断字符串是否为空或者空格
        /// </summary>
        /// <param name="source">字符串</param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }


        /// <summary>截取指定长度的字符串
        /// </summary>
        /// <param name="source">字符串</param>
        /// <param name="len">截取长度</param>
        /// <returns></returns>
        public static string Left(this string source, int len)
        {
            if (source.IsNullOrEmpty())
            {
                throw new ArgumentNullException("source");
            }

            if (source.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return source.Substring(0, len);
        }


        /// <summary>移除字符串中指定开始格式的前缀
        /// </summary>
        /// <param name="source">字符串</param>
        /// <param name="preFixes">前缀数组</param>
        /// <returns></returns>
        public static string RemovePreFix(this string source, params string[] preFixes)
        {
            if (source.IsNullOrWhiteSpace() || preFixes.IsNullOrEmpty())
            {
                return source;
            }
            foreach (var preFix in preFixes)
            {
                if (source.StartsWith(preFix))
                {
                    return source.Right(source.Length - preFix.Length);
                }
            }

            return source;
        }

        /// <summary>截取字符串右侧指定长度的字符串
        /// </summary>
        public static string Right(this string source, int len)
        {
            if (source.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("source");
            }

            if (source.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return source.Substring(source.Length - len, len);
        }


    }
}
