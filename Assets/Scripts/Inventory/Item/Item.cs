using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFarm.Inventory
{
    public class Item : MonoBehaviour
    {
        public int ItemID;
        private SpriteRenderer SpriteRenderer;
        public ItemDetails ItemDetails;
        private BoxCollider2D Coll;
        private void Awake()
        {
            SpriteRenderer=GetComponentInChildren<SpriteRenderer>();
            Coll = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (ItemID != 0)
            {
                Init(ItemID);
            }
        }

        public void Init(int ID)
        {
            ItemID = ID;
            //从Inventory拿数据
            ItemDetails = InventoryManager.Instance.GetItemDetails(ItemID);
            if (ItemDetails!=null)
            {
                SpriteRenderer.sprite = ItemDetails.ItemOnWorldSprite!=null?ItemDetails.ItemOnWorldSprite:ItemDetails.ItemIcon;
                //修改碰撞体尺寸
                Vector2 newSize = SpriteRenderer.sprite.bounds.size;
                Coll.size = newSize;
                Coll.offset=new Vector2(0,SpriteRenderer.sprite.bounds.center.y); 
            }
        }
    }

}