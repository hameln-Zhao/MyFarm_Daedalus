using System.Collections.Generic;
using UnityEngine;
public class ItemDataList_SO<T> : ScriptableObject where T : ItemDetails
{
    public List<T> ItemDetailsList;
}

//一个用于持有所有数据SO的类
[CreateAssetMenu(fileName = "ItemContainer_SO", menuName = "Inventory/ItemContainer")]
public class ItemContainer_SO : ScriptableObject
{
    public ConsumableData_SO consumables;
    public EquipmentData_SO equipments;
}

