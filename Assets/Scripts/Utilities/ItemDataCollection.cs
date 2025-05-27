using System.Collections.Generic;
using UnityEngine;

public class Apparel : ItemDetails
{
    public enum ApparelType
    {
        Head,    // 帽子
        Top,      // 上装
        Bottom,   // 下装
        Shoes,    // 鞋子
        Accessory // 饰品
    }
    
    public ApparelType apparelType;
    public int defense; // 防御值
    public float moveSpeedModifier; // 移动速度修正
    
    void OnEnable()
    {
        ItemType = ItemType.Apparel;
        CanStack = false;
        CanInBag = true;
        CanWear = true;
        CanHold = true;
        CanSell = true;
        CanUse = false;
        CanPlace = false;
        CanRotate = false;
        CanInteractOnMap = false;
        CanUseInCrafting = false;
        // canGift 可以根据具体物品设置
    }
}
public class BuffModule : ItemDetails
{
    public enum BuffType
    {
        Speed,      // 移速
        Attack,     // 攻击
        Defense,    // 防御
        Harvest,    // 收获
        Luck        // 幸运
    }
    
    public BuffType buffType;
    public float buffValue; // 加成值
    public float duration; // 持续时间(0表示永久)
    
    void OnEnable()
    {
        ItemType = ItemType.BuffModule;
        CanStack = false; // 或根据需求可堆叠
        CanInBag = true;
        CanWear = true;    // 需要放入专用装置
        CanHold = true;
        CanSell = true;
        CanUse = false;
        CanPlace = false;
        CanRotate = false;
        CanInteractOnMap = false;
        CanUseInCrafting = true;
        // canGift 可根据需求设置
    }
}
public class Pet : ItemDetails
{
    public enum PetType
    {
        Companion,  // 宠物
        FarmAnimal  // 农场动物
    }
    
    public PetType petType;
    public GameObject petPrefab; // 宠物实体预制体
    public float followDistance = 2f;
    public string[] interactionOptions; // 可交互选项
    
    void OnEnable()
    {
        ItemType = ItemType.Pet;
        CanStack = false;
        CanInBag = false;
        CanWear = false;
        CanHold = false;
        CanSell = true;
        CanUse = false;
        CanPlace = false;
        CanRotate = false;
        CanInteractOnMap = true;
        CanUseInCrafting = false;
        CanGift = false;
    }
    
    // 宠物交互方法
    public void Interact(string interactionType)
    {
        // 实现具体交互逻辑
    }
}
[System.Serializable]
public class Equipment : ItemDetails
{
    //可能还要增加武器？
    public enum EquipmentType
    {
        HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,FishingTool
    }
    //装备自己有基础的攻击伤害 除了鱼竿之外都可以挥动
    public float BaseDamage;
    public EquipmentType equipmentType;
    public override string TypeDisplayName => $"装备 - {equipmentType}";
    public float efficiency = 1f; // 效率系数 例如金斧子的效率是银斧子的两倍
    
    // 配件系统
    public List<EquipmentPart> Attachments = new List<EquipmentPart>();
    public int MaxAttachments = 3; // 最大配件数
    
    void OnEnable()
    {
        ItemType = ItemType.Equipment;
        CanStack = false;
        CanInBag = true;
        CanWear = false;
        CanHold = true;
        CanSell = true;
        CanUse = true;
        CanPlace = false;
        CanRotate = false;
        CanInteractOnMap = false;
        CanUseInCrafting = true;
        // canGift 可根据需求设置
    }
    public bool AddAttachment(EquipmentPart attachment)
    {
        if (Attachments.Count >= MaxAttachments) return false;
        
        Attachments.Add(attachment);
        return true;
    }

    // 移除配件
    public void RemoveAttachment(EquipmentPart attachment)
    {
        Attachments.Remove(attachment);
    }
}

public class EquipmentPart : ItemDetails
{
    public enum PartType
    {
        Float,      // 浮标
        Bait,       // 鱼饵
        Upgrade     // 升级部件
    }
    public PartType partType;
    public override string TypeDisplayName => $"装备配件 - {partType}";
    public Equipment.EquipmentType compatibleEquipment; // 兼容的装备类型
    public float effectValue; // 效果值
    
    //挂一个技能类 在技能类里实现相应的效果？
    void OnEnable()
    {
        ItemType = ItemType.EquipmentPart;
        CanStack = true;
        CanInBag = true;
        CanWear = false;
        CanHold = true;
        CanSell = true;
        CanUse = false;
        CanPlace = false;
        CanRotate = false;
        CanInteractOnMap = false;
        CanUseInCrafting = true;
        // canGift 可根据需求设置
    }
}
[System.Serializable]
public class Consumable : ItemDetails
{
    public enum ConsumableType
    {
        Crop,           // 作物
        Product,         // 农产品
        Potion,          // 药水
        Dish,            // 菜肴
        RitualMaterial,  // 仪式材料
        Forage,          // 野外采集品
        Fish,            // 鱼
        Fuel,            // 燃料
        Seed,            // 种子
        Magic            //魔素
    }
    
    public ConsumableType consumableType;
    public override string TypeDisplayName => $"消耗品 - {consumableType}";
    public int restoreValue; // 恢复值(生命/能量等)
    public float effectDuration; // 效果持续时间
    public Rarity rarity;
    
    void OnEnable()
    {
        ItemType = ItemType.Consumable;
        CanStack = true;
        CanInBag = true;
        CanWear = false;
        CanHold = true;
        CanSell = true;
        CanUse = true; // 大多数消耗品可使用
        CanPlace = true; // 部分可以摆放
        CanRotate = false;
        CanInteractOnMap = false;
        CanUseInCrafting = true;
        CanGift = true;
    }
}

public class Furniture : ItemDetails
{
    public enum FurnitureType
    {
        Chair,      // 椅子
        Bed,        // 床
        Table,      // 桌子
        Decoration, // 装饰品
        Wallpaper,  // 墙纸
        Painting    // 挂画
    }
    
    public FurnitureType furnitureType;
    public GameObject furniturePrefab; // 家具预制体
    public Vector2 footprint = new Vector2(1, 1); // 占地面积
    
    void OnEnable()
    {
        ItemType = ItemType.Furniture;
        CanStack = false;
        CanInBag = true;
        CanWear = false;
        CanHold = true;
        CanSell = true;
        CanUse = false;
        CanPlace = true;
        // 可旋转性根据家具类型设置
        CanRotate = (furnitureType != FurnitureType.Wallpaper && 
                     furnitureType != FurnitureType.Painting);
        // 可交互性根据家具类型设置
        CanInteractOnMap = (furnitureType == FurnitureType.Chair || 
                            furnitureType == FurnitureType.Bed);
        CanUseInCrafting = false;
        CanGift = false;
    }
}
public class QuestItem : ItemDetails
{
    public string questID; // 关联的任务ID
    public string targetNPC; // 需要交付给的NPC
    
    void OnEnable()
    {
        ItemType = ItemType.QuestItem;
        CanStack = true; // 根据具体物品设置
        CanInBag = true;
        CanWear = false;
        CanHold = true;
        CanSell = false;
        CanUse = false;
        CanPlace = false;
        CanRotate = false;
        CanInteractOnMap = false;
        CanUseInCrafting = false;
        CanGift = true; // 但只能赠送给特定NPC
    }
    
    // 检查是否可以赠送给特定NPC
    public bool CanGiftToNPC(string npcName)
    {
        return npcName == targetNPC;
    }
}