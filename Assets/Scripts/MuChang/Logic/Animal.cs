using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public AnimalDetails animalDetails;

    [Header("动物状态")] 
    public int currentGrowthDays = 0;
    public int currentMagicAmount = 0;
    public int yieldCount = 0;   //已产出数量
    
    public int careActionCount;

    public void SpawnItem()
    {
        for (int i = 0; i < animalDetails.producedItemID.Length; i++)
        {
            int amountToProduce = 0;
            if (animalDetails.producedItemMaxAmount[i]==animalDetails.producedItemMinAmount[i])
            {
                amountToProduce = animalDetails.producedItemMaxAmount[i];
            }
            else
            {
                amountToProduce = Random.Range(animalDetails.producedItemMinAmount[i], animalDetails.producedItemMaxAmount[i]);
            }

            for (int j = 0; j < amountToProduce; j++)
            {
                if (animalDetails.generateAtPlayerPosition)
                {
                    //复用农作物的生成
                    EventHandler.CallHarvestAtPlayerPosition(animalDetails.producedItemID[i]);
                }
                else
                {
                    //在地图上生成
                }
            }
        }
    }
    //每日更新生长进度，由animaLManager调用
    public void UpdateGrowth()
    {
        if (currentGrowthDays < animalDetails.TotalGrowthDays)
        {
            currentGrowthDays++;
            UpdateAppearance();
        }
    }
    
    //外部施加魔素
    public void ApplyMagic(int magicAmount)
    {
        currentMagicAmount += magicAmount;
        //添加魔素后的加速生长
        
    }

    public void UpdateAppearance()
    {
        int stageCount = animalDetails.growthDays.Length;
        int stage = 0;
        int dayCounter = animalDetails.TotalGrowthDays;
        for (int i = stageCount - 1; i >= 0; i--)
        {
            if (currentGrowthDays >= dayCounter)
            {
                stage = i;
                break;
            }
            dayCounter -= animalDetails.growthDays[i];
        }
        Debug.Log("stage:"+stage+", dayCounter:"+dayCounter+", stageCount:"+stageCount);
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.sprite =  animalDetails.growthSprite[stage];
        
        //根据阶段调整体型
        float scaleFactor = 1f + 0.5f * stage;
        Debug.Log("体型变大,现在大小:"+scaleFactor);
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    public void DailyGrowthUpdate()
    {
        if (currentGrowthDays < animalDetails.TotalGrowthDays)
        {
            currentGrowthDays++;
            UpdateAppearance();
        }
    }
}
