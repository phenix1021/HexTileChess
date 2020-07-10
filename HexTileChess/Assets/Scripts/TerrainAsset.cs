using System.Collections.Generic;
using UnityEngine;

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
