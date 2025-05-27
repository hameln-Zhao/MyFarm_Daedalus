using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 这个类存放所有技能需要的参数
/// </summary>
[CreateAssetMenu(fileName = "BasicFishingSkillData", menuName = "Skill/BasicSkills/BasicFishingSkillData")]
public class BasicFishingSkillData : SkillData
{
    public float probabilityUp; // 钓鱼技能特有的字段
}