using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public InventoryBag_SO shopData;
    private bool isOpen;

    private void Update()
    {
        if (isOpen&&Input.GetKeyDown(KeyCode.Escape))
        {
            //关闭背包
            CloseShop();
        }
    }
    /// <summary>
    /// 打开商店
    /// </summary>
    public void OpenShop()
    {
        isOpen = true;
        EventHandler.CallBaseBagOpenEvent(SlotType.Shop,shopData);
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);
    }
    /// <summary>
    /// 关闭商店
    /// </summary>
    public void CloseShop()
    {
        isOpen = false;
        EventHandler.CallBaseBagCloseEvent(SlotType.Shop, shopData);
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    }
}

//