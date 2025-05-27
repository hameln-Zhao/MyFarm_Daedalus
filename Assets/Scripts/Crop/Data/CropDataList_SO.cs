using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CropDataList_SO", menuName = "Crop/CropData")]
public class CropDataList_SO : ScriptableObject
{
    public List<CropDetails> CropDetailsList;
}