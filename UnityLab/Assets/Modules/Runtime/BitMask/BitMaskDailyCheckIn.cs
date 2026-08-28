using System;

namespace Kyrnel
{
    /// <summary>
    /// 基于位掩码的每日签到系统（支持最多 32 天）
    /// 使用 int 的 32 个二进制位，每一位代表一天的签到状态（1=已签，0=未签）
    /// </summary>
    public class BitMaskDailyCheckIn
    {
        // 签到位掩码（第0位表示第1天，第1位表示第2天，...）
        private int checkinMask = 0;

        /// <summary>
        /// 签到指定天（从1开始计数）
        /// </summary>
        /// <param name="day">天数（1～32）</param>
        public void CheckIn(int day)
        {
            if (day < 1 || day > 32)
            {
                Console.WriteLine($"天数 {day} 无效，允许范围 1~32");
                return;
            }

            // 计算需要设置的位：第 (day-1) 位
            int bitMask = 1 << (day - 1);
            // 按位或操作，将该位置为1，不影响其他位
            checkinMask |= bitMask;
            Console.WriteLine($"签到成功：第 {day} 天");
        }

        /// <summary>
        /// 检查某天是否已签到
        /// </summary>
        public bool IsCheckedIn(int day)
        {
            if (day < 1 || day > 32) return false;
            int bitMask = 1 << (day - 1);
            return (checkinMask & bitMask) != 0;
        }

        /// <summary>
        /// 获取连续签到天数（从第1天开始连续不断的天数）
        /// </summary>
        public int GetContinuousDays()
        {
            int count = 0;
            for (int day = 1; day <= 32; day++)
            {
                if (IsCheckedIn(day))
                    count++;
                else
                    break;
            }

            return count;
        }
    }
}