using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Farm : MonoBehaviour
{
    private PolygonCollider2D farmCollider;
    private GameObject animalParent;
    

    // Start is called before the first frame update
    void Start()
    {
        // 获取牧场的PolygonCollider2D组件
        farmCollider = GetComponent<PolygonCollider2D>();
        animalParent = GameObject.FindWithTag("AnimalParent");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //Debug.Log("来个史莱姆");
            Debug.Log(animalParent);
            EventHandler.CallPlaceAnimalEvent(2001, animalParent.transform.position);
        }
    }

    // 检查动物是否在牧场范围内
    public bool IsInFarmArea(Vector2 position)
    {
        // 判断给定的坐标是否在牧场的范围内
        return farmCollider.OverlapPoint(position);
    }
}
