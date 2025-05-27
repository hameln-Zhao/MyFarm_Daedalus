using System;
using System.Collections;
using System.Collections.Generic;
using MyFarm.Dialogue;
using UnityEngine;
[RequireComponent(typeof(DialogueContoller))]
public class QuestGiver : MonoBehaviour
{
    private DialogueContoller contoller;
    QuestData_SO currentQuest;
    public DialogData_SO startDialogue;
    public DialogData_SO progressDialogue;
    public DialogData_SO completeDialogue;
    public DialogData_SO finishDialogue;

    #region 获得任务状态
    public bool IsStarted
    {
        get
        {
            if (QuestManager.Instance.CheckQuestExist(currentQuest))
            {
                return QuestManager.Instance.GetQuestTask(currentQuest).IsStarted;
            }
            else return false;
        }
    }
    public bool IsComplete
    {
        get
        {
            if (QuestManager.Instance.CheckQuestExist(currentQuest))
            {
                return QuestManager.Instance.GetQuestTask(currentQuest).IsComplete;
            }
            else return false;
        }
    }
    public bool IsFinished
    {
        get
        {
            if (QuestManager.Instance.CheckQuestExist(currentQuest))
            {
                return QuestManager.Instance.GetQuestTask(currentQuest).IsFinished;
            }
            else return false;
        }
    }
    #endregion
    private void Awake()
    {
        contoller = GetComponent<DialogueContoller>();
    }

    private void Start()
    {
        contoller.currentDialogData= startDialogue;
        currentQuest = contoller.currentDialogData.GetQuestData();
    }

    private void Update()
    {
        if (IsStarted)
        {
            if (IsComplete)
            {
                contoller.currentDialogData = completeDialogue;
            }
            else
            {
                contoller.currentDialogData = progressDialogue;
            }
        }

        if (IsFinished)
        {
            contoller.currentDialogData = finishDialogue;
        }
    }
}
