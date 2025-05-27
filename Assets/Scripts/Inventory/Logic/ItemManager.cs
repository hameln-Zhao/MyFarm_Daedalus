using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace MyFarm.Inventory
{
    public class ItemManager : Singleton<ItemManager>
    {
        public Item itemPrefab;
        public Item bouncePrefab;
        private Transform playerTransform => FindObjectOfType<Player>().transform;
        private Transform itemParent;
        private Dictionary<string,List<SceneItem>> sceneItemDict=new Dictionary<string, List<SceneItem>>();
        private void OnEnable()
        {
            EventHandler.InstaniateItemInScene += OnInstaninateItemInScene;
            EventHandler.BeforeSceneEvent += OnBeforeSceneEvent;
            EventHandler.AfterSceneEvent += OnAfterSceneEvent;
            EventHandler.DropItemEvent += OnDropItemEvent;
        }
        private void OnDisable()
        {
            EventHandler.InstaniateItemInScene -= OnInstaninateItemInScene;
            EventHandler.BeforeSceneEvent += OnBeforeSceneEvent;
            EventHandler.AfterSceneEvent -= OnAfterSceneEvent;
            EventHandler.DropItemEvent -= OnDropItemEvent;
        }

        private void OnBeforeSceneEvent()
        {
            GetAllSceneItems();
        }

        private void OnAfterSceneEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            ReCreateSceneItems();
        }
        private void OnInstaninateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
            item.ItemID = ID;
        }
        private void OnDropItemEvent(int ID, Vector3 mouseWorldPos)
        {
            var item = Instantiate(bouncePrefab, playerTransform.position, Quaternion.identity, itemParent);
            item.ItemID = ID;
            var dir=(mouseWorldPos-playerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mouseWorldPos,dir);
        }
        /// <summary>
        /// 获取当前场景中所有的Item
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    itemID = item.ItemID,
                    position = new SerializableVector3(item.transform.position)
                };
                currentSceneItems.Add(sceneItem);
                if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
                {
                    sceneItemDict[SceneManager.GetActiveScene().name]=currentSceneItems;
                }
                else
                {
                    sceneItemDict.Add(SceneManager.GetActiveScene().name,currentSceneItems);
                }
            }
        }
        /// <summary>
        /// 刷新重建当前场景的物体
        /// </summary>
        private void ReCreateSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            //如果一开始进入一个新场景 字典里是没有加载过这里的东西的
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name,out currentSceneItems))
            {
                if (currentSceneItems!=null)
                {
                    //清场
                    foreach (var item in FindObjectsOfType<Item>())
                    { 
                        Destroy(item.gameObject);
                    }

                    foreach (var item in currentSceneItems)
                    {
                        Item newItem =Instantiate(itemPrefab,item.position.ToVector3(),Quaternion.identity,itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }
    }
}
