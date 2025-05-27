using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyFarm.Inventory
{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;
        public TextMeshProUGUI itemName;
        public TMP_InputField tradeAmount;
        public Button submitButton;
        public Button cancelButton;
        private ItemDetails itemDetails;
        private bool isSellTrade;

        private void Awake()
        {
            cancelButton.onClick.AddListener(CancelTrade);
            submitButton.onClick.AddListener(TradeItem);
        }

        /// <summary>
        /// 设置UI现实的详情
        /// </summary>
        /// <param name="itemDetails"></param>
        /// <param name="isSellTrade"></param>
        public void SetupTradeUI(ItemDetails itemDetails, bool isSellTrade)
        {
            this.itemDetails = itemDetails;
            itemIcon.sprite = itemDetails.ItemIcon;
            itemName.text = itemDetails.ItemName;
            this.isSellTrade = isSellTrade;
            tradeAmount.text = string.Empty;
        }

        private void TradeItem()
        {
            var amount = Convert.ToInt32(tradeAmount.text);//把字符串转为数字
            InventoryManager.Instance.TradeItem(itemDetails, amount, isSellTrade);
            CancelTrade();
        }
        private void CancelTrade()
        {
            gameObject.SetActive(false);
        }
    }
}
