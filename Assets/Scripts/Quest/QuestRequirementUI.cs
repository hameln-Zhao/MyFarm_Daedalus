using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestRequirementUI : MonoBehaviour
{
    private TextMeshProUGUI requirementText;
    private TextMeshProUGUI progressText;

    private void Awake()
    {
        requirementText = GetComponent<TextMeshProUGUI>();
        progressText=transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void SetRequirement(string name,int amount,int currentAmount)
    {
        requirementText.text = name;
        progressText.text = currentAmount.ToString()+"/"+amount.ToString();
    }

    public void SetRequirement(string name, bool isFinished)
    {
        if (isFinished)
        {
            requirementText.text = name;
            progressText.text = "已完成";
            requirementText.color = Color.gray;
            progressText.color = Color.gray;
        }
    }
}
