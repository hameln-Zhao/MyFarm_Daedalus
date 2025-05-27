using UnityEngine;

[CreateAssetMenu(fileName = "NewFishDifficulty", menuName = "Fishing/Fish Difficulty")]
public class FishDifficulty_SO : ScriptableObject
{
    public int fishLevel;                  // 鱼的等级
    public float moveIntervalMin = 1f;     // 方块移动间隔（最小值）
    public float moveIntervalMax = 3f;     // 方块移动间隔（最大值）
    public float moveSpeedMin = 2f;        // 方块移动速度（最小值）
    public float moveSpeedMax = 5f;        // 方块移动速度（最大值）
    public Vector2 moveAreaSize = new Vector2(3, 3); // 移动区域范围（越小难度越高）
}