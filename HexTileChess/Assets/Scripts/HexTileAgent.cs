using UnityEngine;
using Phenix.Unity.Grid;
using Phenix.Unity.AI.SEARCH;

public class HexTileAgent : HexTileComponent, AStarNode<HexTileAgent>
{
    public TerrainTemplate terrainTP = null;
    public Hero hero;    
    public MeshRenderer[] terrainRenderers;    // 地形子对象的MeshRenderer组件

    public HexTileAgent Parent { get; set; }
    public AStarNodeStatus Status { get; set; }
    public float G { get; set; }
    public int PathCacheIdx { get; set; }

    bool _isTransparent = false;

    void Update()
    {        
        if (terrainRenderers == null || terrainTP == null)
        {
            return;
        }

        DoTerrainTransparent();             
    }

    void DoTerrainTransparent()
    {
        if (terrainTP.terrainType == TerrainType.WOODS)
        {
            if (hero)
            {
                if (_isTransparent == false)
                {
                    // 不透明 =》透明
                    foreach (var renderer in terrainRenderers)
                    {
                        // SetColor也行，但要注意SetColor的shader参数（如“_Color”）必须存在，否则无效
                        renderer.material.color = new Color(renderer.material.color.r,
                            renderer.material.color.g, renderer.material.color.b, 50f/255);
                    }
                    _isTransparent = true;
                }
            }
            else
            {
                if (_isTransparent)
                {
                    // 透明 =》不透明
                    foreach (var renderer in terrainRenderers)
                    {
                        // .color = ***; 也行
                        renderer.material.SetColor("_Color", new Color(renderer.material.color.r,
                            renderer.material.color.g, renderer.material.color.b, 255f / 255));
                    }
                    _isTransparent = false;
                }
            }
        }        
    }
}
