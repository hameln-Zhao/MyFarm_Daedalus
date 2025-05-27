using System;
using System.Collections;
using System.Collections.Generic;
using MyFarm.Dialogue;
using MyFarm.Inventory;
using MyFarm.Map;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText;
    private Button button;
    private DialoguePiece currentPiece;
    private string nextPieceID;
    private bool takeQuest;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnOptionClicked);
    }

    public void UpdateOption(DialoguePiece piece, DialogOption option)
    {
        currentPiece = piece;
        optionText.text = option.text;
        nextPieceID = option.targetID;
        takeQuest = option.takeQuest;
    }

    public void OnOptionClicked()
    {
        if (currentPiece.questData!=null)
        {
            var newTask = new QuestManager.QuestTask
            {
                questData = Instantiate(currentPiece.questData)//复制一个so文件
            };
            if (takeQuest)
            {
                //当前任务列表有任务
                if (QuestManager.Instance.CheckQuestExist(newTask.questData))
                {
                    //判断是否完成 给予奖励
                    if (QuestManager.Instance.GetQuestTask(newTask.questData).IsComplete)
                    {
                        newTask.questData.GiveReward();
                        QuestManager.Instance.GetQuestTask(newTask.questData).IsFinished = true;
                    }
                }
                else//当前列表无任务
                {
                    //接受任务的时候判断是否已经完成
                    QuestManager.Instance.questTasks.Add(newTask);
                    QuestManager.Instance.GetQuestTask(newTask.questData).IsStarted = true;
                    if (!newTask.fromStart)//不是接受任务才开始记录，接受时就检查
                    {
                        foreach (var requireName in newTask.questData.RequireTargetName())
                        {
                            InventoryManager.Instance.CheckQuestItemInBag(requireName);//检查背包更新任务情况
                            //TODO:不在背包里的 自定义的数据 也要check
                            if (requireName=="锄地个数")
                            {
                                Debug.Log(GridMapManager.Instance.GetDigTileCount());
                                QuestManager.Instance.UpdateQuestProgress(requireName,GridMapManager.Instance.GetDigTileCount());
                            }
                        }
                    }
                   
                }
            }
        }
        if (nextPieceID=="")
        {
            EventHandler.CallEndDialogEvent();
            EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
            for (int i = 0; i < gameObject.transform.parent.childCount; i++)
            {
                Destroy(gameObject.transform.parent.GetChild(i).gameObject);
            }
        }
        else
        {
            //更新下一句话 调用事件 传入一个 ID 传入字典
            EventHandler.CallChangeDialogue(nextPieceID);
            //隐藏当前的Option
            for (int i = 0; i < gameObject.transform.parent.childCount; i++)
            {
                Destroy(gameObject.transform.parent.GetChild(i).gameObject);
            }
            
        }
    }
}
