using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFarm.Dialogue
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("对话详情")] 
        public string ID;
        public string targetID;//这个ID需要对应跳转目标的ID,如果ID为空且没有选项 就证明是最后一句
        public Sprite faceImage;
        public bool onLeft;
        public string name;
        [TextArea] public string dialogueText;
        public bool hasToPause;//如果可以直接跳过对话就为false 不能就为true
        public bool isDone;
        public List<DialogOption> options = new List<DialogOption>();
        public QuestData_SO questData;
    }

}