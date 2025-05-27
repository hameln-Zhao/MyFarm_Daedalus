using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace MyFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("组件获取")]
        [SerializeField]private Image slotImage;
        public Image slotHightLight;
        [SerializeField]private TextMeshProUGUI amountText;
        [SerializeField]private Button slotButton;
        [Header("格子类型")]
        public SlotType slotType;
        public bool isSelected;
    
        public ItemDetails itemDetails;//默认会有初始化？
        public int itemAmount;
        public int slotIndex;
        
        [SerializeField] private Image cornerIcon; // 右上角图标
        [SerializeField] private Sprite[] rarityIcons; // 稀有度图标数组
        [SerializeField] private Sprite[] toolIcons; // 稀有度图标数组
        public InventoryLocation Location
        {
            get
            {
                return slotType switch
                {
                    SlotType.Bag=>InventoryLocation.Player,
                    SlotType.Box=>InventoryLocation.Box,
                    _=>InventoryLocation.Player
                };
            }
        }
        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
        private void Start()
        {
            isSelected = false;
            if (itemDetails==null)
            {
                UpdateEmptySlot();
            }
        }
    
        /// <summary>
        /// 更新格子UI和信息
        /// </summary>
        /// <param name="itemDetails"></param>
        /// <param name="itemAmount"></param>
        public void UpdateSlot(ItemDetails itemDetails, int itemAmount)
        {
            this.itemDetails = itemDetails;
            slotImage.sprite = itemDetails.ItemIcon;
            this.itemAmount = itemAmount;
            amountText.text = itemAmount.ToString();
            slotImage.enabled = true;
            slotButton.interactable = true;
            //TODO:可能右上角有图标也可以加上
        }
        /// <summary>
        /// 将格子更新为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateSlotHighLight(-1);
                EventHandler.CallItemSelectedEvent(itemDetails,isSelected);
            }
            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            slotButton.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails==null)return;
            isSelected=!isSelected;
            if (slotHightLight!=null)
            {
                inventoryUI.UpdateSlotHighLight(slotIndex);
                if (slotType==SlotType.Bag)
                {
                    //通知背包里的物体被选中
                    EventHandler.CallItemSelectedEvent(itemDetails,isSelected);
                }
            }
           
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0)
            {
                inventoryUI.dragItemImage.enabled = true; 
                inventoryUI.dragItemImage.sprite = slotImage.sprite;
                inventoryUI.dragItemImage.SetNativeSize();
                isSelected = true;
                inventoryUI.UpdateSlotHighLight(slotIndex);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItemImage.transform.position=Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItemImage.enabled = false;
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                //代表碰撞到的是UI
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>()==null) return;
                var target = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex=target.slotIndex;
                //在自身背包交换
                if (slotType==SlotType.Bag&&target.slotType==SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex,targetIndex);
                    inventoryUI.UpdateSlotHighLight(-1);//取消所有高亮
                    //
                }else if (slotType == SlotType.Shop && target.slotType==SlotType.Bag)//买
                {
                    EventHandler.CallShowTradeUI(itemDetails,false);
                }
                else if (slotType == SlotType.Bag && target.slotType==SlotType.Shop)//卖
                {
                    EventHandler.CallShowTradeUI(itemDetails,true);
                }
                else if (slotType !=SlotType.Shop && target.slotType!=SlotType.Shop && slotType!=target.slotType)//箱子与背包间
                {
                    //跨背包数据交换物品
                    InventoryManager.Instance.SwapItem(Location,slotIndex,target.Location,target.slotIndex);
                }
                //清空所有高亮显示
                inventoryUI.UpdateSlotHighLight(-1);
            }//扔在了地上
            else
            {
                if (itemDetails.CanDropped)
                {
                    //获取鼠标松开后对应的世界坐标
                    var pos=Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,-Camera.main.transform.position.z));
                    EventHandler.CallInstaniateItemInScene(itemDetails.ItemID,pos);
                }
            }
        }
        /// <summary>
        /// 如果右上角有图标可以用上的设置方法
        /// </summary>
        /// <param name="itemDetails"></param>
        private void UpdateCornerIcon(ItemDetails itemDetails)
        {
            if (cornerIcon != null && rarityIcons != null && rarityIcons.Length > 0)
            {
                // 检查是否为 ConsumableDetails
                if (itemDetails is Consumable consumableDetails)
                {
                    // 根据稀有度设置图标
                    int rarityIndex = (int)consumableDetails.rarity;
                    if (rarityIndex >= 0 && rarityIndex < rarityIcons.Length)
                    {
                        cornerIcon.sprite = rarityIcons[rarityIndex];
                        cornerIcon.enabled = true;
                    }
                    else
                    {
                        cornerIcon.enabled = false;
                    }
                }
                /*// 检查是否为 ToolDetails
                else if (itemDetails is Equipment toolDetails)
                {
                    // 根据稀有度设置图标
                    int typeIndex = (int)toolDetails.toolType;
                    if (typeIndex >= 0 && typeIndex < toolIcons.Length)
                    {
                        cornerIcon.sprite = rarityIcons[typeIndex];
                        cornerIcon.enabled = true;
                    }
                    else
                    {
                        cornerIcon.enabled = false;
                    }
                }*/
                else
                {
                    // 如果不是特定类型，隐藏图标
                    cornerIcon.enabled = false;
                }
            }
            else
            {
                cornerIcon.enabled = false;
            }
        }
    }
}

