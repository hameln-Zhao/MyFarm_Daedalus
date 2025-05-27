using System;
using System.Collections;
using System.Collections.Generic;
using MyFarm.Dialogue;
using UnityEngine;

namespace MyFarm.Dialogue
{
    [CreateAssetMenu(fileName = "DialogData_SO", menuName = "Dialog/DialogData")]

    public class DialogData_SO : ScriptableObject
    {
        public List<DialoguePiece> dialogPieces = new List<DialoguePiece>();
        public Dictionary<string, DialoguePiece> dialogDict= new Dictionary<string, DialoguePiece>();
        
        //一但数据在Unity窗口中被更改了 会调用
        private void OnValidate()
        {
            dialogDict.Clear();
            foreach (var piece in dialogPieces)
            {
                if (!dialogDict.ContainsKey(piece.ID))
                {
                    dialogDict.Add(piece.ID, piece);
                }
            }
        }
        /// <summary>
        /// 获取当前对话中的任务数据
        /// </summary>
        /// <returns></returns>
        public QuestData_SO GetQuestData()
        {
            QuestData_SO currentQuest=null;
            foreach (var piece in dialogPieces)
            {
                if (piece.questData!= null)
                {
                    currentQuest = piece.questData;
                    break;
                }
            }
            return currentQuest;
        }
    }
   
}
