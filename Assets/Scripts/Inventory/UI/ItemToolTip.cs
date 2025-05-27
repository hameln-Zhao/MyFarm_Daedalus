using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemToolTip : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI nameText;
    [SerializeField]private TextMeshProUGUI typeText;
    [SerializeField]private TextMeshProUGUI descripitonText;
    [SerializeField]private Text valueText;
    [SerializeField]private GameObject bottomPart;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="slotType">用于判断是在包里还是商店</param>
    public void SetItemToolTip(ItemDetails itemDetails,SlotType slotType)
    {
        nameText.text = itemDetails.ItemName;
        //typeText.text = GetItemType(itemDetails.ItemType);
        typeText.text = itemDetails.TypeDisplayName;
        descripitonText.text = itemDetails.ItemDescription;
        if (itemDetails.CanSell)
        {
            bottomPart.SetActive(true);
            var price=itemDetails.ItemPrice;
            if (slotType==SlotType.Bag)
            {
                price=(int)(price*itemDetails.SellPercentage);
            }
            valueText.text = price.ToString();
        }
        else//不可出售
        {
            bottomPart.SetActive(false);
        }
    
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Apparel => "装扮",
            ItemType.BuffModule => "加成插件",
            ItemType.Furniture => "家具",
            ItemType.Pet => "宠物/动物",
            ItemType.Equipment => "装备",
            ItemType.EquipmentPart => "装备配件",
            ItemType.Consumable => "消耗品",
            ItemType.QuestItem => "非卖品",
            ItemType.Miscellaneous => "其他",
            _ => "无"
        };
    }
}
