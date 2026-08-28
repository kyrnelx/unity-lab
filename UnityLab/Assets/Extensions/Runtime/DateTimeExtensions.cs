using System;

namespace Kyrnel
{
    public static class DateTimeExtensions
    {
        public const long MinuteSeconds = 60;
        public const long HourSeconds = 3600;
        public const long DaySeconds = 86400;
        public const long WeekSeconds = 604800;

        public const long SecondMilliseconds = 1000;
        public const long MinuteMilliseconds = 60000;
        public const long HourMilliseconds = 3600000;
        public const long DayMilliseconds = 86400000;
        public const long WeekMilliseconds = 604800000;

        /// <summary>
        /// 将指定日期的零点（今日午夜 00:00:00）转换为 Unix 时间戳（秒）。
        /// </summary>
        public static long ToMidnightUnixTimeSeconds(this System.DateTime dateTime)
        {
            var midnight = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            var epoch = new System.DateTime(1970, 1, 1, 0, 0, 0);
            return (long)(midnight - epoch).TotalSeconds;
        }

        /// <summary>
        /// 将指定日期时间转换为 Unix 时间戳（秒）。
        /// </summary>
        public static long ToUnixTimeSeconds(this System.DateTime dateTime)
        {
            var epoch = new System.DateTime(1970, 1, 1, 0, 0, 0);
            var seconds = (long)(dateTime - epoch).TotalSeconds;
            return seconds;
        }

        /// <summary>
        /// 将 Unix 时间戳（秒）还原为对应的本地 DateTime 实例。
        /// </summary>
        public static DateTime ToLocalDateTime(this long timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
        }

        /// <summary>
        /// 获取指定日期所在周的起始天（默认周一），并将其时间部分归零。
        /// </summary>
        public static DateTime GetStartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = ((int)dt.DayOfWeek - (int)startOfWeek + 7) % 7;
            return dt.Date.AddDays(-diff);
        }

        /// <summary>
        /// 判断两个日期是否属于不同的周（默认以周一为周的起点）。
        /// </summary>
        public static bool IsDifferentWeek(this DateTime dt, DateTime other, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return dt.GetStartOfWeek(startOfWeek) != other.GetStartOfWeek(startOfWeek);
        }
    }

/*
 * Unix 时间戳（Unix timestamp）定义为 从 1970 年 1 月 1 日 00:00:00 UTC 开始经过的秒数（不含闰秒）。
 * 这个起始时间正是 Unix 系统的纪元（Epoch），最初由早期的 Unix 开发者选定，并沿用至今，成为很多计算机系统的时间表示基准。
 */
}