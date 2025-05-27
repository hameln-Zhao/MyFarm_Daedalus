using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyFarm.Inventory;

[CreateAssetMenu(fileName = "QuestData_SO", menuName = "Quest/QuestData")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequire
    {
        public string name;
        public int requiredAmount;
        public int currentAmount;
    }
    public string questName;
    [TextArea]
    public string description;

    public bool isStarted;
    public bool isCompleted;
    public bool isFinished;
    public bool fromStart;//判断这个任务是否是从接受后才开始记录
    public bool isCommit;//判断这个任务的完成内容是否需要提交
    
    public List<QuestRequire> questRequirements=new List<QuestRequire>();
    public List<InventoryItem> RewardItems=new List<InventoryItem>();

    /// <summary>
    /// 查找任务是否结束
    /// </summary>
    public void CheckQuestProgress()
    {
        //查找所有满足条件的questRequire 如果和要求相等就完成
        var finishedQuests = questRequirements.Where(q => q.requiredAmount <= q.currentAmount);
        isCompleted=finishedQuests.Count()==questRequirements.Count;
        if (isCompleted)
        {
            
        }
    }
    /// <summary>
    /// 完成任务给予奖励
    /// </summary>
    public void GiveReward()
    {
        foreach (var reward in RewardItems)
        {
            //如果背包和actionBar是分开的逻辑的话 这里就要复杂判断了 小狗骑士教程里讲
            InventoryManager.Instance.UpdateRewardItem(reward);
        }   
    }
    public List<string> RequireTargetName()
    {
        List<string> targetNameList=new List<string>();
        foreach (var require in questRequirements)
        {
            targetNameList.Add(require.name);
        }

        return targetNameList;
    }
}
