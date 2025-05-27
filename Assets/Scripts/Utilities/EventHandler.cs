using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyFarm.Dialogue;
public static class EventHandler
{
    public static event Action<InventoryLocation,List<InventoryItem> > UpdataInventoryUI;
    public static void CallUpdataInventoryUI(InventoryLocation inventoryLocation, List<InventoryItem> inventoryItems)
    {
        UpdataInventoryUI?.Invoke(inventoryLocation, inventoryItems);
    }
    
    public static event Action<int,Vector3 > InstaniateItemInScene;
    public static void CallInstaniateItemInScene(int ID, Vector3 pos)
    {
        InstaniateItemInScene?.Invoke(ID, pos);
    }

    public static event Action<int, Vector3> DropItemEvent;

    public static void CallDropItemEvent(int ID, Vector3 pos)
    {
        DropItemEvent?.Invoke(ID, pos);
    }
    
    public static event Action<ItemDetails,bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }

    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }
    
    public static event Action<int, int> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour)
    {
        GameMinuteEvent?.Invoke(minute, hour);
    }
    
    public static event Action<int, int,int,int,Season> GameDateEvent;
    public static void CallGameDateEvent(int hour, int day, int month, int year,Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }
    
    public static event Action<string,Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }

    
    public static event Action BeforeSceneEvent;
    public static void CallBeforeSceneEvent()
    {
        BeforeSceneEvent?.Invoke();
    }
    
    public static event Action AfterSceneEvent;
    public static void CallAfterSceneEvent()
    {
        AfterSceneEvent?.Invoke();
    }
    
    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 pos)
    {
        MoveToPosition?.Invoke(pos);
    }
    
    public static event Action<Vector3,ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }
    public static event Action<ItemDetails> MousePressEvent;
    public static void CallMousePressEvent(ItemDetails itemDetails)
    {
        MousePressEvent?.Invoke(itemDetails);
    }
    public static event Action<Vector3,ItemDetails> MouseReleaseEvent;
    public static void CallMouseReleaseEvent(Vector3 pos,ItemDetails itemDetails)
    {
        MouseReleaseEvent?.Invoke(pos,itemDetails);
    }
    /// <summary>
    /// 基本上都是更改地图信息的操作
    /// </summary>
    public static event Action<Vector3,ItemDetails> ExcuteActionAfterAnimation;
    public static void CallExcuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExcuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }
    /// <summary>
    /// 更新种子所在地块
    /// </summary>
    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID,TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(ID,tileDetails);
    }
    /// <summary>
    /// 在玩家位置生成物品
    /// </summary>
    public static event Action<int> HarvestAtPlayerPosition;
    public static void CallHarvestAtPlayerPosition(int ID)
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }

    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }
    
    public static event Action<DialoguePiece> ShowDialogEvent;
    public static void CallShowDialogEvent(DialoguePiece dialoguePiece)
    {
        ShowDialogEvent?.Invoke(dialoguePiece);
    }
    
    public static event Action<SlotType,InventoryBag_SO> BaseBagOpenEvent;
    public static void CallBaseBagOpenEvent(SlotType slotType,InventoryBag_SO bagData)
    {
        BaseBagOpenEvent?.Invoke(slotType,bagData);
    }
    
    public static event Action<SlotType,InventoryBag_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SlotType slotType,InventoryBag_SO bagData)
    {
        BaseBagCloseEvent?.Invoke(slotType,bagData);
    }
    
    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }

    public static event Action<ItemDetails, bool> ShowTradeUI;
    public static void CallShowTradeUI(ItemDetails itemDetails,bool isSell)
    {
        ShowTradeUI?.Invoke(itemDetails,isSell);
    }
    //灯光
    public static event Action<Season, LightShift, float> LightShiftChangeEvent;
    public static void CallLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        LightShiftChangeEvent?.Invoke(season,lightShift,timeDifference);
    }
    //跳转选项
    public static event Action<string> ChangeDialogue;
    public static void CallChangeDialogue(string PieceID)
    {
        ChangeDialogue?.Invoke(PieceID);
    }
    //结束对话
    public static event Action EndDialogEvent;
    public static void CallEndDialogEvent()
    {
        EndDialogEvent?.Invoke();
    }
    //更新需求UI
    public static event Action<QuestData_SO> UpdateQuestRequireEvent;
    public static void CallUpdateQuestRequireEvent(QuestData_SO questData)
    {
        UpdateQuestRequireEvent?.Invoke(questData);
    }
    //更新奖励UI
    public static event Action<ItemDetails,int> UpdateQuestRewardEvent;
    public static void CallUpdateQuestRewardEvent(ItemDetails itemDetails,int amount)
    {
       UpdateQuestRewardEvent?.Invoke(itemDetails,amount);
    }
    
    //作物生长吸收魔素
    public static event Action<TileDetails, int> ApplyMagicEvent;

    public static void CallApplyMagicEvent(TileDetails tileDetails, int magicAmount)
    {
        ApplyMagicEvent?.Invoke(tileDetails,magicAmount);
    }

    public static event Action<int, Vector3> PlaceAnimalEvent;

    public static void CallPlaceAnimalEvent(int animalID, Vector3 pos)
    {
        PlaceAnimalEvent?.Invoke(animalID,pos);
    }
    
    public static event Action<Animal,int> ApplyMagicToAnimalEvent;

    public static void CallApplyMagicToAnimalEvent(Animal animal, int magicAmount)
    {
        ApplyMagicToAnimalEvent?.Invoke(animal,magicAmount);
    }
    //执行将要扔竿的逻辑
    public static event Action FishRodAccumulation;
    public static void CallFishRodAccumulation()
    {
        FishRodAccumulation?.Invoke();
    }
    //执行扔出鱼竿逻辑，开启钓鱼
    public static event Action<Vector3> ThrowFishRod;
    public static void CallThrowFishRod(Vector3 mouseWorldPos)
    {
        ThrowFishRod?.Invoke(mouseWorldPos);
    }
    //执行等待鱼上钩
    public static event Action<Transform> StartingFishing;
    public static void CallStartingFishing(Transform fishingFloatingTrans)
    {
        StartingFishing?.Invoke(fishingFloatingTrans);
    }
    //执行结束钓鱼
    public static event Action FinishFishing;
    public static void CallFinishFishing()
    {
        FinishFishing?.Invoke();
    }
    //进行钓鱼
    public static event Action GoFishing;
    public static void CallGoFishing()
    {
        GoFishing?.Invoke();
    }
}
