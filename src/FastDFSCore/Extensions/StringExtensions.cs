using System;

namespace FastDFSCore.Extensions
{
    /// <summary>String类型扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>获取字符串平台无关的Hashcode
        /// </summary>
        /// <param name="source">字符串</param>
        /// <returns></returns>
        public static int GetStringHashcode(this string source)
        {
            if (source.IsNullOrWhiteSpace())
            {
                return 0;
            }
            unchecked
            {
                int hash = 23;
                foreach (char c in source)
                {
                    hash = (hash << 5) - hash + c;
                }
                if (hash < 0)
                {
                    hash = Math.Abs(hash);
                }
                return hash;
            }
        }

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

        /// <summary>按照正常行结尾格式化字符串
        /// </summary>
        /// <param name="source">字符串</param>
        /// <returns></returns>
        public static string NormalizeLineEndings(this string source)
        {
            return source.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        }


        /// <summary>移除字符串中指定结尾格式的字符
        /// </summary>
        /// <param name="source">字符串</param>
        /// <param name="postFixes">结尾字符串数组</param>
        /// <returns></returns>
        public static string RemovePostFix(this string source, params string[] postFixes)
        {
            if (source.IsNullOrWhiteSpace() || postFixes.IsNullOrEmpty())
            {
                return source;
            }

            foreach (var postFix in postFixes)
            {
                if (source.EndsWith(postFix))
                {
                    return source.Left(source.Length - postFix.Length);
                }
            }
            return source;
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

        /// <summary>
        /// Uses string.Split method to split given string by given separator.
        /// </summary>
        public static string[] Split(this string source, string separator)
        {
            return source.Split(new[] { separator }, StringSplitOptions.None);
        }

        /// <summary>
        /// Uses string.Split method to split given string by given separator.
        /// </summary>
        public static string[] Split(this string source, string separator, StringSplitOptions options)
        {
            return source.Split(new[] { separator }, options);
        }

        /// <summary>
        /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
        /// </summary>
        public static string[] SplitToLines(this string source)
        {
            return source.Split(Environment.NewLine);
        }

        /// <summary>
        /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
        /// </summary>
        public static string[] SplitToLines(this string source, StringSplitOptions options)
        {
            return source.Split(Environment.NewLine, options);
        }

        /// <summary>将首字母变成小写,驼峰写法?(非真正驼峰,驼峰写法首字母为小写,而不是收个单词)
        /// </summary>
        public static string ToCamelCase(this string source)
        {
            if (source.IsNullOrWhiteSpace())
            {
                return source;
            }

            if (source.Length == 1)
            {
                return source.ToLowerInvariant();
            }

            return char.ToLowerInvariant(source[0]) + source.Substring(1);
        }


    }
}
