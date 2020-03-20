using FastDFSCore.Codecs;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FastDFSCore.Tests.Codecs
{
    public class EndecodeUtilTest
    {
        [Fact]
        public void EncodeGroupName_Test()
        {
            var v1 = EndecodeUtil.EncodeGroupName("g1");
            Assert.Equal(Consts.FDFS_GROUP_NAME_MAX_LEN, v1.Length);
            var v2 = EndecodeUtil.EncodeGroupName("1234567890qwertyuiopasdfghjklzxcvbnm1234567890", "utf-8");
            Assert.Equal(Consts.FDFS_GROUP_NAME_MAX_LEN, v2.Length);
        }


        [Fact]
        public void EncodeFileExtBuffer_Test()
        {
            var v1 = EndecodeUtil.EncodeFileExt(".jpg", "utf-8");
            Assert.Equal(Consts.FDFS_FILE_EXT_NAME_MAX_LEN, v1.Length);
            var v2 = EndecodeUtil.EncodeFileExt(".abcdefghijkl", "utf-8");
            Assert.Equal(Consts.FDFS_FILE_EXT_NAME_MAX_LEN, v2.Length);
        }

        [Fact]
        public void CreatePrefixBuffer_Test()
        {
            var v1 = EndecodeUtil.EncodePrefix("prefix1", "utf-8");
            Assert.Equal(Consts.FDFS_FILE_PREFIX_MAX_LEN, v1.Length);
            var v2 = EndecodeUtil.EncodePrefix("1234567890qwertyuiopasdfghjklzxcvbnm1234567890","utf-8");
            Assert.Equal(Consts.FDFS_FILE_PREFIX_MAX_LEN, v2.Length);
        }

    }
}
