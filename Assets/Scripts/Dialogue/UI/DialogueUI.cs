using System;
using System.Collections;
using System.Collections.Generic;
using MyFarm.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class DialogueUI : MonoBehaviour
{
   public GameObject dialogueBox;
   public Text dialogueText;
   public Image faceRight,faceLeft;
   public TextMeshProUGUI nameRight, nameLeft;
   public GameObject continueBox;
   [Header("Options")]
   public RectTransform optionsPanel;
   public OptionUI optionPrefab;
   private void Awake()
   {
      continueBox.SetActive(false);
   }

   private void OnEnable()
   {
      EventHandler.EndDialogEvent += EndDialog;
      EventHandler.ShowDialogEvent += UpdateDialogue;
   }

   private void OnDisable()
   {
      EventHandler.EndDialogEvent -= EndDialog;
      EventHandler.ShowDialogEvent -= UpdateDialogue;
   }

   public void EndDialog()
   {
      dialogueBox.SetActive(false);
   }
   
   public void UpdateDialogue(DialoguePiece piece)
   {
      StartCoroutine(ShowDialogue(piece));
   }
   private IEnumerator ShowDialogue(DialoguePiece piece)
   {
      if (piece!=null)
      {
         piece.isDone = false;
         dialogueBox.SetActive(true);
         continueBox.SetActive(false);
         dialogueText.text =String.Empty;
         if (piece.name != String.Empty)
         {
            //这里没判断有没有图片
            if (piece.onLeft)
            {
               faceLeft.gameObject.SetActive(true);
               faceRight.gameObject.SetActive(false);
               faceLeft.sprite = piece.faceImage;
               nameLeft.text = piece.name;
            }
            else
            {
               faceLeft.gameObject.SetActive(false);
               faceRight.gameObject.SetActive(true);
               faceRight.sprite = piece.faceImage;
               nameRight.text = piece.name;
            }
         }
         else
         {
            faceLeft.gameObject.SetActive(false);
            faceRight.gameObject.SetActive(false);
            nameLeft.gameObject.SetActive(false);
            nameRight.gameObject.SetActive(false);
         }

         yield return dialogueText.DOText(piece.dialogueText, 1f).WaitForCompletion();
         piece.isDone = true;
         if (piece.hasToPause&&piece.isDone&&piece.options.Count==0)
         {
            Debug.Log("有了");
            continueBox.SetActive(true);
         }
         else
         {
            continueBox.SetActive(false);
         }
         //创建选项
         CreateOptions(piece);
      }
      else
      {
         dialogueBox.SetActive(false);
         yield break;
      }
   }

   private void CreateOptions(DialoguePiece piece)
   {
      if (optionsPanel.childCount>0)
      {
         for (int i = 0; i < optionsPanel.childCount; i++)
         {
            Destroy(optionsPanel.GetChild(i).gameObject);
         }
      }

      for (int i = 0; i < piece.options.Count; i++)
      { 
         var option = Instantiate(optionPrefab, optionsPanel);
         option.UpdateOption(piece,piece.options[i]);
      }
   }
   
}
