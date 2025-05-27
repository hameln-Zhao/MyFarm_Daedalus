using System;

namespace MyFarm.Utilities
{
    public class Compare
    {
        public static bool IsBetween(int value, int min, int max)
        {
            return value >= min && value <= max;
        }
    }
    /// <summary>
    /// 输入概率，返回是否成功
    /// </summary>
    public class ProbabilityHelper
    {
        private static readonly Random random = new Random();

        public static bool GetProbabilityResult(float totalProbability)
        {
            // 确保概率在 [0, 1] 范围内
            if (totalProbability < 0f)
                totalProbability = 0f;
            else if (totalProbability > 1f)
                totalProbability = 1f;

            // 生成一个 [0, 1) 范围内的随机数
            float randomValue = (float)random.NextDouble();

            // 如果随机数小于等于概率，返回 true
            return randomValue <= totalProbability;
        }
    }
}