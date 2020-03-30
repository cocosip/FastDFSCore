﻿using System;

namespace FastDFSCore.Utility
{
    /// <summary>时间工具类
    /// </summary>
    public static class DateTimeUtil
    {
        /// <summary> 将Int32类型的整数转换成时间
        /// </summary>
        public static DateTime ToDateTime(int seconds)
        {
            var begtime = Convert.ToInt64(seconds) * 10000000; //100毫微秒为单位
            var dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var tricks1970 = dt1970.Ticks; //1970年1月1日刻度
            var timeTricks = tricks1970 + begtime; //日志日期刻度
            var dt = new DateTime(timeTricks, DateTimeKind.Utc); //转化为DateTime
            return dt;
        }


    }
}