using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFarm.Inventory
{
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxBagTemplate;
        public InventoryBag_SO boxBagData;
        public GameObject tipsIcon;
        private bool canOpen=false;
        private bool isOpen;
        public int index;
        private void OnEnable()
        {
            if (boxBagData == null)
            {
                boxBagData = Instantiate(boxBagTemplate);
            }

            var key = this.name + index;
            if (InventoryManager.Instance.GetBoxDataList(key)!=null)//刷新地图读取数据
            {
                boxBagData.InventoryItemList= InventoryManager.Instance.GetBoxDataList(key);
            }
            else //新建箱子
            {
                if (index==0)
                {
                    index = InventoryManager.Instance.BoxDataCount;
                } 
                InventoryManager.Instance.AddBoxDataDict(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = true;
                tipsIcon.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = false;
                tipsIcon.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isOpen && canOpen && Input.GetMouseButtonDown(1))
            {
                //打开箱子
                isOpen = true;
                EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
            }

            if (!canOpen && isOpen)
            {
                //关闭打开的箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            {
                //ESC关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
        }
    }
}

