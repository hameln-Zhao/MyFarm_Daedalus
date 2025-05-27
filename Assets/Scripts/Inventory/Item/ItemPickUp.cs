using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFarm.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item= other.GetComponent<Item>();
            if (item!=null)
            {
                if (item.ItemDetails.CanPickedup)
                {
                    //拾取物品添加到背包
                    InventoryManager.Instance.AddItem(item,true);
                }
            }
        }
    }
}
