using System.Collections.Generic;
using UnityEngine;

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
