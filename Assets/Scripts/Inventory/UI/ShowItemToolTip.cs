using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SlotUI slotUI;
       private InventoryUI inventoryUI => FindObjectOfType<InventoryUI>();
        private void Awake()
        {
            slotUI=GetComponent<SlotUI>();
        }
        

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails!=null)
            {
                inventoryUI.itemToolTip.gameObject.SetActive(true);
                inventoryUI.itemToolTip.SetItemToolTip(slotUI.itemDetails,slotUI.slotType);
                //inventoryUI.dragItemImage.raycastTarget=true;
                inventoryUI.itemToolTip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * 60;
            }
            else
            {
                inventoryUI.itemToolTip.gameObject.SetActive(false);
                //inventoryUI.dragItemImage.raycastTarget=false;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);
        }
    }
}
