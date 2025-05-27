using UnityEngine;
[System.Serializable]
public struct TimeRange
{
    public int StartTime; // 开始时间
    public int EndTime;   // 结束时间
}
[System.Serializable]
public struct RangeFloat
{
    public float Min;
    public float Max;
}
[System.Serializable]
public class FishDetails : Consumable
{
    [Header("可以出现的区域")]
    public int[] fieldID;
    
    [Header("可以出现的季节")]
    public Season[] seasons;
    [Header("可以出现的天气")]
    public Weather[] weathers;
    [Header("出现的时间范围")]
    public TimeRange[] spawnTimeRanges; // 输入1030表示10:30
    
    [Header("对应的方案")] 
    public int[] schemes;
    [Header("这条鱼对应的等级")] 
    public int level;
    public int difficultyLevel; //等级和难度等级分开 可能有些低等级的鱼难钓

    [Space]
    [Header("钓鱼工具")] 
    public int[] fishingToolItemID;
    
    [Space] 
    [Header("钓上鱼的数量")] 
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    
    [Header("鱼的重量")] 
    public RangeFloat weightRange;

    [Header("使用")] 
    public int healthValue;
    
    [Header("Options")]
    public bool generateAtPlayerPosition;
    
    [Header("鱼上钩的难度")]
    public bool canDash = false;          // 是否会冲刺
    public float dashProbability = 0.1f;  // 冲刺概率(每秒)
    public float dashSpeedMultiplier = 1.5f; // 冲刺时速度倍数
    public float dashDuration = 1f;       // 冲刺持续时间(秒)
    
    // 检查当前时间是否可以钓到这种鱼
    public bool IsAvailableAtTime(int currentTime)
    {
        if (spawnTimeRanges == null || spawnTimeRanges.Length == 0)
            return true;
            
        foreach (var timeRange in spawnTimeRanges)
        {
            if (currentTime >= timeRange.StartTime && currentTime <= timeRange.EndTime)
                return true;
        }
        return false;
    }
    
    // 检查当前季节是否可以钓到这种鱼
    public bool IsAvailableInSeason(Season currentSeason)
    {
        if (seasons == null || seasons.Length == 0)
            return true;
            
        return System.Array.Exists(seasons, season => season == currentSeason);
    }
    
    // 获取随机重量
    public float GetRandomWeight()
    {
        return Random.Range(weightRange.Min, weightRange.Max);
    }
    
    // 获取随机钓获数量
    public int GetRandomProducedAmount(int toolIndex)
    {
        if (producedMinAmount == null || producedMaxAmount == null || 
            toolIndex < 0 || toolIndex >= producedMinAmount.Length)
            return 1;
            
        return Random.Range(producedMinAmount[toolIndex], producedMaxAmount[toolIndex] + 1);
    }
}
