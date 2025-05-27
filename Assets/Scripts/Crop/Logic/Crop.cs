using System.Collections;
using System.Collections.Generic;
using MyFarm.Inventory;
using UnityEngine;
//收割的所有逻辑
public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    private int harvestActionCount;
    private TileDetails tileDetails;
    public void ProcessToolAction(ItemDetails tool,TileDetails tileDetails)
    {
        this.tileDetails=tileDetails;
        //工具使用次数
        int requirementCount = cropDetails.GetTotalRequireCount(tool.ItemID);
        if (requirementCount==-1)return;
        //判断是否有动画和树木
        //点击计数器
        if (harvestActionCount<requirementCount)
        {
            harvestActionCount++;
            //播放粒子效果
            //播放声音
        }

        if (harvestActionCount==requirementCount)
        {
            if (cropDetails.generateAtPlayerPosition)
            {
                //生成在人物头顶
                //生成农作物
                SpawnHarvestItems();
            }
        }
    }
    /// <summary>
    /// 生成农作物
    /// </summary>
    public void SpawnHarvestItems()
    {
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduce = 0;
            if (cropDetails.producedMaxAmount[i]==cropDetails.producedMinAmount[i])
            {//代表只生成指定数量的
                amountToProduce=cropDetails.producedMaxAmount[i];
            }
            else
            {//随机数量
                amountToProduce=Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i]+1);
            }
            QuestManager.Instance.UpdateQuestProgress(InventoryManager.Instance.GetItemDetails(cropDetails.producedItemID[i]).ItemName,amountToProduce);
            //执行生成指定物品
            for (int j = 0; j < amountToProduce; j++)
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
                else //在世界地图上生成物品
                {
                    
                }
            }

            if (tileDetails != null)
            {
                tileDetails.daysSinceLastHarvest++;
                //是否可以重复生长
                if (cropDetails.daysToRegrow>0&&tileDetails.daysSinceLastHarvest<cropDetails.regrowTimes)
                {
                    tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                    //刷新种子
                    EventHandler.CallRefreshCurrentMap();
                }
                else //不可重复生长
                {
                    tileDetails.daysSinceLastHarvest = -1;
                    tileDetails.seedItemID = -1;
                }
                Destroy(gameObject);
            }
        }
    }
}
