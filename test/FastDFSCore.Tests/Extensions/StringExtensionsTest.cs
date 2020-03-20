using FastDFSCore.Extensions;
using System;
using System.Text;
using Xunit;

namespace FastDFSCore.Tests.Extensions
{
    public class StringExtensionsTest
    {
        [Fact]
        public void GetStringHashcode_Test()
        {
            var str1 = "";
            Assert.Equal(0, str1.GetStringHashcode());
            var str2 = "123456";
            Assert.True(str2.GetStringHashcode() > 0);
        }

        [Fact]
        public void IsNullOrEmpty_Test()
        {
            var str1 = "";
            Assert.True(str1.IsNullOrEmpty());

            var str2 = " ";
            Assert.False(str2.IsNullOrEmpty());

            string str3 = null;
            Assert.True(str3.IsNullOrEmpty());
        }

        [Fact]
        public void IsNullOrWhiteSpace_Test()
        {
            string str1 = null;
            Assert.True(str1.IsNullOrWhiteSpace());
            var str2 = "";
            Assert.True(str2.IsNullOrWhiteSpace());
            var str3 = " ";
            Assert.True(str3.IsNullOrWhiteSpace());
        }

        [Fact]
        public void Left_Test()
        {
            string str1 = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                str1.Left(3);
            });

            string str2 = "12";
            Assert.Throws<ArgumentException>(() =>
            {
                str2.Left(3);
            });

            var str3 = "abc123";
            Assert.Equal("abc", str3.Left(3));
        }

        [Fact]
        public void Right_Test()
        {
            var str1 = "";
            Assert.Throws<ArgumentNullException>(() =>
            {
                str1.Right(2);
            });

            var str2 = "haha";
            Assert.Throws<ArgumentException>(() =>
            {
                str2.Right(6);
            });

            var str3 = "ABC123";
            Assert.Equal("C123", str3.Right(4));

        }

        [Fact]
        public void NormalizeLineEndings_Test()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("hello");
            sb.Append(" world! ");
            sb.Append("I'm ZhangSan");
            var str1 = sb.ToString();

            Assert.Equal("hello world! I'm ZhangSan", str1.NormalizeLineEndings());
        }

        [Fact]
        public void RemovePostFix_Test()
        {
            var str1 = "";
            Assert.Equal(str1, str1.RemovePostFix("1"));
            var str2 = "123";
            Assert.Equal("", str2.RemovePostFix("123", "3"));

            var str3 = "abcdef";
            Assert.Equal("abcde", str3.RemovePostFix("f", "de", "ab"));

            var str4 = "12345AbCde";
            Assert.Equal("12345AbC", str4.RemovePostFix("E", "de"));
            Assert.Equal("12345AbCde", str4.RemovePostFix("3", "De"));
        }

        [Fact]
        public void RemovePreFix_Test()
        {
            var str1 = "";
            Assert.Equal(str1, str1.RemovePreFix("1"));
            var str2 = "123";
            Assert.Equal(str2, str2.RemovePreFix());

            var str3 = "12AB34";
            Assert.Equal("B34", str3.RemovePreFix("12A", "12"));
            Assert.Equal("AB34", str3.RemovePreFix("12", "12A"));

            var str4 = "Ab12C";
            Assert.Equal("b12C", str4.RemovePreFix("ab", "A"));
            Assert.Equal("Ab12C", str4.RemovePreFix("ab", "a"));

        }

        [Fact]
        public void Split_Test()
        {
            var str1 = "abc,def,hij";
            var array1 = str1.Split(",");
            Assert.Equal("abc", array1[0]);
            Assert.Equal("def", array1[1]);
            Assert.Equal("hij", array1[2]);


            var str2 = "123,abc,xyz,";
            var array2 = str2.Split(",", StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(3, array2.Length);
            Assert.Equal("123", array2[0]);
            Assert.Equal("abc", array2[1]);
            Assert.Equal("xyz", array2[2]);

        }

        [Fact]
        public void SplitToLines_Test()
        {
            var sb1 = new StringBuilder();
            sb1.AppendLine("aaa");
            sb1.AppendLine("");
            sb1.AppendLine("bbb");
            sb1.AppendLine(" ");
            sb1.AppendLine("ccc");

            var array1 = sb1.ToString().SplitToLines();
            Assert.Equal(6, array1.Length);
            Assert.Equal("aaa", array1[0]);
            Assert.Equal("", array1[1]);
            Assert.Equal(" ", array1[3]);

            var array2 = sb1.ToString().SplitToLines(StringSplitOptions.None);
            Assert.Equal(6, array2.Length);

            var array3 = sb1.ToString().SplitToLines(StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(4, array3.Length);
            Assert.Equal(" ", array3[2]);
            Assert.Equal("ccc", array3[3]);
        }

        [Fact]
        public void ToCamelCase_Test()
        {
            var str1 = "";
            Assert.Equal(str1, str1.ToCamelCase());

            var str2 = "Z";
            Assert.Equal("z", str2.ToCamelCase());

            var str3 = "ZHANGSAN";
            Assert.Equal("zHANGSAN", str3.ToCamelCase());

        }

    }
}
