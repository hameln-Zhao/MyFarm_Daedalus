using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
//挂载在NPC上 管控对话数据

namespace MyFarm.Dialogue
{
    public class DialogueContoller : MonoBehaviour
    {
        public UnityEvent OnFinishEvent;
        private GameObject uisign;
        public DialogData_SO currentDialogData;
        [SerializeField]
        private DialoguePiece currentPiece;
        private void Awake()
        {
            uisign = transform.GetChild(1).gameObject;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                DialogueManager.Instance.canTalk = true;
                DialogueManager.Instance.currentDialogData = currentDialogData;
                DialogueManager.Instance.uisign = uisign;
                DialogueManager.Instance.currentPiece = currentDialogData.dialogPieces[0];
                DialogueManager.Instance.OnFinishEvent = OnFinishEvent;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                DialogueManager.Instance.uisign .SetActive(false);
                DialogueManager.Instance.canTalk = false;
                DialogueManager.Instance.currentDialogData = null;
                DialogueManager.Instance.uisign = null;
                DialogueManager.Instance.currentPiece = null;
                DialogueManager.Instance.OnFinishEvent = null;
            }
        }
    }
}
