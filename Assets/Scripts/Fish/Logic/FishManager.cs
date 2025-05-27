using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyFarm.Map;
using MyFarm.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using MyFarm.Inventory;
//存储所有钓鱼逻辑

public class FishManager : Singleton<FishManager>
{
    public FishDataList_SO fishData;
    public bool isThrowing;//用于判断当前动画执行的阶段
    public FishState fishGameState = FishState.Empty;//记录钓鱼的状态
    public Transform currentFishFLoatingTrans;
    public FishDetails currentFish;
    public Transform fishLineStartTrans;
    private Season currentSeason;
    private Weather currentWeather;//目前还未添加天气系统
    private float waitTime;
    private TileDetails fishTile;
    private ItemDetails currentTool;
    private Coroutine fishingCoroutine;
    private InventoryItem fishItem;

    List<FishDetails> possibleFish = new List<FishDetails>();
    List<FishDetails> possibleGarbage = new List<FishDetails>();
    Dictionary<int,List<FishDetails>> currentRareFishList = new Dictionary<int,List<FishDetails>>();
    Dictionary<int,List<FishDetails>> currentCommonFishList = new Dictionary<int,List<FishDetails>>();
    private Dictionary<int, float> levelProbabilities = new Dictionary<int, float>()
    {
        {1, 0.8f},  // level1初始概率80%
        {2, 0.15f}, // level2初始概率15%
        {3, 0.05f}  // level3初始概率5%
    };
    private const float ScaleFactor = 2;//放大因子默认为2
    private const float MinLevel1Probability = 0.1f; // level1最低保留概率
    //鱼竿影响的映射
    private Dictionary<int, float> ToolIDEffectMap = new Dictionary<int, float>
    {
        { 1004, 0.1f }, 
    };
    private Dictionary<int, int> ToolIDLevelMap = new Dictionary<int, int>
    {
        { 1004, 1 }, 
    };
    
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 5f;
    private void OnEnable()
    {
        EventHandler.StartingFishing += OnStartingFishing;
        EventHandler.FinishFishing += OnFinishFishing;
        EventHandler.GameDayEvent += OnGameDayEvent;
    }
    private void OnDisable()
    {
        EventHandler.StartingFishing -= OnStartingFishing;
        EventHandler.FinishFishing -= OnFinishFishing;
        EventHandler.GameDayEvent -= OnGameDayEvent;
    }

    private void OnGameDayEvent(int day, Season season)
    {
        currentSeason = season;
    }

    /// <summary>
    /// 进入钓鱼状态
    /// 结束状态：1、鼠标左键点击 2、进入钓鱼小游戏
    /// </summary>
    private void OnStartingFishing(Transform fishFLoatingTrans)
    {
        currentFishFLoatingTrans = fishFLoatingTrans;
        fishTile = GridMapManager.Instance.GetTileDetailsByItemPosition(fishFLoatingTrans);
        if (fishTile == null)
        {
            EventHandler.CallFinishFishing();
            return;
        }
        if (fishTile.canFish)
        {
            fishGameState = FishState.WaitForFish;
            //进入等待鱼上钩的阶段
            fishingCoroutine = StartCoroutine(WaitForFish());
            //得到最终的鱼
            if (CalculateProbability())
            {
                Debug.Log("有鱼要上钩");
                GetProFishType(fishTile);//获取可能上钩的鱼的种类
                currentFish = GetFinalFish(); // 确定最终钓到的鱼
    
                if (currentFish != null)
                {
                    Debug.Log($"目前正在钓: {currentFish.ItemName} (等级 {currentFish.level})(稀有度 {currentFish.rarity})");
                }
            }
            else //垃圾或宝箱
            {
                Debug.Log("垃圾或宝箱来咯");
                GetProGarbageType(fishTile);
                currentFish = GetFinalGarbage(); // 确定最终钓到的垃圾
                
                if (currentFish != null)
                {
                    Debug.Log($"钓到了: {currentFish.ItemName} (等级 {currentFish.level})");
                }
            }
        }
        else
        {
           EventHandler.CallFinishFishing();
        }
    }
    #region 判断鱼上钩的条件
    /// <summary>
    /// 计算鱼是否上钩的函数
    /// </summary>
    /// <returns></returns>
    public bool CalculateProbability()
    {
        float skilleffect = 0f;
        //获取人物当前身上的buff（buff这个可以做成像技能一样的，由player管理一个总值，然后在这获取到）
        //获取当前的钓竿与影响值
        currentTool = CursorManager.Instance.currentItem;
        ToolIDEffectMap.TryGetValue(currentTool.ItemID, out float toolEffect);
        //获取距离岸边的距离与影响值
        float distanceEffect = CalculateDistanceEffect(GridMapManager.Instance.GetDistanceToShore(fishTile));
        //获取人物技能
        if (SkillManager.Instance.GetSkillBySkillType<BasicFishingSkill>(SkillType.BasicFishing)!=null)
        {
            skilleffect=SkillManager.Instance.GetSkillBySkillType<BasicFishingSkill>(SkillType.BasicFishing).probabilityUp;
        }
       
        //总的计算公式 这里假设相加起来
        float totalProbability = distanceEffect+skilleffect+toolEffect;
        /*Debug.Log("鱼竿加成"+toolEffect);
        Debug.Log("距离加成"+distanceEffect);
        Debug.Log("技能加成"+skilleffect);    
        Debug.Log("鱼上钩总概率"+totalProbability);*/
        return ProbabilityHelper.GetProbabilityResult(totalProbability);
    }
    #endregion
    #region 计算具体鱼的过程

     /// <summary>
    /// 计算综合影响因子（整合技能、buff、距离等）
    /// </summary>
    private float CalculateTotalInfluenceFactor()
    {
        float totalFactor = 0f;
    
        // 1. 鱼竿基础影响
        ToolIDEffectMap.TryGetValue(currentTool.ItemID, out float toolEffect);
        totalFactor += toolEffect;
    
        // 2. 距离影响
        totalFactor += CalculateDistanceEffect(GridMapManager.Instance.GetDistanceToShore(fishTile));
    
        // 3. 技能影响
        if (SkillManager.Instance.GetSkillBySkillType<BasicFishingSkill>(SkillType.BasicFishing) != null)
        {
            totalFactor += SkillManager.Instance.GetSkillBySkillType<BasicFishingSkill>(SkillType.BasicFishing).probabilityUp;
        }
        Debug.Log("最终的影响因子"+totalFactor);
        return Mathf.Clamp(totalFactor, 0f, 1f); // 限制在0-1范围内
    }
    /// <summary>
    /// 根据鱼竿影响因子调整各级别鱼的概率
    /// </summary>
    private Dictionary<int, float> GetAdjustedLevelProbabilities()
    {
        float totalFactor = CalculateTotalInfluenceFactor();
        float effectiveFactor = totalFactor * 2f; // 放大因子
        Debug.Log("effectiveFactor"+effectiveFactor);
        Dictionary<int, float> adjustedProbs = new Dictionary<int, float>();
    
        // 计算从level1转移的概率量
        float transferred = levelProbabilities[1] * effectiveFactor;
        transferred = Mathf.Min(transferred, levelProbabilities[1] - MinLevel1Probability);
    
        // 计算新概率
        adjustedProbs[1] = levelProbabilities[1] - transferred;
        adjustedProbs[2] = levelProbabilities[2] + transferred * 0.75f;
        adjustedProbs[3] = levelProbabilities[3] + transferred * 0.25f;
        // 输出调整后的概率（调试用）
        /*Debug.Log($"调整后的概率分布 - " +
                  $"Level1: {adjustedProbs[1]:P1} " +
                  $"(原始: {levelProbabilities[1]:P1}), " +
                  $"Level2: {adjustedProbs[2]:P1} " +
                  $"(原始: {levelProbabilities[2]:P1}), " +
                  $"Level3: {adjustedProbs[3]:P1} " +
                  $"(原始: {levelProbabilities[3]:P1})");
    
        // 输出影响因素详情
        Debug.Log($"影响因素详情 - " +
                  $"总因子: {totalFactor:F2}, " +
                  $"有效因子(×2): {effectiveFactor:F2}, " +
                  $"转移量: {transferred:P1}");*/
        return adjustedProbs;
    }
    /// <summary>
    /// 根据调整后的概率随机选择鱼的等级
    /// </summary>
    private int GetRandomFishLevel(float factor)
    {
        var adjustedProbs = GetAdjustedLevelProbabilities();
    
        float randomValue = UnityEngine.Random.value;
    
        if (randomValue < adjustedProbs[1]) return 1;
        else if (randomValue < adjustedProbs[1] + adjustedProbs[2]) return 2;
        else return 3;
    }
    /// <summary>
    /// 降级处理，当目标等级没有鱼时尝试低一级，如果降到只有垃圾也可以
    /// </summary>
    private void GetProFishTypeWithFallback(TileDetails currentTile, int fallbackLevel)
    {
        foreach (var fish in fishData.fishDataList)
        {
            if (fish.level != fallbackLevel)
                continue;

            if (ConditionsFilter(currentTile, fish))
            {
                possibleFish.Add(fish);
            }
        }
    
        // 如果还是没有，继续降级
        if (possibleFish.Count == 0 && fallbackLevel > 1)
        {
            GetProFishTypeWithFallback(currentTile, fallbackLevel - 1);
        }
    }


    #endregion
    #region 获取可以钓起来的鱼列表
    /// <summary>
    /// 根据当前瓦片信息+时间+季节+天气 以及随机概率来获取会有什么等级的鱼上钩，再从中选
    /// 概率根据上钩的鱼排列
    /// 如果这样写会不会到时候鱼很多，开销会很大呢？？
    /// </summary>
    private void GetProFishType(TileDetails currentTile)
    {
        possibleFish.Clear();
        // 获取当前鱼竿的影响因子
        ToolIDEffectMap.TryGetValue(currentTool.ItemID, out float toolEffect);
        // 根据影响因子随机选择鱼的等级
        int targetLevel = GetRandomFishLevel(toolEffect);
        foreach (var fish in fishData.fishDataList)
        {
            //跳过垃圾
            if (fish.level==0)
                continue;
            // 只考虑目标等级的鱼
            if (fish.level != targetLevel)
                continue;
            if (ConditionsFilter(currentTile, fish))
            {
                possibleFish.Add(fish);
            }
        }
        // 如果没有符合条件的鱼，降级处理
        if (possibleFish.Count == 0 && targetLevel > 1)
        {
            GetProFishTypeWithFallback(currentTile, targetLevel - 1);
            return;
        }
        /*possibleFish.ForEach(fish => 
            Debug.Log($"Name: {fish.ItemName}, Rarity: {fish.rarity}"));*/
    }
    /// <summary>
    /// 获取可以钓上来的垃圾
    /// </summary>
    /// <param name="currentTile"></param>
    private void GetProGarbageType(TileDetails currentTile)
    { 
        possibleFish.Clear();
        foreach (var fish in fishData.fishDataList)
        {
            if (fish.level>0)
                continue;
            if (ConditionsFilter(currentTile, fish))
            {   
                // 如果所有条件都满足，加入可能列表
                possibleGarbage.Add(fish);
            }
         
        }
        /*possibleGarbage.ForEach(fish => 
            Debug.Log($"Name: {fish.ItemName}, Rarity: {fish.rarity}"));*/
    }
    #endregion
    #region 得到最终钓起的鱼

    /// <summary>
    /// 获取最终上钩的鱼
    /// </summary>
    public FishDetails GetFinalFish()
    {
        if (possibleFish.Count == 0)
        {  
            Debug.Log("没有适合的鱼");
            return null;//可能直接给个垃圾就行/或者宝箱？
        }
    
        // 按等级分组,生成为一个等级+鱼列表的字典
        currentRareFishList = possibleFish
            .Where(f => f.level > 0 && f.rarity == Rarity.Rare)
            .GroupBy(f => f.level)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        currentCommonFishList = possibleFish
            .Where(f => f.level > 0 && f.rarity == Rarity.Common)
            .GroupBy(f => f.level)
            .ToDictionary(g => g.Key, g => g.ToList());
        // 获取调整后的等级概率
        var levelProbs = GetAdjustedLevelProbabilities();
    
        // 过滤掉概率为0的等级，并过滤掉没有鱼的等级。转换为等级->概率的键值对
        var availableLevels = levelProbs.Where(kv => kv.Value > 0 && currentCommonFishList.ContainsKey(kv.Key)).ToList();
    
        // 如果没有可用等级，返回null
        if (availableLevels.Count == 0) return null;//返回垃圾或者宝箱？
    
        // 计算总概率用于归一化
        float totalProb = availableLevels.Sum(x => x.Value);
    
        // 随机选择等级
        float randomValue = UnityEngine.Random.Range(0f, totalProb);
        float cumulative = 0f;
        int selectedLevel = 1;
        //通过遍历来确定随机数属于哪个区间，进而决定等级
        foreach (var levelProb in availableLevels)
        {
            cumulative += levelProb.Value;
            if (randomValue <= cumulative)
            {
                selectedLevel = levelProb.Key;
                break;
            }
        }
        // 从选定等级的鱼中随机选择一条
        var fishInLevel = currentCommonFishList[selectedLevel];
        return fishInLevel[UnityEngine.Random.Range(0, fishInLevel.Count)];
    }
    /// <summary>
    /// 获取随机垃圾
    /// </summary>
    /// <returns></returns>
    private FishDetails GetFinalGarbage()
    {
        return possibleGarbage[Random.Range(0, possibleGarbage.Count)];
    }

    #endregion
    #region 其他函数

    public FishDetails GetFishDetails(int itemID)
    {
        return fishData.fishDataList.Find(c=>c.ItemID==itemID);
    }

    public int GetFishToolLevel()
    {
        return  ToolIDLevelMap[currentTool.ItemID];
    }

    #endregion
    #region 影响函数
    /// <summary>
    /// 计算距离影响
    /// </summary>
    /// <param name="distance">距离岸边的距离</param>
    /// <returns></returns>
    private float CalculateDistanceEffect(int distance)
    {
        if (Compare.IsBetween(distance, 1, 3))
        {
            return 0.1f;
        }
        else if (Compare.IsBetween(distance, 4, 6))
        {
            return 0.2f;
        }
        else
        {
            return 0f;
        }
    }

    #endregion
    #region 季节+时间+条件判断

    /// <summary>
    /// 判断当前季节是否有此鱼
    /// </summary>
    /// <param name="fishDetails"></param>
    /// <returns></returns>
    private bool SeasonAvailable(FishDetails fishDetails)
    {
        
        for (int i = 0; i < fishDetails.seasons.Length; i++)
        {
            if (currentSeason == fishDetails.seasons[i])
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 判断当前时间段是否有此鱼
    /// </summary>
    /// <param name="fishDetails"></param>
    /// <returns></returns>
    private bool TimeAvailable(FishDetails fishDetails)
    {
        if (fishDetails.spawnTimeRanges == null || fishDetails.spawnTimeRanges.Length == 0)
            return false;
        int currentMinutes = TimeManager.Instance.GameTimeByMinutes;
        foreach (var timeRange in fishDetails.spawnTimeRanges)
        {
            // 直接转换HHmm格式为分钟数
            int startMinutes = (timeRange.StartTime / 100) * 60 + (timeRange.StartTime % 100);
            int endMinutes = (timeRange.EndTime / 100) * 60 + (timeRange.EndTime % 100);
            // 处理跨天情况（如2300-0100）
            if (endMinutes < startMinutes)
            {
                if (currentMinutes >= startMinutes || currentMinutes <= endMinutes)
                    return true;
            }
            // 正常时间范围
            else if (currentMinutes >= startMinutes && currentMinutes <= endMinutes)
            {
                return true;
            }
        }
        return false;
    }
    private bool ConditionsFilter(TileDetails currentTile,FishDetails fish)
    {
        // 检查水域类型
        if (!fish.fieldID.Contains(currentTile.filedType))
            return false;
        // 检查季节
        if (!SeasonAvailable(fish))
            return false;
        // 检查时间范围
        if(!TimeAvailable(fish))
            return false;
        /*// 检查天气
        if (!fish.PreferredWeather.Contains(currentWeather))
            continue;*/
        // 检查鱼竿
        if (!fish.fishingToolItemID.Contains(currentTool.ItemID))
            return false;
        return true;
    }

    #endregion
    
    /// <summary>
    /// TODO:根据当前钓鱼等级 有无鱼饵 来决定等待时间
    /// </summary>
    private IEnumerator WaitForFish()
    {
        //可能需要根据各种因素来决定等待的时间，目前直接给随机数
        float currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
        Debug.Log("预计等待"+currentWaitTime);
        // 等待currentWaitTime秒
        float timer = 0f;
        while (timer < currentWaitTime)
        {
            timer += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(" 玩家收钩了");
                EventHandler.CallFinishFishing();
                yield break; // 立即终止协程
            }
        
            yield return null; // 每帧检测
        }
        EventHandler.CallGoFishing();
    }
    /// <summary>
    /// 成功钓鱼后将鱼加入背包
    /// </summary>
    public void AddFishToBag()
    {
        fishItem.ItemID = currentFish.ItemID;
        fishItem.ItemAmount = 1;//默认设定每次只能钓起一条鱼
        InventoryManager.Instance.UpdateRewardItem(fishItem);
    }

    public void ChangePerfectFish()
    {
      var rareFish = currentRareFishList.Values
            .SelectMany(fishList => fishList)
            .FirstOrDefault(fish => fish.ItemName == currentFish.ItemName);
      if (rareFish!=null)
      {
          Debug.Log("成功转换了");
      }
      else
      {
          Debug.Log("好像没有捏");
      }
      
      currentFish = rareFish ?? currentFish;
      
       Debug.Log("获取到稀有的鱼了"+ currentFish.rarity);
    }
    /// <summary>
    /// 用于结束钓鱼的逻辑
    /// </summary>
    private void OnFinishFishing()
    {
        Debug.Log("调用结束逻辑");
        if (fishingCoroutine != null)
        {
            StopCoroutine(fishingCoroutine);
            fishingCoroutine = null;
        }
    
        Destroy(currentFishFLoatingTrans?.gameObject);
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
       
        fishGameState = FishState.Empty;
    }

    
}
