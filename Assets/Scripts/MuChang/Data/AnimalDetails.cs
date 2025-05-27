using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AnimalDetails:Pet
{
   public int AnimalID;

   [Header("各阶段所需天数")] 
   public int[] growthDays;
   public int TotalGrowthDays
   {
      get
      {
         int total = 0;
         foreach (var days in growthDays)
         {
            total += days;
         }
         return total;
      }
   }
   
   //[Header("各个生长阶段的prefab")]
   //public GameObject[] growthPrefab;
   
   [Header("各个生长阶段的图标")]
   public Sprite[] growthSprite;

   [Header("产物信息")]  //需要数组吗？
   public int[] producedItemID;
   public int[] producedItemMinAmount;
   public int[] producedItemMaxAmount;

   [Header("重复产出设置")] 
   public int daysToRegrow;
   public int regrowTimes;

   [Header("其他设置")] 
   public bool generateAtPlayerPosition;
   public bool hasAnimation;
   public bool hasParticalEffect;

   [Header("魔素相关")] 
   public bool canMutate;
   public int speedUpDays;

   
   

}
