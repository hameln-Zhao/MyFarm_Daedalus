using System;
using System.Collections;
using System.Collections.Generic;
using MyFarm.Inventory;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public GameObject questPanel;
    private bool isOpen;
    [Header("QuestBtn")]
    public RectTransform questListTransform;
    public QuestBtnUI questBtn;
    public TextMeshProUGUI questContentText;
    [Header("Requirement")]
    public RectTransform requirementTransform;
    public QuestRequirementUI requirement;
    [Header("Reward")]
    public RectTransform rewardTransform;
    public SlotUI rewardItem;

    private void OnEnable()
    {
        EventHandler.UpdateQuestRequireEvent += SetRequirementList;
        EventHandler.UpdateQuestRewardEvent += SetRewardItem;
    }

    private void OnDisable()
    {
        EventHandler.UpdateQuestRequireEvent -= SetRequirementList;
        EventHandler.UpdateQuestRewardEvent -= SetRewardItem;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            OpenQuest();
        }
    }

    private void OpenQuest()
    {
        isOpen=!isOpen;
        questPanel.SetActive(isOpen);
        if (isOpen)
        {
            questContentText.text = string.Empty;
            //显示面板内容
            SetQuestList();
        }
       
    }
    public void SetQuestList()
    {
        //先删掉所有的Quest
        foreach (Transform item in questListTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in requirementTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (var task in QuestManager.Instance.questTasks)
        {
            var nameBtn = Instantiate(questBtn, questListTransform);
            nameBtn.SetNameButton(task.questData);
        }
    }

    public void SetRequirementList(QuestData_SO questData)
    {
        questContentText.text = questData.description;
        foreach (Transform item in requirementTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (var questRequire in questData.questRequirements)
        {
            var q = Instantiate(requirement, requirementTransform);
            if (questData.isFinished)
            {
                q.SetRequirement(questRequire.name,questData.isFinished);
            }
            else
            {
                q.SetRequirement(questRequire.name,questRequire.requiredAmount,questRequire.currentAmount);
            }
        }

    }

    public void SetRewardItem(ItemDetails itemDetails, int amount)
    {
        foreach (Transform reward in rewardTransform)
        {
            Destroy(reward.gameObject);
        }
        var item = Instantiate(rewardItem, rewardTransform);
        item.UpdateSlot(itemDetails, amount);
    }
}
