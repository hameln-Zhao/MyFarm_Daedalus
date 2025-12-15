using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/Stats Config", fileName = "PlayerStatsConfig")]
public class PlayerStats_SO : ScriptableObject
{
    [Header("Max Values")]
    [Min(1)] public int maxHP = 100;
    [Min(0)] public int maxMP = 50;

    [Header("Start Values (runtime init)")]
    [Min(0)] public int startHP = 100;
    [Min(0)] public int startMP = 50;
}