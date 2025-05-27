/*public enum ItemType
{
    Seed,Commodity,Furniture,Clothing,Produce,Collection,Pet,Equipment,
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,FishingTool
}*/
public enum ItemType
{
    Apparel,        // 装扮
    BuffModule,     // 加成插件
    Pet,            // 宠物/动物
    Equipment,      // 装备
    EquipmentPart,  // 装备配件
    Consumable,     // 消耗品
    Furniture,      // 家具
    QuestItem,      // 非卖品
    Miscellaneous   // 其他
}


/// <summary>
/// 工具类型
/// </summary>
public enum ToolType
{
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,FishingTool
}
/// <summary>
/// 稀有度
/// </summary>
public enum Rarity
{
    Common, Rare
}
public enum SlotType
{
    Bag,Box,Shop
}

public enum InventoryLocation
{
    Player,Box
}

public enum PartType
{
    None,Carry,Hoe,Water,Break,Fish
}

public enum PartName
{
    Body,Hair,Arm,Tool
}

public enum Season
{
    春天,夏天,秋天,冬天
}

public enum Weather
{
    晴天,雨天,暴雨,雪天,暴雪
}
public enum GridType
{
    //herdable 畜牧
     Diggable,DropItem,PlaceFurniture,NPCObstacle,FishArea,Herdable
}
//星露谷里那种打开商店人物不能移动 时间却没暂停的是第三种状态吧
public enum GameState
{
    GamePlay,Pause,PlayerStop
}
//钓鱼的阶段状态
public enum FishState
{
    Empty,Charge,Throw,WaitForFish,Playing,Reap,
}

public enum LightShift
{
    Morning,Night,
}

public enum SoundName
{
    none, FootStepSoft, footStepHard,
    Axe, Pickaxe, Hoe, Reap, Water, Basket, Chop,
    Pickup, Plant, TreeFalling, Rustle,
    AmbientCountryside1, AmbientCountryside2, MusicCalm1, MusicCalm2, MusicCalm3, MusicCalm4, MusicCalm5, MusicClam6, AmbientIndoor1,
}