using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Pattern;
using Phenix.Unity.AI.SEARCH;
using System.Collections;

public class StageMgr : Singleton<StageMgr>
{    
    public StageConfigAsset stageConfigAsset;   // 关卡配置表    
    public StageGridAsset stageGridAsset;       // 关卡网格表
    public TerrainAsset terrainAsset;           // 地形表
    public HeroAsset heroAsset;                 // 英雄表
    public SkillAsset skillAsset;               // 技能表
    public RandomStageConfigAsset randomStageConfigAsset;   // 随机关卡配置表

    // 加载非随机关卡（数据取自StageGridAsset）
    public void LoadStage(int stageID)
    {
        StageConfig stageData = stageConfigAsset.GetData(stageID);
        // 创建网格
        HexGridAgent.Instance.gridParams = stageGridAsset.GetData(stageData.gridID);
        HexGridAgent.Instance.CreateGrid();

        // 加载地形        
        foreach (var item in stageData.terrains)
        {
            TerrainTemplate tp = terrainAsset.GetData(item.terrainType);
            if (tp == null)
            {
                continue;
            }
            HexTileAgent tileComp = HexGridAgent.Instance.TileCompFromCoords(item.coords);
            tileComp.terrainTP = tp;
            CreateTerrainObj(tileComp);
        }

        // 加载红方成员
        foreach (var item in stageData.redHeros)
        {
            CreateHero(item, CombatSide.RED, false);
        }

        // 加载蓝方成员
        foreach (var item in stageData.blueHeros)
        {
            CreateHero(item, CombatSide.BLUE, true);
        }
    }

    void CreateHero(HeroLayout heroLayout, CombatSide side, bool isAI)
    {
        // 创建实例
        HeroTemplate heroTP = heroAsset.GetData(heroLayout.heroID);
        GameObject go = GameObject.Instantiate(heroTP.prefab);
        HexTileAgent tileAgent = HexGridAgent.Instance.TileCompFromCoords(heroLayout.coords);
        go.transform.position = tileAgent.tile.Center;
        go.transform.forward = (side == CombatSide.RED ? Vector3.right : - Vector3.right);
        Hero hero = go.GetComponent<Hero>();
        hero.Init(heroTP, side, isAI);
        hero.tileComp = tileAgent;
        tileAgent.hero = hero;
        // 加入战斗
        CombatMgr.Instance.Join(hero);
    }

    // 随机关卡（数据取自RandomStageGridAsset）
    IEnumerator LoadRandomStage(int stageID)
    {
        bool success = false;
        while (success == false)
        {
            RandomStageConfig stageData = randomStageConfigAsset.GetData(1);
            // 创建网格
            HexGridAgent.Instance.gridParams = stageGridAsset.GetData(stageData.gridID);
            HexGridAgent.Instance.CreateGrid();

            // 默认地形列表
            List<HexTileAgent> plains = new List<HexTileAgent>(HexGridAgent.Instance.Tiles);

            List<HexTileAgent> woods = new List<HexTileAgent>();
            List<HexTileAgent> marshes = new List<HexTileAgent>();
            List<HexTileAgent> rivers = new List<HexTileAgent>();
            List<HexTileAgent> mountains = new List<HexTileAgent>();

            int totalTileCount = plains.Count; // 地图总tile数量
            TerrainTemplate terrainTP = null;            
            int maxCountPerChunk = totalTileCount * stageData.chunkMaxPercentSize / 100;
            foreach (var terrainLayout in stageData.terrains)
            {
                terrainTP = terrainAsset.GetData(terrainLayout.terrainType);
                int terrainCount = totalTileCount * terrainLayout.percent / 100;
                List<HexTileAgent> ret = new List<HexTileAgent>();
                switch (terrainLayout.terrainType)
                {
                    case TerrainType.MOUNTAIN:
                        ProcessRandomTerrain(terrainCount, maxCountPerChunk, terrainTP,
                            plains, mountains, ref ret);
                        break;
                    case TerrainType.RIVER:
                        ProcessRandomTerrain(terrainCount, maxCountPerChunk, terrainTP,
                            plains, rivers, ref ret);
                        break;
                    case TerrainType.MARSH:
                        ProcessRandomTerrain(terrainCount, maxCountPerChunk, terrainTP,
                            plains, marshes, ref ret);
                        break;
                    case TerrainType.WOODS:
                        ProcessRandomTerrain(terrainCount, maxCountPerChunk, terrainTP,
                            plains, woods, ref ret);
                        break;
                    default:
                        Debug.LogError("错误的地形类型！");
                        yield break;
                }
            }

            // 由于前面RandomTerrain使用了Status，所以此处要统一重置为NONE
            foreach (var item in HexGridAgent.Instance.Tiles)
            {
                item.Status = AStarNodeStatus.NONE;
            }
            
            if (HexGridAgent.Instance.CheckUnreachableButNonblockTiles())
            {
                Debug.Log("检测到‘孤岛’，重新生成地图。");
                yield return new WaitForEndOfFrame();                
            }
            else
            {
                RandomHeros(stageData.redHeroCount, stageData.blueHeroCount);
                Debug.Log("生成地图成功！");
                success = true;
            }            
        }

        CombatMgr.Instance.StartCombat();
    }

    void RandomHeros(int redCount, int blueCount)
    {
        HeroLayout heroLayout = new HeroLayout();
        int totalCount = redCount + blueCount;

        List<HexTileAgent> tiles = new List<HexTileAgent>();
        // 随机获得totalCount个非阻挡的不同tile
        HexGridAgent.Instance.RandomUnblockTiles(ref tiles, totalCount);
        if (tiles.Count == 0)
        {
            Debug.LogError("没有足够的非阻挡tile容纳heros！");
            return;
        }

        List<HeroTemplate> heros = new List<HeroTemplate>();
        // 随机获得totalCount个可重复的hero
        heroAsset.RandomHeros(ref heros, totalCount);
        if (heros.Count == 0)
        {
            Debug.LogError("随机产生hero失败！");
            return;
        }

        for (int i = 0; i < totalCount; i++)
        {
            heroLayout.coords = tiles[i].tile.Coords;
            heroLayout.heroID = heros[i].heroID;
            if (i < redCount)
            {
                CreateHero(heroLayout, CombatSide.RED, false);
            }
            else
            {
                CreateHero(heroLayout, CombatSide.BLUE, true);
            }
        }        
    }

    void ProcessRandomTerrain(int terrainCount/*计划生成的tile总数*/, int maxCountPerChunk/*每次最多生成的tile数量*/, 
        TerrainTemplate terrainTP, List<HexTileAgent> plains, List<HexTileAgent> terrains, ref List<HexTileAgent> ret)
    {
        // 为了避免生成的terrainCount过于集中，引入maxCountPerChunk，使得每次调用RandomTerrain最多生成maxCountPerChunk个地形。
        while (terrainCount >= maxCountPerChunk)
        {
            RandomTerrain(terrainTP, plains, maxCountPerChunk, ref ret);
            terrainCount -= maxCountPerChunk;
            foreach (var item in ret)
            {
                CreateTerrainObj(item);
                terrains.Add(item);
            }
        }

        if (terrainCount > 0)
        {
            RandomTerrain(terrainTP, plains, terrainCount, ref ret);
            foreach (var item in ret)
            {
                CreateTerrainObj(item);
                terrains.Add(item);
            }
        }                
    }

    void CreateTerrainObj(HexTileAgent tile)
    {        
        if (tile.terrainTP.prefab)
        {
            /*GameObject terrain = Instantiate(terrainTP.prefab,
                item.transform.position, Quaternion.identity, item.transform);*/
            // 为什么上面写法子对象的scale就会比例失调，下面就正常？？？
            GameObject terrain = Instantiate(tile.terrainTP.prefab);
            terrain.transform.SetParent(tile.transform);
            terrain.transform.position = tile.transform.position;            
            //terrain.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

            tile.terrainRenderers = terrain.GetComponentsInChildren<MeshRenderer>();
        }
    }

    void RandomTerrain(TerrainTemplate terrainTP, List<HexTileAgent> plains/*默认类型tile列表*/, int count,
        ref List<HexTileAgent> tilesWithTerrain/*地形tile列表*/)
    {
        tilesWithTerrain.Clear();
        List<HexTileAgent> neighbors = new List<HexTileAgent>();
        List<HexTileAgent> openTiles = new List<HexTileAgent>();

        HexTileAgent firstTile = null;
        
        while (tilesWithTerrain.Count < count)
        {
            if (openTiles.Count == 0)
            {
                if (plains.Count == 0)
                {
                    break;
                }
                // 从默认类型tile中随机选择一个
                firstTile = plains[Random.Range(0, plains.Count)];
                openTiles.Add(firstTile);
                firstTile.Status = AStarNodeStatus.CLOSED;
            }

            // 从开放列表中随机选择一个作为当前tile
            HexTileAgent cur = openTiles[Random.Range(0, openTiles.Count)];
            cur.terrainTP = terrainTP;  // 设置地形
            openTiles.Remove(cur);      // 从open列表移除
            plains.Remove(cur);         // 从默认地形中移除
            tilesWithTerrain.Add(cur);  // 添加到结果队列

            HexGridAgent.Instance.Neighbors(cur, ref neighbors);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.terrainTP != null)
                {
                    continue;
                }

                if (neighbor.Status == AStarNodeStatus.CLOSED)
                {
                    continue;
                }

                openTiles.Add(neighbor);
                neighbor.Status = AStarNodeStatus.CLOSED;
            }
        }        
    }    

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadRandomStage(1));
        //LoadStage(1);
        //CombatMgr.Instance.StartCombat();        
    }

    
}
