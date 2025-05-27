using System;
using System.Collections;
using System.Collections.Generic;
using MyFarm.Inventory;
using UnityEngine;

namespace MyFarm.CropPlant
{
    public class CropManager : Singleton<CropManager>
    {
        public CropDataList_SO CropData;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;
        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneEvent += OnAfterSceneEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.ApplyMagicEvent += OnApplyMagicEvent;
        }
        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneEvent -= OnAfterSceneEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.ApplyMagicEvent -= OnApplyMagicEvent;
        }
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
        }
        private void OnPlantSeedEvent(int itemID, TileDetails tileDetails)
        {
            CropDetails cropDetails = GetCropDetails(itemID);
            //场地空的，种子有的，季节是允许的
            if (cropDetails != null && SeasonAvailable(cropDetails)&& tileDetails.seedItemID==-1)
            {
                tileDetails.seedItemID = itemID;
                tileDetails.growthDays = 0;
                tileDetails.currentMagicAmount = 0;
                tileDetails.isMutated = false;
                //remove一下
                InventoryManager.Instance.RemoveItem(itemID,1);
                //显示农作物
                DisplayCropPlant(tileDetails,cropDetails);
            }else if (tileDetails.seedItemID!=-1)
            {
                //显示农作物
                DisplayCropPlant(tileDetails,cropDetails);
            }
        }
        /// <summary>
        /// 显示农作物
        /// </summary>
        /// <param name="tileDetails"></param>
        /// <param name="cropDetails"></param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            //成长阶段
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;
            //倒序计算成长阶段
            for (int i = growthStages-1; i >=0 ; i--)
            {
                if (tileDetails.growthDays>=dayCounter)
                {
                    currentStage = i;
                    break;
                }

                dayCounter -= cropDetails.growthDays[i];
            }
            //获取当前阶段的prefab
            GameObject cropPrefab=cropDetails.growthPrefabs[currentStage];
            //Sprite cropSprite = cropDetails.growthSprites[currentStage];
            Debug.Log(tileDetails.isMutated);
            Sprite cropSprite = tileDetails.isMutated ? cropDetails.mutantSprites[currentStage] : cropDetails.growthSprites[currentStage];
            Vector3 pos=new Vector3(tileDetails.gridX+0.5f,tileDetails.gridY+0.5f,0);
            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity,cropParent);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite=cropSprite;
            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;
        }
        /// <summary>
        /// 施加魔素时更新tiledetails
        /// </summary>
        /// <param name="tileDetails"></param>
        /// <param name="magicAmount"></param>
        private void OnApplyMagicEvent(TileDetails tileDetails, int magicAmount)
        {
            if (tileDetails != null && tileDetails.seedItemID != -1)
            {
                tileDetails.currentMagicAmount+=magicAmount;
                Debug.Log("当前植物魔素："+tileDetails.currentMagicAmount);
                CropDetails cropDetails = GetCropDetails(tileDetails.seedItemID);

                if (!tileDetails.isMutated && cropDetails != null &&
                    cropDetails.CanEnterMutation(tileDetails.currentMagicAmount))
                {
                    tileDetails.isMutated = true;
                }
                
                DisplayCropPlant(tileDetails, cropDetails);
            }
        }

        private void OnAfterSceneEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent=GameObject.FindWithTag("CropParent").transform;
        }
        /// <summary>
        /// 获取种子信息
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int itemID)
        {
            return CropData.CropDetailsList.Find(c=>c.CropID==itemID);
        }
        /// <summary>
        /// 判断当前季节是否可以种植
        /// </summary>
        /// <param name="cropDetails"></param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails cropDetails)
        {
            for (int i = 0; i < cropDetails.seasons.Length; i++)
            {
                if (currentSeason==cropDetails.seasons[i])
                {
                    return true;
                }
            }
            return false;
        }
        
    }

}