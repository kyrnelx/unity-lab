using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kyrnel
{
    public class Weight : MonoBehaviour
    {
        [Serializable]
        public class WeightItem
        {
            public string Name;
            public int Weight;
        }

        public List<WeightItem> Items;

        private void Awake()
        {
            Items = new List<WeightItem>()
            {
                new()
                {
                    Name = "橘子",
                    Weight = 25
                },
                new()
                {
                    Name = "菠萝",
                    Weight = 25
                },
                new()
                {
                    Name = "哈密瓜",
                    Weight = 50
                },
            };
        }

        private void Start()
        {
            Debug.Log(GetItem()?.Name);
            Debug.Log(GetItem()?.Name);
            Debug.Log(GetItem()?.Name);
            Debug.Log(GetItem()?.Name);
            Debug.Log(GetItem()?.Name);
        }

        public WeightItem GetItem()
        {
            int totalWeight = Items.Sum(item => item.Weight);
            int randomWeight = UnityEngine.Random.Range(0, totalWeight);
            int cumulativeWeight = 0;
            foreach (var item in Items)
            {
                cumulativeWeight += item.Weight;
                if (randomWeight < cumulativeWeight)
                    return item;
            }

            return null;
        }

        /*
         按权重随机抽取一个物品

         ========== 图解算法 ==========

         1. 计算总权重（例如 25+25+50 = 100）

         2. 随机整数 randomWeight ∈ [0, 100)   // 左闭右开，可取 0~99

         3. 累计权重 cumulativeWeight，依次划分子区间：

             橘子区间: [0, 25)     → 整数范围 0~24   (25个值)
             菠萝区间: [25, 50)    → 整数范围 25~49  (25个值)
             哈密瓜区: [50, 100)   → 整数范围 50~99  (50个值)

            ⭐ 判断条件 randomWeight < cumulativeWeight

            ┌─────────────────────────────────────────────────────┐
            │  橘子(25)   │   菠萝(25)    │       哈密瓜(50)       │
            └─────────────────────────────────────────────────────┘
            0            25              50                      100
            ↑            ↑               ↑                        ↑
            randomWeight=0~24 → 橘子      randomWeight=50~99 → 哈密瓜

         4. 遍历物品，累加权重，一旦 randomWeight < 当前累计值 → 命中

         ==========================================================
        */
    }
}