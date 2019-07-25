using System;

namespace FastDFSCore.Client
{
    /// <summary>时间工具类
    /// </summary>
    public static class DateTimeUtil
    {
        /// <summary> 将时间转换成int32类型
        /// </summary>
        public static int ToInt32(DateTime datetime, int defaultValue = 0)
        {
            //默认情况下以1970.01.01为开始时间计算
            try
            {
                var datezero = new DateTime(1970, 1, 1, 0, 0, 0);
                // TimeSpan seconds = end.AddDays(1) - startdate;
                var seconds = datetime - datezero;
                defaultValue = Convert.ToInt32(seconds.TotalSeconds);
            }
            catch (Exception)
            {
                // ignored
            }
            return defaultValue;
        }

        /// <summary> 将时间转换成long类型,以毫秒为单位
        /// </summary>
        public static long ToInt64(DateTime datetime, long defaultValue = 0)
        {
            //默认情况下以1970.01.01为开始时间计算
            try
            {
                var datezero = new DateTime(1970, 1, 1, 0, 0, 0);
                // TimeSpan seconds = end.AddDays(1) - startdate;
                var seconds = datetime - datezero;
                defaultValue = Convert.ToInt64(seconds.TotalMilliseconds);
            }
            catch (Exception)
            {
                // ignored
            }
            return defaultValue;
        }

        /// <summary> 将Int32类型的整数转换成时间
        /// </summary>
        public static DateTime ToDateTime(int seconds)
        {

            var begtime = Convert.ToInt64(seconds) * 10000000; //100毫微秒为单位
            var dt1970 = new DateTime(1970, 1, 1, 0, 0, 0);
            var tricks1970 = dt1970.Ticks; //1970年1月1日刻度
            var timeTricks = tricks1970 + begtime; //日志日期刻度
            var dt = new DateTime(timeTricks); //转化为DateTime
            //DateTime enddt = dt.Date;//获取到日期整数
            return dt;
        }

    }
}
