using System;
using System.Collections;
using System.Collections.Generic;
using MyFarm.Inventory;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestBtnUI : MonoBehaviour
{
    public Text questNameText;
    public QuestData_SO currentData;

    private void Awake()
    {
       GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }

    void UpdateQuestContent()
    {
        EventHandler.CallUpdateQuestRequireEvent(currentData);

        foreach (var item in currentData.RewardItems)
        {
            //调用设置UI的函数
            EventHandler.CallUpdateQuestRewardEvent(InventoryManager.Instance.GetItemDetails(item.ItemID),item.ItemAmount);
        }
    }
    public void SetNameButton(QuestData_SO questData)
    {
        currentData=questData;
        if (questData.isCompleted)
        {
            questNameText.text=questData.questName+"(完成)";
        }
        else
        {
            questNameText.text=questData.questName;
        }
    }
}
