using System;

namespace Kyrnel
{
    /// <summary>
    /// 伤害类型（位掩码枚举）
    /// [Flags] 特性表示该枚举支持位运算组合
    /// </summary>
    [Flags]
    public enum DamageType
    {
        None    = 0,           // 0b_0000_0000
        Damage1 = 1 << 1,      // 0b_0000_0010  (十进制2)
        Damage2 = 1 << 2,      // 0b_0000_0100  (十进制4)
        Damage3 = 1 << 3,      // 0b_0000_1000  (十进制8)
        Damage4 = 1 << 4,      // 0b_0001_0000  (十进制16)
    }
    
    public class BitMaskDamageImmunity
    {
        // 当前免疫掩码，初始无免疫
        private DamageType _immunityDamageMask = DamageType.None;

        /// <summary>
        /// 添加免疫伤害类型（按位或操作）
        /// 
        /// 图解示例：
        ///   当前 _immunityDamageMask = DamageType.Damage1 (二进制 0010)
        ///   添加 DamageType.Damage2 (二进制 0100)
        /// 
        ///   0010  (已有 Damage1)
        /// | 0100  (新添加 Damage2)
        /// -----
        ///   0110  (结果：同时免疫 Damage1 和 Damage2)
        /// </summary>
        /// <param name="damageType">要添加的伤害类型（可组合）</param>
        public void AddImmunityDamage(DamageType damageType)
        {
            _immunityDamageMask |= damageType;
        }

        /// <summary>
        /// 移除免疫伤害类型（按位与、取反操作）
        /// 
        /// 图解示例：
        ///   当前 _immunityDamageMask = DamageType.Damage1 | DamageType.Damage2 (二进制 0110)
        ///   移除 DamageType.Damage2 (二进制 0100)
        /// 
        ///   ~0100 = 1011
        ///   0110 & 1011 = 0010 (只剩 Damage1)
        /// </summary>
        /// <param name="damageType">要移除的伤害类型</param>
        public void RemoveImmunityDamage(DamageType damageType)
        {
            _immunityDamageMask &= ~damageType;
        }

        /// <summary>
        /// 检查是否免疫指定的伤害类型
        /// 
        /// 图解示例1（免疫）：
        ///   _immunityDamageMask = 0110 (免疫 Damage1 和 Damage2)
        ///   检查 DamageType.Damage2 (0100)
        ///   0110 & 0100 = 0100 != 0 → 免疫 ✅
        /// 
        /// 图解示例2（不免疫）：
        ///   _immunityDamageMask = 0110
        ///   检查 DamageType.Damage3 (1000)
        ///   0110 & 1000 = 0000 == 0 → 不免疫 ❌
        /// 
        /// 图解示例3（组合检查）：
        ///   检查 DamageType.Damage1 | DamageType.Damage3 (0010 | 1000 = 1010)
        ///   0110 & 1010 = 0010 != 0 → 免疫（因为包含 Damage1）
        /// </summary>
        /// <param name="damageType">要检查的伤害类型（可组合，任意位匹配即免疫）</param>
        /// <returns>如果免疫掩码中包含 damageType 的任意位则返回 true</returns>
        public bool IsImmunityDamage(DamageType damageType)
        {
            return (_immunityDamageMask & damageType) != 0;
        }
    }
}