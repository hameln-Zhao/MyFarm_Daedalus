using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//管理所有背包相关的数据
namespace MyFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("物品数据")]
        public ItemContainer_SO ItemContainer_SO;
        [Header("玩家背包数据")]
        public InventoryBag_SO PlayerBag_SO;

        [Header("存储箱")] 
        public InventoryBag_SO CurrentBoxBag;
        [Header("金钱")] 
        public int playerMoney;

        private Dictionary<string,List<InventoryItem>> boxDataDict= new Dictionary<string, List<InventoryItem>>();
        private Dictionary<int, ItemDetails> itemDictionary;
        public int BoxDataCount => boxDataDict.Count;
        private void Start()
        {
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player,PlayerBag_SO.InventoryItemList);
        }

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        }
        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
        }
        private void OnHarvestAtPlayerPosition(int itemID)
        {
            int index = GetItemIndexInBag(itemID);
          
            AddItemAtIndex(itemID,index,1);
            //更新UI
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player,PlayerBag_SO.InventoryItemList);
        }

        private void OnDropItemEvent(int ID, Vector3 mouseWorldPos)
        {
            RemoveItem(ID,1);
        }
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO Bag_SO)
        {
            CurrentBoxBag = Bag_SO;
        }
        /// <summary>
        /// 构建初始物品的字典
        /// </summary>
        private void BuildItemDictionary()
        {
            itemDictionary = new Dictionary<int, ItemDetails>();
    
            // 添加consumables主列表
            if (ItemContainer_SO.consumables != null)
            {
                foreach (var item in ItemContainer_SO.consumables.ItemDetailsList)
                {
                    itemDictionary[item.ItemID] = item;
                }
            }

            // 添加consumables子列表（如FishDataList_SO）
            if (ItemContainer_SO.consumables is ConsumableData_SO consumableData && 
                consumableData.fishDataList_SO != null)
            {
                foreach (var item in consumableData.fishDataList_SO.fishDataList)
                {
                    itemDictionary[item.ItemID] = item;
                }
            }

            // 添加equipments列表
            if (ItemContainer_SO.equipments != null)
            {
                foreach (var item in ItemContainer_SO.equipments.ItemDetailsList)
                {
                    itemDictionary[item.ItemID] = item;
                }
            }
        }

        /// <summary>
        /// 返回ID对应的ItemDeatails
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int itemID)
        {
            if (itemDictionary == null)
            {
                BuildItemDictionary();
            }

            if (itemDictionary.TryGetValue(itemID, out var item))
            {
                return item;
            }

            Debug.LogWarning($"未找到ID为{itemID}的物品");
            return null;
        }
        /// <summary>
        /// 添加物品到背包 这里考虑的是世界上的物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">是否销毁</param>
        public void AddItem(Item item,bool toDestory)
        {
            //背包有没有空位 itemid是否有为0的
            //是否已经有该物体
            int index = GetItemIndexInBag(item.ItemID);
            AddItemAtIndex(item.ItemID,index,1);
        
            if (toDestory)
            {
                Destroy(item.gameObject);
            }
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player,PlayerBag_SO.InventoryItemList);
        }
        /// <summary>
        /// 添加物品到背包的函数，如：任务奖励等
        /// </summary>
        /// <param name="item"></param>
        public void UpdateRewardItem(InventoryItem item)
        {
            //背包有没有空位 itemid是否有为0的
            //是否已经有该物体
            int index = GetItemIndexInBag(item.ItemID);
            AddItemAtIndex(item.ItemID,index,item.ItemAmount);//有负数自动减
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player,PlayerBag_SO.InventoryItemList);
        }
        /// <summary>
        /// 添加消耗品到背包中
        /// </summary>
        /// <param name="item"></param>
        public void UpdateConsumableItem(InventoryItem item,Consumable.ConsumableType consumableType )
        {
            //背包有没有空位 itemid是否有为0的
            //是否已经有该物体
            int index = GetItemIndexInBag(item.ItemID);
            AddItemAtIndex(item.ItemID,index,item.ItemAmount);//有负数自动减
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player,PlayerBag_SO.InventoryItemList);
        }
        /// <summary>
        /// 查找并返回背包已有物品位置
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns>没有返回-1</returns>
        private int GetItemIndexInBag(int itemID)
        {
            for (int i = 0; i < PlayerBag_SO.InventoryItemList.Count; i++)
            {
                if (PlayerBag_SO.InventoryItemList[i].ItemID==itemID)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns>没有返回false</returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < PlayerBag_SO.InventoryItemList.Count; i++)
            {
                if (PlayerBag_SO.InventoryItemList[i].ItemID==0)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddItemAtIndex(int itemID,int index, int amount)
        {
            if (index==-1 && CheckBagCapacity())//背包无物体 且 有空位
            {
                var item = new InventoryItem{ItemID =itemID,ItemAmount = amount};
                for (int i = 0; i < PlayerBag_SO.InventoryItemList.Count; i++)
                {
                    if (PlayerBag_SO.InventoryItemList[i].ItemID==0)
                    {
                        PlayerBag_SO.InventoryItemList[i] = item;
                        break;
                    }
                }
            }
            else//背包有这个
            {
                int currentItemAmount = PlayerBag_SO.InventoryItemList[index].ItemAmount+amount;
                if (currentItemAmount>=0)
                {
                var item = new InventoryItem{ItemID =itemID,ItemAmount = currentItemAmount};
                PlayerBag_SO.InventoryItemList[index] = item;     
                }
                else
                {
                    Debug.LogError("物品数量少于需求");
                }
            }
        }
        /// <summary>
        /// 交换物品
        /// </summary>
        /// <param name="fromIndex">拖动的物体</param>
        /// <param name="targetIndex">放置的物体</param>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem fromItem = PlayerBag_SO.InventoryItemList[fromIndex];
            InventoryItem targetItem = PlayerBag_SO.InventoryItemList[targetIndex];
            if (targetItem.ItemID != 0)
            {
                PlayerBag_SO.InventoryItemList[targetIndex] = fromItem;
                PlayerBag_SO.InventoryItemList[fromIndex] = targetItem;
            }
            else
            {
                PlayerBag_SO.InventoryItemList[targetIndex] = fromItem;
                PlayerBag_SO.InventoryItemList[fromIndex] = new InventoryItem();
            }
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player,PlayerBag_SO.InventoryItemList);
            EventHandler.CallItemSelectedEvent(null,false);
        }
        /// <summary>
        /// 背包交换数据
        /// </summary>
        /// <param name="LocationFrom"></param>
        /// <param name="fromIndex"></param>
        /// <param name="LocationTarget"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(InventoryLocation LocationFrom,int fromIndex, InventoryLocation LocationTarget,int targetIndex)
        {
            var currentList = GetItemList(LocationFrom);
            var targetList = GetItemList(LocationTarget);
            InventoryItem currentItem = currentList[fromIndex];
            if (targetIndex<targetList.Count)//确保目标index在数据列表范围之内
            {
                InventoryItem targetItem = targetList[targetIndex];
                if (targetItem.ItemID!=0 && currentItem.ItemID!=targetItem.ItemID)//有不相同的两个物品
                {
                    currentList[fromIndex]=targetItem;
                    targetList[targetIndex]=currentItem;
                }
                else if (currentItem.ItemID==targetItem.ItemID)
                {
                    targetItem.ItemAmount +=currentItem.ItemAmount;
                    targetList[targetIndex]=targetItem;
                    currentList[fromIndex]= new InventoryItem();
                }
                else //目标空格子
                {
                    targetList[targetIndex]=currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
            }
            EventHandler.CallUpdataInventoryUI(LocationFrom,currentList);
            EventHandler.CallUpdataInventoryUI(LocationTarget,targetList);
        }
        /// <summary>
        /// 根据位置返回背包的数据列表
        /// </summary>
        /// <param name="inventoryLocation"></param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(InventoryLocation inventoryLocation)
        {
              return inventoryLocation  switch
            {
                InventoryLocation.Player => PlayerBag_SO.InventoryItemList ,
                InventoryLocation.Box => CurrentBoxBag.InventoryItemList,
                _=> null
            };
        }
        /// <summary>
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <param name="removeAmount">物品数量</param>
        public void RemoveItem(int itemID,int removeAmount)
        {
            var index = GetItemIndexInBag(itemID);
            if (PlayerBag_SO.InventoryItemList[index].ItemAmount>removeAmount)
            {
                int currentItemAmount = PlayerBag_SO.InventoryItemList[index].ItemAmount-removeAmount;
                var item = new InventoryItem{ItemID =itemID,ItemAmount = currentItemAmount};
                PlayerBag_SO.InventoryItemList[index] = item;
            }
            else
            {
                var item = new InventoryItem();
                PlayerBag_SO.InventoryItemList[index] = item;
            }
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player,PlayerBag_SO.InventoryItemList);
        }
        /// <summary>
        /// 交易物品
        /// </summary>
        /// <param name="itemDetails"></param>
        /// <param name="amount"></param>
        /// <param name="isSellTrade"></param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.ItemPrice * amount;
            //获得背包物品位置
            int index = GetItemIndexInBag(itemDetails.ItemID);
            if (isSellTrade) //卖
            {
                if (PlayerBag_SO.InventoryItemList[index].ItemAmount >= amount)
                {
                    RemoveItem(itemDetails.ItemID, amount);
                    //卖出总价
                    cost = (int)(cost * itemDetails.SellPercentage);
                    playerMoney += cost;
                }
            } 
            else if (playerMoney-cost>=0)//买
            {
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.ItemID, index, amount);
                }
                playerMoney -= cost;
            }
            //刷新UI
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player,PlayerBag_SO.InventoryItemList);
        }
        /// <summary>
        /// 查找箱子数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if (boxDataDict.ContainsKey(key))
            {
                return boxDataDict[key];
            }

            return null;
        }
        /// <summary>
        /// 加入箱子字典数据
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxDataDict(Box box)
        {
            var key = box.name + box.index;
            if (!boxDataDict.ContainsKey(key))
            {
                boxDataDict.Add(key,box.boxBagData.InventoryItemList);
            }
        }
        # region 检测背包里的任务物品

        public void CheckQuestItemInBag(string questItemName)
        {
            foreach (var item in PlayerBag_SO.InventoryItemList)
            {
                if (item.ItemID!=0)
                {
                    if (GetItemDetails(item.ItemID).ItemName==questItemName)
                    {
                        QuestManager.Instance.UpdateQuestProgress(questItemName,item.ItemAmount);
                    }
                }
            }
        }
        #endregion
    }

}
