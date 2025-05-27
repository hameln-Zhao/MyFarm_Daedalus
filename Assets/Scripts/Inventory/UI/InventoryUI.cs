using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

namespace MyFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {   
        //拖过拖拽赋值
        public ItemToolTip itemToolTip;
        [Header("拖拽图片")]
        public Image dragItemImage;
        [SerializeField]private SlotUI[] playerSlots;
        [Header("玩家背包")]
        [SerializeField] private GameObject BagUI;
        [Header("交易UI")] 
        public TradeUI tradeUI;
        public TextMeshProUGUI playerMoneyText;  
        [Header("通用背包")] 
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;
        public GameObject boxSlotPrefab;
        [SerializeField] private List<SlotUI> baseBagSlots;
        private bool IsBagOpen;
        private void OnEnable()
        {
            EventHandler.UpdataInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneEvent += OnBeforeSceneEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI += OnShowTradeUI;
        }

   

        private void OnDisable()
        {
            EventHandler.UpdataInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneEvent -= OnBeforeSceneEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI -= OnShowTradeUI;
        }

        private void OnShowTradeUI(ItemDetails itemDetails, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetupTradeUI(itemDetails,isSell);
        }

        private void Start()
        {
            //给每一个各自序号
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }

            IsBagOpen = BagUI.activeInHierarchy;
            playerMoneyText.text=InventoryManager.Instance.playerMoney.ToString();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                openBagUI();       
            }
        }
        
        private void OnBeforeSceneEvent()
        {
            //TODO：跨场景后取消高亮
            UpdateSlotHighLight(-1);
        }
        /// <summary>
        /// 打开背包事件
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,
                SlotType.Box => boxSlotPrefab,
                _ => null
            };
            //生成背包UI
            baseBag.SetActive(true);
            baseBagSlots=new List<SlotUI>();
            for (int i = 0; i < bagData.InventoryItemList.Count; i++)
            {
                var slot=Instantiate(prefab, baseBag.transform.GetChild(1)).GetComponent<SlotUI>();
                slot.slotIndex = i;//给序列号初始值
                baseBagSlots.Add(slot);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
            if (slotType==SlotType.Shop)
            {
                BagUI.GetComponent<RectTransform>().pivot = new Vector2(-0.5f, 0.5f);
                BagUI.SetActive(true);
            }
            OnUpdateInventoryUI(InventoryLocation.Box,bagData.InventoryItemList);
        }
        /// <summary>
        /// 关闭背包事件
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);
            UpdateSlotHighLight(-1);
            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();
            if (slotType==SlotType.Shop)
            {
                BagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                BagUI.SetActive(false);
            }

            if (slotType == SlotType.Box)
            {
                BagUI.SetActive(false);
            }
        }
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].ItemAmount>0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].ItemID);
                            playerSlots[i].UpdateSlot(item, list[i].ItemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].ItemAmount>0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].ItemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].ItemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
            playerMoneyText.text=InventoryManager.Instance.playerMoney.ToString();
        }

        public void openBagUI()
        {
            IsBagOpen = !IsBagOpen;
            BagUI.SetActive(IsBagOpen);
        }
        /// <summary>
        /// 更新Slot高亮
        /// </summary>
        /// <param name="index"></param>
        public void UpdateSlotHighLight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.slotIndex==index)
                {
                    slot.slotHightLight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHightLight.gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// 暴露给按钮关闭商店界面
        /// </summary>
        public void CloseBaseBagUI()
        {
            SlotType defaultSlotType = SlotType.Shop;
            InventoryBag_SO defaultData=new InventoryBag_SO();
            OnBaseBagCloseEvent(defaultSlotType,defaultData);
            EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
        }
    }
}

