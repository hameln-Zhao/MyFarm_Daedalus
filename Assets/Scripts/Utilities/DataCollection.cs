using System;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.Serialization;
// 物品分类

[System.Serializable]
public class ItemDetails
{
    public int ItemID; // 物品ID
    public string ItemName; // 物品名称
    public ItemType ItemType; // 物品类型
    public Sprite ItemIcon; // 物品图标
    public Sprite ItemOnWorldSprite; // 物品在世界中的图标
    public string ItemDescription; // 物品描述
    public int ItemPrice; // 物品价格
    public int ItemUseRadius;
    [Range(0, 1)] public float SellPercentage; // 出售价格百分比
    
    [Header("Item Properties")]
    public bool CanStack;//是否可以堆叠
    public bool CanInBag;//是否可以放入背包
    public bool CanWear; //是否可以穿戴
    public bool CanHold; //是否可以手持
    public bool CanSell;//是否可以出售
    public bool CanUse;//是否可以使用
    public bool CanPlace;//是否可以摆放
    public bool CanRotate;//是否可以旋转
    public bool CanInteractOnMap;//是否可以和地图交互
    public bool CanUseInCrafting;//是否可以用于制作
    public bool CanGift;//是否可以赠送
    public bool CanPickedup; //是否可以拾取
    public bool CanDropped; //是否可以丢下
    public bool CanCarried; //是否可以举起
    public virtual string TypeDisplayName => GetDefaultTypeName();
    protected string GetDefaultTypeName()
    {
        return "未知类型"; // 或根据 ItemType 返回通用名称
    }
}


[System.Serializable]
//struct不需要判断是否为空 class需要
public struct InventoryItem
{
    public int ItemID;
    public int ItemAmount;
}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}
//定义可以序列化的vector3
[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    //格子坐标
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}

[System.Serializable]
public class SceneFurniture
{
    public int boxIndex;
}
//单个格子的信息
//这里的filedType用来区分水域或其他区域
//水域：池塘 0 小河 1 海洋 2
[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public GridType gridType;
    public bool boolTypeValue;
    public int filedType;
}

[System.Serializable]
public class TileDetails
{
    public int gridX;
    public int gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;
    public bool canFish;
    public int filedType;
    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    public int seedItemID = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;
    
    //魔素相关
    public int currentMagicAmount = 0; // 当前魔素值，初始为0
    public bool isMutated = false; // 标记作物是否进入变异状态
}

public enum SkillType
{
    BasicFishing,
    Heal
}