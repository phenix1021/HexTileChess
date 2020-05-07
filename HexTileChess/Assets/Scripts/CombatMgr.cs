using UnityEngine;
using Phenix.Unity.TurnBased;

public enum CombatSide
{
    NONE = 0,
    RED = 1,
    BLUE = 2,
}

public enum CombatResult
{
    NONE = 0,
    EVEN,       // 平手
    RED_WIN,    // 红方胜
    BLUE_WIN,   // 蓝方胜
}

public class CombatMgr : TurnBasedMgr<Hero/*, HeroProp*/>
{
    public static CombatMgr Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = this;

        onCombatBegin = OnCombatBegin;
        onCombatEnd = OnCombatEnd;
        onTurnBegin = OnTurnBegin;
        onTurnEnd = OnTurnEnd;
        onResultJudge = JudgeCombatResult;
    }    

    void OnCombatBegin()
    {
        Debug.Log("战斗开始");
    }

    void OnCombatEnd()
    {
        Debug.Log("战斗结束");
    }

    void OnTurnBegin(int turnID)
    {
        Debug.Log(string.Format("第{0}回合开始", turnID));
    }

    void OnTurnEnd(int turnID)
    {
        Debug.Log(string.Format("第{0}回合结束", turnID));
    }

    int JudgeCombatResult()
    {
        int remainRedCount = 0, remainBlueCount = 0;

        foreach (var client in Clients)
        {
            if (client.IsQuit)
            {
                continue;
            }

            if (client.side == CombatSide.RED)
            {
                ++remainRedCount;
            }
            else if (client.side == CombatSide.BLUE)
            {
                ++remainBlueCount;
            }
        }

        if (remainRedCount == 0 && remainBlueCount > 0)
        {
            return (int)CombatResult.BLUE_WIN;
        }
        else if (remainBlueCount == 0 && remainRedCount > 0)
        {
            return (int)CombatResult.RED_WIN;
        }
        else if (remainBlueCount == 0 && remainRedCount == 0)
        {
            return (int)CombatResult.EVEN;
        }

        return 0;
    }    
}
