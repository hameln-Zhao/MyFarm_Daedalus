using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    // 存储技能数据的 ScriptableObject
    public SkillData_SO SkillDataSO;
    // 存储运行时技能实例的列表
    private List<Skill> SkillList = new List<Skill>();

    private void Start()
    {
        InitSkills();
        //通过这样的方式在游戏过程中加入技能
        //SkillManager.Instance.AddDynamicSkill(SkillDataSO.GetSkillDataByName("Fireball"));
    }

    /// <summary>
    /// 初始化技能
    /// </summary>
    public void InitSkills()
    {
        foreach (var skillData in SkillDataSO.SkillDataList)
        {
            Skill skill = SkillFactory.CreateSkill(skillData);
            Debug.Log($"Created skill: {skill.skillType}, Type: {skillData.skillType}");
            AddSkill(skill);
        }
    }

    // 添加技能
    public void AddSkill(Skill skill)
    {
        if (skill != null && !SkillList.Contains(skill))
        {
            SkillList.Add(skill);
            Debug.Log("Added skill: " + skill.skillType);
        }
    }
    /// <summary>
    /// 添加游戏中需要加入的技能
    /// </summary>
    /// <param name="skillData"></param>
    public void AddDynamicSkill(SkillData skillData)
    {
        Skill skill = SkillFactory.CreateSkill(skillData);
        AddSkill(skill);
        Debug.Log($"Dynamically added skill: {skill.skillType}");
    }
    // 升级技能
    public void LevelUpSkill(SkillType skillType)
    {
        Skill skill = SkillList.Find(s => s.skillType == skillType);
        if (skill != null)
        {
            skill.skillLevel++; // 提升等级
            skill.UpdateValues(); // 更新数值
            Debug.Log($"Leveled up {skillType} to Level {skill.skillLevel}");
        }
        else
        {
            Debug.LogWarning($"Skill not found: {skillType}");
        }
    }

    // 降级技能
    public void LevelDownSkill(SkillType skillType)
    {
        Skill skill = SkillList.Find(s => s.skillType == skillType);
        if (skill != null)
        {
            skill.skillLevel--; // 降低等级
            skill.UpdateValues(); // 更新数值
            Debug.Log($"Leveled down {skillType} to Level {skill.skillLevel}");
        }
        else
        {
            Debug.LogWarning($"Skill not found: {skillType}");
        }
    }

    // 移除技能
    public void RemoveSkill(SkillType skillType)
    {
        Skill skillToRemove = SkillList.Find(s => s.skillType == skillType);
        if (skillToRemove != null)
        {
            SkillList.Remove(skillToRemove);
            Debug.Log("Removed skill: " + skillType);
        }
        else
        {
            Debug.LogWarning("Skill not found: " + skillType);
        }
    }

    // 获取所有技能
    public List<Skill> GetAllSkills()
    {
        return SkillList;
    }

    // 根据名称获取技能
    public T GetSkillBySkillType<T>(SkillType skillType) where T : Skill
    {
        return SkillList.Find(s => s.skillType == skillType) as T;
    }
    // 获取技能数量
    public int GetSkillCount()
    {
        return SkillList.Count;
    }

    // 打印所有技能信息
    public void PrintAllSkills()
    {
        Debug.Log("Current SkillList:");
        foreach (var skill in SkillList)
        {
            Debug.Log($"- {skill.skillType} (Level {skill.skillLevel})");
        }
    }
}