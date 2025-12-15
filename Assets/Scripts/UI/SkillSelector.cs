using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SkillSelector : MonoBehaviour
{
    [Header("UI 绑定")]
    public Button btnUp;
    public Button btnDown;
    public Transform displayGroup;            // 父物体，用于动态挂 SkillItem
    public GameObject skillItemPrefab;        // SkillItem 预制体

    [Header("颜色设置")]
    public Color selectedColor = new Color(1, 1, 1, 0.8f);
    public Color unselectedColor = new Color(1, 1, 1, 0.3f);

    private List<Skill> skills = new List<Skill>();
    private int selectedIndex = 0;

    private List<GameObject> skillItemInstances = new List<GameObject>();

    void Start()
    {
        btnUp.onClick.AddListener(() => Scroll(-1));
        btnDown.onClick.AddListener(() => Scroll(1));

        // 初始化技能列表（从 SkillManager 中拿）
        skills = SkillManager.Instance.GetAllSkills();
        UpdateDisplay();
    }

    public void RefreshSkills()
    {
        skills = SkillManager.Instance.GetAllSkills();
        selectedIndex = Mathf.Clamp(selectedIndex, 0, skills.Count - 1);
        UpdateDisplay();
    }

    public void AddSkill(Skill newSkill)
    {
        SkillManager.Instance.AddSkill(newSkill);
        RefreshSkills();
    }

    public void RemoveSkill(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            SkillManager.Instance.RemoveSkill(skills[index].skillType);
            RefreshSkills();
        }
    }

    void Scroll(int direction)
    {
        if (skills.Count == 0) return;
        selectedIndex = (selectedIndex + direction + skills.Count) % skills.Count;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        ClearSkillItems();

        if (skills.Count == 0) return;

        List<int> indicesToShow = new List<int>();

        if (skills.Count == 1)
        {
            indicesToShow.Add(selectedIndex);
        }
        else if (skills.Count == 2)
        {
            indicesToShow.Add(selectedIndex);
            indicesToShow.Add((selectedIndex + 1) % skills.Count);
        }
        else
        {
            indicesToShow.Add((selectedIndex - 1 + skills.Count) % skills.Count);
            indicesToShow.Add(selectedIndex);
            indicesToShow.Add((selectedIndex + 1) % skills.Count);
        }
        Debug.Log("skill_num:"+skills.Count);
        for (int i = 0; i < indicesToShow.Count; i++)
        {
            int idx = indicesToShow[i];
            Skill skill = skills[idx];

            GameObject item = Instantiate(skillItemPrefab, displayGroup);
            skillItemInstances.Add(item);

            // 填内容
            Image icon = item.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI nameText = item.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            Image bg = item.transform.Find("Background").GetComponent<Image>();
            icon.sprite = skill.icon;
            //Debug.Log("skillname"+ skill.skillType.ToString());
            nameText.text= skill.skillType.ToString();
            nameText.fontSize = 12;
            bg.color = (i == 1 && skills.Count >= 3) ? selectedColor : unselectedColor;
        }
    }

    void ClearSkillItems()
    {
        foreach (var item in skillItemInstances)
        {
            Destroy(item);
        }
        skillItemInstances.Clear();
    }

    public int GetSelectedSkillIndex() => selectedIndex;
}
