using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyFarm.Dialogue
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        public DialogData_SO currentDialogData;
        public DialoguePiece currentPiece;
        public UnityEvent OnFinishEvent;
        public GameObject uisign;
        public bool canTalk;
        public bool isTalking;
     
        private void OnEnable()
        {
            EventHandler.ChangeDialogue += OnChangeDialogue;
        }

        private void OnDisable()
        {
            EventHandler.ChangeDialogue -= OnChangeDialogue;
        }

        private void OnChangeDialogue(string pieceID)
        {
            currentPiece = currentDialogData.dialogDict[pieceID];
            StartCoroutine(DialogueRoutine(currentPiece));
        }
        
        private void Update()
        {
            if (uisign != null)
            {
                uisign.SetActive(canTalk);
            }
            if (currentDialogData != null && canTalk && Input.GetKeyDown(KeyCode.Space) && !isTalking)
            {
                StartCoroutine(DialogueRoutine(currentPiece));
            }
        }

        private void ResetDialogueList()
        {
            for (int i = currentDialogData.dialogPieces.Count - 1; i > -1; i--)
            {
                currentDialogData.dialogPieces[i].isDone = false;
            }
        }

        private IEnumerator DialogueRoutine(DialoguePiece dialoguePiece)
        {
            isTalking = true;
            if (dialoguePiece != null)
            {
                //传到UI显示对话
                //有选项会自动执行跳转的功能
                if (dialoguePiece.options.Count > 0)
                {
                    EventHandler.CallShowDialogEvent(dialoguePiece);
                    EventHandler.CallUpdateGameStateEvent(GameState.Pause);
                }
                else //没选项不会触发
                {
                    EventHandler.CallShowDialogEvent(dialoguePiece);
                    EventHandler.CallUpdateGameStateEvent(GameState.Pause);
                    UpdateNextPiece(); //自动更新下一条piece
                }

                yield return new WaitUntil(() => dialoguePiece.isDone);
                isTalking = false;
            }
            else //对话说完
            {
                EventHandler.CallShowDialogEvent(null);
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
                ResetDialogueList(); //重新补充
                isTalking = false;
                if (OnFinishEvent != null)
                {
                    OnFinishEvent?.Invoke();
                    canTalk = false;
                }
            }
        }
        /// <summary>
        /// 根据targetID更新下一句话
        /// </summary>
        private void UpdateNextPiece()
        {
            //没有选项且无targetID的情况 对话结束
            if (currentPiece.targetID == string.Empty)
            {
                currentPiece = null; //不知道会不会立马更新
            }
            else
            {
                currentPiece = currentDialogData.dialogDict[currentPiece.targetID];
            }
        }
    }
}