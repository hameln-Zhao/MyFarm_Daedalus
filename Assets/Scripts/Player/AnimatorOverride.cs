using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFarm.Inventory;
public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;
    public SpriteRenderer holdItem;

    [Header("各部位动画列表")] 
    public List<AnimatorType> animatorTypes;
    private Dictionary<string,Animator> animatorNameDict=new Dictionary<string, Animator>();
    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        //初始化字典
        foreach (var anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.AfterSceneEvent += OnAfterSceneEvent;
        EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.AfterSceneEvent -= OnAfterSceneEvent;
        EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
    }

    private void OnHarvestAtPlayerPosition(int itemID)
    {//显示收货的物品的图片
        Sprite itemSprite = InventoryManager.Instance.GetItemDetails(itemID).ItemOnWorldSprite;
        if (holdItem.enabled==false)
        {
            StartCoroutine(ShowItem(itemSprite));
        }
    }

    private IEnumerator ShowItem(Sprite itemSprite)
    {
        holdItem.enabled = true;
        holdItem.sprite = itemSprite;
        yield return new WaitForSeconds(Settings.holdImgTime);
        holdItem.enabled = false;
    }
    private void OnAfterSceneEvent()
    {
        //TODO：取消所有选择和动画
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        PartType currentType;
        if (itemDetails==null)//传入空 直接把所有动画都b掉
        {
            currentType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            //WORKFLOW:不同工具返回不同的动画在这补全
            //TODO:钓鱼的动画要有两套,先按住左键的时候拉杆 松开左键的时候抛竿
            currentType = itemDetails switch
            {
              
                Equipment {equipmentType: Equipment.EquipmentType.HoeTool} => PartType.Hoe,
                Equipment {equipmentType: Equipment.EquipmentType.WaterTool} => PartType.Water,
                { CanCarried: true } => PartType.Carry,
                //可以加拔起来的动画
                _ => PartType.None
            };
            if (isSelected == false)
            {
                currentType = PartType.None;
                holdItem.enabled = false;
            }
            else
            {
                if (currentType == PartType.Carry)
                {
                    holdItem.sprite = itemDetails.ItemOnWorldSprite;
                    holdItem.enabled = true;
                }
                else
                {
                    holdItem.enabled = false;
                }
            }
        }

        SwitchAnimator(currentType);
    }

    private void SwitchAnimator(PartType partType)
    {
        foreach (var anim in animatorTypes)
        {
            if (anim.partType == partType)
            {
                animatorNameDict[anim.partName.ToString()].runtimeAnimatorController=anim.overrideController;
            }
        }
    }
}
