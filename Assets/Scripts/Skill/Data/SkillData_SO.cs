using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 这个类为每个技能所需的基本数据
/// </summary>
public class SkillData : ScriptableObject
{
    public SkillType skillType; // 技能类型标识
    public int skillLevel;
    public int skillMaxLevel;
    public float cooldown=0f;
    public float damage=0f;
    public float range=0f;
    public float manaCost=0f;
}
/// <summary>
/// 用于存储技能数据，仅在初始化时使用。
/// </summary>
[CreateAssetMenu(fileName = "SkillDataList_SO", menuName = "Skill/SkillData")]
public class SkillData_SO : ScriptableObject
{
    public List<SkillData> SkillDataList = new List<SkillData>();
    // 根据技能名称查找 SkillData
    public SkillData GetSkillDataBySkillType(SkillType skillType)
    {
        return SkillDataList.Find(data => data.skillType == skillType);
    }
}
