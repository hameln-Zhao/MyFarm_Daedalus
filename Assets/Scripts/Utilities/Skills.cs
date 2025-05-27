using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class Skill
{
    public SkillType skillType; // 技能名称
    public int skillLevel;    // 技能等级
    public int skillMaxLevel;    // 技能最大等级
    public float cooldown;    // 冷却时间
    public float damage;      // 增加伤害值
    public float range;       // 作用范围
    public float manaCost;    // 消耗的魔法值

    public virtual void UpdateValues(){}

    public abstract void Execute();
}
public class ActionSkill : Skill
{
    // 技能效果方法
    public override void Execute()
    {
        // 基础技能效果
        Debug.Log("Executing skill: " + skillType);
    }
}
public class PassiveSkill : Skill
{
    // 技能效果方法
    public override void Execute()
    {
        // 基础技能效果
        Debug.Log("Executing skill: " + skillType);
    }
}
public static class SkillFactory
{
    public static Skill CreateSkill(SkillData skillData)
    {
        Skill skill = null;

        switch (skillData.skillType)
        {
            case SkillType.BasicFishing:
                skill = new BasicFishingSkill();
                ((BasicFishingSkill)skill).probabilityUp = ((BasicFishingSkillData)skillData).probabilityUp;
                break;
            default:
                throw new ArgumentException("Invalid skill type");
        }

        // 设置技能属性
        skill.skillType = skillData.skillType;
        skill.skillLevel = skillData.skillLevel;
        skill.skillMaxLevel = skillData.skillMaxLevel;
        skill.cooldown = skillData.cooldown;
        skill.damage = skillData.damage;
        skill.range = skillData.range;
        skill.manaCost = skillData.manaCost;

        return skill;
    }
}
public class BasicFishingSkill : PassiveSkill
{
    public float probabilityUp;
    // 存储每个等级对应的概率
    private Dictionary<int, float> levelProbabilityMap = new Dictionary<int, float>
    {
        { 1, 0.1f }, // 等级 1 的概率
        { 2, 0.2f }, // 等级 2 的概率
        { 3, 0.3f }, // 等级 3 的概率
    };
    //这个probabilityUp的值应该根据钓鱼的等级去浮动
    //钓鱼的所有东西的总处理都暴露出去 需要在fishmanager里获取到，去计算最终概率
    //技能效果方法
    public override void Execute()
    {
        // 基础技能效果
        Debug.Log("Executing skill: " + skillType);
    }
    public override void UpdateValues()
    {
        if (levelProbabilityMap.TryGetValue(skillLevel, out float probability))
        {
            probabilityUp = probability;
            Debug.Log($"Update skill: {skillType}, Level: {skillLevel}, Probability Up: {probabilityUp}");
        }
        else
        {
            Debug.LogWarning($"No probability defined for skill level: {skillLevel}");
        }
    }
}