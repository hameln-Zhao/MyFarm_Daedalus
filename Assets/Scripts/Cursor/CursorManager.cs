using System;
using System.Collections;
using System.Collections.Generic;
using MyFarm.CropPlant;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MyFarm.Map;
public class CursorManager : Singleton<CursorManager>
{
    public Sprite normal, tool, seed, item;
    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;
    private bool cursorEnable; //有选中的物体才会变成true。注意切换场景会false掉
    private bool cursorPositionVaild;//用于判断鼠标当前的位置是否有效
    private bool cursorPressVaild;//用于判断物品是否可以长按鼠标使用
    //鼠标检测
    private Camera mainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;
    public ItemDetails currentItem;
    private Transform playerTransform => FindObjectOfType<Player>().transform;
    void Start()
    {
        cursorCanvas=GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage=cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite=normal;
        SetCursorSprite(currentSprite);
        mainCamera=Camera.main;
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneEvent += OnBeforeSceneEvent;
        EventHandler.AfterSceneEvent += OnAfterSceneEvent;
    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.AfterSceneEvent -= OnAfterSceneEvent;
        EventHandler.BeforeSceneEvent -= OnBeforeSceneEvent;
    }
    
    private void Update()
    {
        if (cursorCanvas == null) return;
        cursorImage.transform.position = Input.mousePosition;
        if (!InteractWithUI()&&cursorEnable)
        {
            SetCursorSprite(currentSprite);
            //再加层判断就能实现切换场景也能用了
            CheckCursorVaild();
            CheckPlayerInput();
            //CheckCursorPressVaild();
        }
        else
        {
            SetCursorSprite(normal);
        }
        
    }
    private void OnAfterSceneEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
        //cursorEnable = true;
    }
    
    private void OnBeforeSceneEvent()
    {
        cursorEnable = false;
    }
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        //WORKFLOW:添加所有类型的图片
        if (!isSelected)
        {
            currentItem = null;
            cursorEnable=false;
            currentSprite = normal;
        }
        else //物品被选中才切换
        { 
            currentItem = itemDetails;
            //WORKFLOW:添加所有类型对应图片
            currentSprite=itemDetails switch
            {  
                // 类型匹配 + 属性匹配 类似于 itemDetails is Equipment equipment {}里是内联属性 可以自己判断
                Consumable {consumableType: Consumable.ConsumableType.Seed} => seed,
                Consumable =>item,
                Equipment  => tool,
                Furniture  => tool,
                _=> normal
            };
            cursorEnable = true;
        }
    }

    #region 设置鼠标样式
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorSprite(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1,1);
    }
    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorVaild()
    {
        cursorPositionVaild=true;
        cursorImage.color = new Color(1, 1, 1,1);
    }
    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInVaild()
    {
        cursorPositionVaild = false;
        cursorImage.color = new Color(1, 0, 0,0.4f);
    }
    #endregion
    /// <summary>
    /// 判断是否和 UI有互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        //当前
        if (EventSystem.current !=null&&EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0)&& cursorPositionVaild)
        {
            
            //执行方法
            EventHandler.CallMouseClickedEvent(mouseWorldPos,currentItem);
        }
        if (Input.GetMouseButtonUp(0))
        {
            //执行方法
            EventHandler.CallMouseReleaseEvent(mouseWorldPos,currentItem);
        }
    }
    private void CheckCursorVaild()
    {
        mouseWorldPos=mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos=currentGrid.WorldToCell(mouseWorldPos);
        var playerGridPos=currentGrid.WorldToCell(playerTransform.position);
        //判断是否在适用范围内
        if (Mathf.Abs(mouseGridPos.x-playerGridPos.x)>currentItem.ItemUseRadius||Mathf.Abs(mouseGridPos.y-playerGridPos.y)>currentItem.ItemUseRadius)
        {
            SetCursorInVaild();
            return;
        }
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        if (currentTile!=null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            //WORKFLOW:补充所有物品的类型判断
            switch (currentItem)
            {
                case  Consumable {consumableType: Consumable.ConsumableType.Seed}:
                    if (currentTile.daysSinceDug>-1&& currentTile.seedItemID==-1)SetCursorVaild();else SetCursorInVaild();
                    break;
                case Consumable {consumableType: Consumable.ConsumableType.Magic}:
                    if (currentTile != null && currentTile.seedItemID != -1)
                    {
                        CropDetails currentCopMagic = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
                        if (currentCopMagic != null) SetCursorVaild(); else SetCursorInVaild();
                    }  else SetCursorInVaild();
                    break;
                case Equipment {equipmentType: Equipment.EquipmentType.HoeTool}:
                    if (currentTile.canDig) SetCursorVaild();else SetCursorInVaild();
                    break;
                case Equipment {equipmentType: Equipment.EquipmentType.WaterTool}:
                    if (currentTile.daysSinceDug>-1 && currentTile.daysSinceWatered ==-1) SetCursorVaild();else SetCursorInVaild();
                    break;
                case Equipment {equipmentType: Equipment.EquipmentType.CollectTool}:
                    if (currentCrop!=null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.ItemID))
                            if(currentTile.growthDays>=currentCrop.TotalGrowthDays)SetCursorVaild();else SetCursorInVaild();
                    }else SetCursorInVaild();
                    break;
                 case Equipment {equipmentType: Equipment.EquipmentType.FishingTool}:
                     if (currentTile.canFish && FishManager.Instance.fishGameState == FishState.Empty) SetCursorVaild();else SetCursorInVaild();
                      break;
                //可以丢的物品：
                case Consumable:
                    if (currentTile.canDropItem && currentItem.CanDropped) SetCursorVaild();else SetCursorInVaild();
                    break;
            }
        }
        else//如果不是Tile默认是不可用的
        {
            SetCursorInVaild();
        }
    }
}
