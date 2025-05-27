using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FishDataList_SO", menuName = "Fish/FishData")]
public class FishDataList_SO : ScriptableObject
{
    public List<FishDetails> fishDataList;
}