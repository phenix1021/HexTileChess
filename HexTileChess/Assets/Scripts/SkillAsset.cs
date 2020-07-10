using System.Collections.Generic;
using UnityEngine;

// 技能表
[CreateAssetMenu(fileName = "skillAsset", menuName = "XXX Game/Skill")]
public class SkillAsset : ScriptableObject
{
    public List<SkillTemplate> skills = new List<SkillTemplate>();

    public SkillTemplate GetData(int skillID)
    {
        foreach (var skill in skills)
        {
            if (skill.skillID == skillID)
            {
                return skill;
            }
        }
        return null;
    }
}
