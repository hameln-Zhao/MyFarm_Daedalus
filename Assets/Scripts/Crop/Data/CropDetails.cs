using UnityEngine;
//这里没有再做Editor去搞种子库
[System.Serializable]
public class CropDetails:Consumable
{
    public int CropID => ItemID;
    [Header("不同阶段需要的时间")] 
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }
    [Header("不同生长阶段的prefab")]
    public GameObject[] growthPrefabs;
    [Header("不同阶段的照片")]
    public Sprite[] growthSprites;
    [Header("可种植的季节")]
    public Season[] seasons;
    [Space] 
    [Header("收割工具")] 
    public int[] harvestToolItemID;//可能有多个可以收割的工具  我想用0表示空手
    [Header("每种工具使用次数")] 
    public int[] requireActionCount;//和上面的一一对应
    [Header("转换新物品的ID")] //树转换成树根 葡萄变成葡萄架子 那空的怎么办？
    public int transferItemID;
    [Space] 
    [Header("收割果实信息")] 
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius; //例如大树会倒在一个地方 植物一般就在原地
    [Header("再次生长时间")]
    public int daysToRegrow;
    public int regrowTimes;//由于初始化都是tile信息的初始化都是-1 这里的2就表示可以收割三次
    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;
    [Header("变异相关")] 
    public bool canMutate;//是否可以变异
    public int requiredMagicAmountForMutation;//进入变异生长所需要的魔素
    public int requiredMagicAmountForMutantCrop;//生产变异作物所需要的魔素
    public int[] mutantProducedItemID;//变异后作物ID；
    public int[] mutantProducedMinAmount;
    public int[] mutantProducedMaxAmount;
    public Sprite[] mutantSprites;
    
    //TODO:特效 音效等
    /// <summary>
    /// 检查当前工具是否可用
    /// </summary>
    /// <param name="toolID"></param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvestToolItemID)
        {
            if (tool==toolID)
            {
                return true;
            }
        }
        return false;
    }

    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if (harvestToolItemID[i]==toolID)
            {
                return requireActionCount[i];
            }
        }
        return -1;
    }
    /// <summary>
    /// 检查是否可以进入变异生长
    /// </summary>
    /// <param name="currentMagicAmount"></param>
    /// <returns></returns>
    public bool CanEnterMutation(int currentMagicAmount)
    {
        if (canMutate && currentMagicAmount >= requiredMagicAmountForMutation)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 检查是否能生产变异作物
    /// </summary>
    /// <param name="currentMagicAmount"></param>
    /// <returns></returns>
    public bool CanProduceMutantCrop(int currentMagicAmount)
    {
        if (currentMagicAmount >= requiredMagicAmountForMutantCrop)
        {
            return true;
        }
        return false;
    }
}
