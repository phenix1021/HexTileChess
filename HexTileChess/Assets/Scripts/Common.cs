using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Grid;

[System.Serializable]
public class HeroTemplate
{
    public int heroID;            // 类型ID
    public int groupID;           // 组ID（如物理系、魔法火系、魔法水系等）
    public string name;
    public string desc;
    public GameObject prefab;
    public float speed;           // 移动速度
    public int attack;            // 攻击力    
    public int defend;            // 防御力
    public int move;              // 机动力
    public bool canFly;           // 能否飞行
    public int maxFury = 3;       // 最大怒气值（可释放大招）
    public int counterCount;      // 一回合中允许自动反击的次数
    public int normalSkill;       // 普通攻击技能ID
    public int furySkill;         // 放大招攻击技能ID
    public int counterSkill;      // 反击技能ID
}

// 技能使用类型
[System.Serializable]
public enum SkillUseType
{
    NONE = 0,
    SIMPLE,         // 简单技能（如物理近战、远程魔法）
    CAST,           // 投掷技能（如弓箭等依靠投掷物攻击）
    COMPLEX,        // 复杂技能（如龙息）
}

[System.Serializable]
public class SkillTemplate
{
    public int skillID;         // 技能类型ID
    public string name;         // 名称
    public string desc;         // 描述    
    public int useRange = 1;    // 最大使用范围(以施用者为中心)    
    public int effectRange = 0; // 作用效果范围(以目标为中心，0表示只作用于目标)
    public float effectVal;     // 效果值（损益值或增益值）
    public float animPrepareTime;       // 动画播放前的准备时长（秒）
    public float animEffectStartTime;   // 从动画开始播放到hit结算的时长（秒）
    public float animEffectKeepTime;    // hit持续时长（秒）
    public float animFinalizeTime;      // hit结算之后直到command结束的时长（秒）
    public GameObject projectile;   // 投掷物prefab
    public GameObject fx;           // 技能特效prefab
    public GameObject fxImpact;     // 技能受击特效prefab
}

[System.Serializable]
public class TerrainTemplate
{
    public TerrainType terrainType; // 类型ID    
    public GameObject prefab;       // prefab
    public int heroGroupID;         // 影响的英雄组ID
    public bool isBlock;            // 是否障碍物
    public float deltaAttack;       // 攻击力增益
    public float deltaAttackRange;  // 攻击距离增益
    public float deltaDefend;       // 防御力增益
    public float deltaMove;        // 机动力增益
}

// 地形
[System.Serializable]
public enum TerrainType
{
    NONE = 0,
    MOUNTAIN = 1,       // 山地（障碍物）
    RIVER = 2,          // 河流
    MARSH = 3,          // 沼泽
    WOODS = 4,          // 树林    
}

// 地形分布数据
[System.Serializable]
public class TerrainLayout
{
    public HexCoordinates coords;
    public TerrainType terrainType;
}

// 随机地形分布数据
[System.Serializable]
public class RandomTerrainLayout
{    
    public TerrainType terrainType;
    public int percent;
}

// 英雄分布数据
[System.Serializable]
public class HeroLayout
{
    public HexCoordinates coords;
    public int heroID;
}

// 关卡配置数据
[System.Serializable]
public class StageConfig
{
    public int stageID;
    public int gridID;                   // 对应LevelGrid中的gridID
    public List<TerrainLayout> terrains; // 地形分布
    public List<HeroLayout> redHeros;    // 红方hero分布
    public List<HeroLayout> blueHeros;   // 蓝方hero分布
}

// 随机关卡配置数据
[System.Serializable]
public class RandomStageConfig
{
    public int stageID;
    public int gridID;   // 对应LevelGrid中的gridID
    public List<RandomTerrainLayout> terrains; // 地形分布
    public int chunkMaxPercentSize;
    public int redHeroCount;    // 红方英雄数量
    public int blueHeroCount;   // 蓝方英雄数量
}

// 关卡网格数据
[System.Serializable]
public class StageGrid
{
    public int gridID;
    public HexGridParams gridParams;// basePose约定为tag==HexGridBase的对象
}