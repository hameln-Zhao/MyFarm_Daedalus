using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;//通常用在列表和数组当中 帮助循环查找
public class QuestManager : Singleton<QuestManager>
{
    [System.Serializable]
    public class QuestTask
    {
        public QuestData_SO questData;
        public bool IsStarted { get { return questData.isStarted; } set { questData.isStarted = value; } }
        public bool IsComplete { get { return questData.isCompleted; } set { questData.isCompleted = value; } }
        public bool IsFinished { get { return questData.isFinished; } set { questData.isFinished = value; } }
        public bool fromStart { get { return questData.fromStart; } set { questData.fromStart = value; } }
        public bool isCommit { get { return questData.isCommit; } set { questData.isCommit = value; } }
    }
    public List<QuestTask> questTasks = new List<QuestTask>();
    //在更改瓦片 瓦片销毁 拾取物品 使用物品 等调用
    public void UpdateQuestProgress(string requireName, int amount)
    {
        foreach (var task in questTasks)
        {
            //找到包含这个物品的任务
            var matchTask=task.questData.questRequirements.Find(q => q.name == requireName);
            if (matchTask != null)
                matchTask.currentAmount+=amount;
            task.questData.CheckQuestProgress();
        }
    }
    /// <summary>
    /// 检查任务是否存在
    /// </summary>
    /// <param name="questData"></param>
    /// <returns></returns>
    public bool CheckQuestExist(QuestData_SO questData)
    {
        if (questData!=null)
        {
            return questTasks.Any(q=>q.questData.questName==questData.questName);
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 获取任务Data
    /// </summary>
    /// <param name="questData"></param>
    /// <returns></returns>
    public QuestTask GetQuestTask(QuestData_SO questData)
    {
        return questTasks.Find(q=>q.questData.questName==questData.questName);
    }
}
