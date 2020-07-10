using System.Collections.Generic;
using UnityEngine;


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