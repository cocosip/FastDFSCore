using System;

namespace FastDFSCore.Utility
{
    /// <summary>时间工具类
    /// </summary>
    internal static class DateTimeUtil
    {

        /// <summary>将时间转换成int32类型时间戳(从1970-01-01 00:00:00 开始计算)
        /// </summary>
        /// <param name="datetime">时间</param>
        /// <returns></returns>
        internal static int ToInt32(DateTime datetime)
        {
            //默认情况下以1970.01.01为开始时间计算
            var timeSpan = datetime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt32(timeSpan.TotalSeconds);
        }

        /// <summary>将int32类型的整数时间戳转换成时间
        /// </summary>
        /// <param name="seconds">整数时间戳(从1970-01-01 00:00:00 开始计算的总秒数)</param>
        /// <returns></returns>
        internal static DateTime ToDateTime(int seconds)
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
