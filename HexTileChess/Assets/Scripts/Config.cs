using UnityEngine;
using Phenix.Unity.Grid;
using System.Collections.Generic;

// 网格配置文件
[CreateAssetMenu(fileName = "stageGrid", menuName = "XXX Game/StageGrid")]
public class StageGridAsset : ScriptableObject
{
    public List<StageGrid> grids = new List<StageGrid>();

    public HexGridParams GetData(int gridID)
    {
        foreach (var grid in grids)
        {
            if (grid.gridID == gridID)
            {
                return grid.gridParams;
            }
        }
        return null;
    }
}

// 关卡配置文件
[CreateAssetMenu(fileName = "stageConfig", menuName = "XXX Game/StageConfig")]
public class StageConfigAsset : ScriptableObject
{
    public List<StageConfig> stages = new List<StageConfig>();

    public StageConfig GetData(int stageID)
    {
        foreach (var stage in stages)
        {
            if (stage.stageID == stageID)
            {
                return stage;
            }
        }
        return null;
    }
}

// 地形表
[CreateAssetMenu(fileName = "terrainAsset", menuName = "XXX Game/Terrain")]
public class TerrainAsset : ScriptableObject
{
    public List<TerrainTemplate> terrains = new List<TerrainTemplate>();

    public TerrainTemplate GetData(TerrainType terrainType)
    {
        foreach (var terrain in terrains)
        {
            if (terrain.terrainType == terrainType)
            {
                return terrain;
            }
        }
        return null;
    }
}

// 英雄表
[CreateAssetMenu(fileName = "heroAsset", menuName = "XXX Game/Hero")]
public class HeroAsset : ScriptableObject
{
    public List<HeroTemplate> heros = new List<HeroTemplate>();

    public HeroTemplate GetData(int heroID)
    {
        foreach (var hero in heros)
        {
            if (hero.heroID == heroID)
            {
                return hero;
            }
        }
        return null;
    }

    public void RandomHeros(ref List<HeroTemplate> ret, int count)
    {
        ret.Clear();
        for (int i = 0; i < count; i++)
        {
            ret.Add(heros[Random.Range(0, heros.Count)]);
        }
    }
}

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

// 随机关卡配置文件
[CreateAssetMenu(fileName = "randomStageConfig", menuName = "XXX Game/RandomStageConfig")]
public class RandomStageConfigAsset : ScriptableObject
{
    public List<RandomStageConfig> stages = new List<RandomStageConfig>();

    public RandomStageConfig GetData(int stageID)
    {
        foreach (var stage in stages)
        {
            if (stage.stageID == stageID)
            {
                return stage;
            }
        }
        return null;
    }
}