using UnityEngine;
using Phenix.Unity.Grid;

/// <summary>
/// 部队类型
/// </summary>
[System.Serializable]
public enum ArmyType
{
    SOLDIER = 0,    // 步兵
    HORSE = 1,      // 骑兵
    BOW = 2,        // 弩兵
    WHEEL = 3,      // 车兵
    STONE = 4,      // 投石车
    BIRD = 5,       // 机关鸟
}

public class Army
{
    public static int idGenerator = 0;  // id产生器

    public int ArmyID { get; private set; }
    public int InstID { get; private set; }

    public Hero Hero { get; set; } // 统帅

    public int hp;      // 生命值
    public int mp;      // 行动力    
    public int combat;  // 士气

    public Army(int armyID)
    {
        InstID = ++idGenerator;
        ArmyID = armyID;
    }
}
