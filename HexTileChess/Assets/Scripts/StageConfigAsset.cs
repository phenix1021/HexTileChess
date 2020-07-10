using Phenix.Unity.Grid;
using System.Collections.Generic;
using UnityEngine;

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
