using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableDataList_SO", menuName = "Inventory/Containers/Consumable")]
public class ConsumableData_SO : ItemDataList_SO<Consumable>
{
    public FishDataList_SO fishDataList_SO;
}