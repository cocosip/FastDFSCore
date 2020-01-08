using FastDFSCore.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FastDFSCore.Tests.Client.Common
{
    public class UtilTest
    {

        [Theory]
        [InlineData(".dcm", "dcm")]
        [InlineData("aaa.jpg", "aaa.jpg")]
        public void ParseExtWithOut_Test(string ext,string expected)
        {
            var actual = Util.ParseExtWithOut(ext);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateGroupNameBuffer_Test()
        {
            var v1 = Util.CreateGroupNameBuffer(Encoding.UTF8, "g1");
            Assert.Equal(Consts.FDFS_GROUP_NAME_MAX_LEN, v1.Length);
            var v2= Util.CreateGroupNameBuffer(Encoding.UTF8, "1234567890qwertyuiopasdfghjklzxcvbnm1234567890");
            Assert.Equal(Consts.FDFS_GROUP_NAME_MAX_LEN, v2.Length);
        }

        [Fact]
        public void CreateFileExtBuffer_Test()
        {
            var v1 = Util.CreateFileExtBuffer(Encoding.UTF8, ".jpg");
            Assert.Equal(Consts.FDFS_FILE_EXT_NAME_MAX_LEN, v1.Length);
            var v2 = Util.CreateFileExtBuffer(Encoding.UTF8, ".abcdefghijkl");
            Assert.Equal(Consts.FDFS_FILE_EXT_NAME_MAX_LEN, v2.Length);
        }

        [Fact]
        public void CreatePrefixBuffer_Test()
        {
            var v1 = Util.CreatePrefixBuffer(Encoding.UTF8, "prefix1");
            Assert.Equal(Consts.FDFS_FILE_PREFIX_MAX_LEN, v1.Length);
            var v2 = Util.CreatePrefixBuffer(Encoding.UTF8, "1234567890qwertyuiopasdfghjklzxcvbnm1234567890");
            Assert.Equal(Consts.FDFS_FILE_PREFIX_MAX_LEN, v2.Length);
        }

        [Fact]
        public void StringToByte_ByteToString_Test()
        {
            var v1 = Util.StringToByte(Encoding.UTF8, "hello");
            var v2 = Util.StringToByte(Encoding.Unicode, "hello");
            var v3 = Util.StringToByte(Encoding.UTF8, "hello");
            Assert.NotEqual(v1, v2);
            Assert.Equal(v1, v3);


            var v4 = Util.ByteToString(Encoding.UTF8, v1);
            Assert.Equal("hello", v4);
            var v5 = Util.ByteToString(Encoding.Unicode, v2);
            Assert.Equal("hello", v5);


        }
    }
}
