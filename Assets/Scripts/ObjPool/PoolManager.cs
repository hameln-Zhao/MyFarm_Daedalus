using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
    
    private Queue<GameObject> soundQueue = new Queue<GameObject>();

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        CreatePool();
    }

    /// <summary>
    /// 生成对象池
    /// </summary>
    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            var parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, parent),
                e => { e.gameObject.SetActive(true);},
                e => { e.gameObject.SetActive(false);},
                e => { Destroy(e);}
                );
            poolEffectList.Add(newPool);
        }
    }
    
    
    
}
