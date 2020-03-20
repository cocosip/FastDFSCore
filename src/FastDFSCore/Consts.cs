using System;

namespace FastDFSCore
{
    /// <summary>常量
    /// </summary>
    public class Consts
    {
        /// <summary>扩展名最大长度, fastdfs源码中: common/fdfs_global.h 
        /// </summary>
        public const byte FDFS_FILE_EXT_NAME_MAX_LEN = 6; //扩展名最大长度  // common/fdfs_global.h 

        /// <summary>版本长度
        /// </summary>
        public const byte FDFS_VERSION_SIZE = 6;

        /// <summary>协议固定长度
        /// </summary>
        public const byte FDFS_PROTO_PKG_LEN_SIZE = 8;

        /// <summary>Int长度
        /// </summary>
        public const byte FDFS_PROTO_PKG_INT_LEN_SIZE = 4;

        /// <summary>GroupInfo长度
        /// </summary>
        public const byte FDFS_GROUP_INFO_SIZE = 105;

        /// <summary>StorageInfo长度
        /// </summary>
        public const int FDFS_STORAGE_INFO_SIZE = 612;

        /// <summary>IP地址长度
        /// </summary>
        public const byte IP_ADDRESS_SIZE = 16;

        /// <summary>Storage Id的最大长度
        /// </summary>
        public const byte FDFS_STORAGE_ID_MAX_SIZE = 16;

        /// <summary>文件前缀最大的长度    tracker/tracker_types.h
        /// </summary>
        public const byte FDFS_FILE_PREFIX_MAX_LEN = 16;

        /// <summary>组名最大长度
        /// </summary>
        public const byte FDFS_GROUP_NAME_MAX_LEN = 16;

        /// <summary>域名最大长度
        /// </summary>
        public const Int16 FDFS_DOMAIN_NAME_MAX_SIZE = 128;

        /***Command-Tracker***/

        /// <summary>列出出一个Group信息
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_ONE_GROUP = 90;

        /// <summary>列出全部的Group信息
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_ALL_GROUPS = 91;

        /// <summary>查询出Storage
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_STORAGE = 92;

        /// <summary>查询Storeage
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ONE = 101;

        /// <summary>QUERY_FETCH_ONE
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ONE = 102;

        /// <summary>QUERY_UPDATE
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE = 103;

        /// <summary>用组名查询Storage
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE = 104;

        /// <summary>QUERY_FETCH_ALL
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ALL = 105;

        /// <summary>查询全部的Storage
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ALL = 106;

        /// <summary>按照组名查询全部Storage
        /// </summary>
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ALL = 107;

        /***Command-Storage***/

        /// <summary>上传文件
        /// </summary>
        public const byte STORAGE_PROTO_CMD_UPLOAD_FILE = 11;

        /// <summary>删除文件
        /// </summary>
        public const byte STORAGE_PROTO_CMD_DELETE_FILE = 12;

        /// <summary>设置METADATA
        /// </summary>
        public const byte STORAGE_PROTO_CMD_SET_METADATA = 13;

        /// <summary>下载文件
        /// </summary>
        public const byte STORAGE_PROTO_CMD_DOWNLOAD_FILE = 14;

        /// <summary>获取METADATA信息
        /// </summary>
        public const byte STORAGE_PROTO_CMD_GET_METADATA = 15;

        /// <summary>更新Slave文件
        /// </summary>
        public const byte STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE = 21;

        /// <summary>查询文件信息
        /// </summary>
        public const byte STORAGE_PROTO_CMD_QUERY_FILE_INFO = 22;

        /// <summary>上传附加文件
        /// </summary>
        public const byte STORAGE_PROTO_CMD_UPLOAD_APPENDER_FILE = 23;

        /// <summary>附加文件
        /// </summary>
        public const byte STORAGE_PROTO_CMD_APPEND_FILE = 24;

        /// <summary>修改附加文件  modify appender file  3.06新增特性
        /// </summary>
        public const byte STORAGE_PROTO_CMD_MODIFY_FILE = 34;

        /// <summary>删除附加文件 truncate appender file 3.06新增特性
        /// </summary>
        public const byte STORAGE_PROTO_CMD_TRUNCATE_FILE = 36;

        /// <summary>退出
        /// </summary>
        public const byte FDFS_PROTO_CMD_QUIT = 82;


        /// <summary>METADATA_KEY_VALUE_SEPARATOR
        /// </summary>
        public const byte METADATA_KEY_VALUE_SEPARATOR = 0x02;

        /// <summary>METADATA_PAIR_SEPARATER
        /// </summary>
        public const byte METADATA_PAIR_SEPARATER = 0x01;
    }

    /// <summary>MetaData选项
    /// </summary>
    public enum MetaDataOption
    {
        /// <summary>重写
        /// </summary>
        Overwrite,

        /// <summary>合并
        /// </summary>
        Merge
    }
}
