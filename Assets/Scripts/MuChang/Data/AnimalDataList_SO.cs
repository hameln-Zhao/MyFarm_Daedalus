using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalDetailList", menuName = "Animal/AnimalData")]
public class AnimalDataList_SO : ScriptableObject
{
    public List<AnimalDetails> AnimalDetailList;
}
