using System.Collections.Generic;
using UnityEngine;
using Phenix.Unity.Grid;

public class HexGridAgent : HexGridComponent<HexTileAgent>
{
    public static HexGridAgent Instance { get; private set; }

    public HexTileAgent TileAgentOnPointer { get; private set; }

    [SerializeField]
    GameObject _tileFlagPrefab;     // tile标识（如当前hero、可到达tile、可选择敌人、可攻击地点）prefab
    [SerializeField]
    Material _selectedFlagMat, _reachableFlagMat, _attackableFlagMat, _attackPointFlagMat;

    List<HexTileAgent> _reachableTiles = new List<HexTileAgent>();
    List<HexTileAgent> _attackableTiles = new List<HexTileAgent>();
    List<HexTileAgent> _attackPointTiles = new List<HexTileAgent>();

    TileFlagMgr _tileFlagMgr;
    HexTileAgent _preAttackTarget;

    protected virtual void Awake()
    {
        Instance = this;

        _tileFlagMgr = new TileFlagMgr(_tileFlagPrefab, _selectedFlagMat, _reachableFlagMat,
            _attackableFlagMat, _attackPointFlagMat);
    }

    protected override bool IsBlock(HexTileAgent hexTileComponent)
    {
        return (hexTileComponent.terrainTP != null && hexTileComponent.terrainTP.isBlock);
    }

    protected override bool IsWalkable(HexTileAgent hexTileComponent)
    {
        if (hexTileComponent.terrainTP != null && hexTileComponent.terrainTP.isBlock)
        {
            // 有地形障碍物
            return false;
        }

        if (hexTileComponent.hero)
        {
            // 有hero
            return false;
        }
        
        return true;
    }

    protected override void OnTileCreated(HexTileAgent tileComp)
    {
        tileComp.terrainTP = null;        
    }

    private void Update()
    {
        TileAgentOnPointer = CheckTouch();        
        if (TileAgentOnPointer != null)
        {
            Debug.Log(string.Format("coords: {0}, pos: {1}", TileAgentOnPointer.tile.Coords,
                TileAgentOnPointer.tile.Center));
            //var mat = TileAgentOnPointer.transform.GetChild(0).GetComponent<MeshRenderer>().material;
            //Debug.Log("");
        }
    }

    HexTileAgent CheckTouch()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit, 1000, 1 << LayerMask.NameToLayer(gridParams.tileLayerName)))
            {
                return TileCompFromPosition(hit.point);                
            }
        }
        return null;
    }

    // 是否在可达列表里
    public bool InReachableList(HexTileAgent tileOnPointer)
    {
        if (tileOnPointer == null)
        {
            return false;
        }

        foreach (var item in _reachableTiles)
        {
            if (item.transform.position == tileOnPointer.tile.Center)
            {
                return true;
            }
        }
        return false;
    }

    // 是否在攻击点列表里
    public bool InAttackPointList(HexTileAgent tileOnPointer)
    {
        if (tileOnPointer == null)
        {
            return false;
        }

        foreach (var item in _attackPointTiles)
        {
            if (item.transform.position == tileOnPointer.tile.Center)
            {
                return true;
            }
        }
        return false;
    }

    // 是否在可攻击对象里
    public bool InAttackableList(HexTileAgent tileOnPointer)
    {
        if (tileOnPointer == null)
        {
            return false;
        }

        foreach (var item in _attackableTiles)
        {
            if (item.transform.position == tileOnPointer.tile.Center)
            {
                return true;
            }
        }
        return false;
    }

    // 绘制选中tile
    public void ShowSelected()
    {
        if (CombatMgr.Instance.CurClient == null)
        {
            return;
        }
        _tileFlagMgr.CreateSelectedFlag(CombatMgr.Instance.CurClient.tileComp.tile.Center);
        //CombatMgr.Instance.CurClient.tileComp.flagRenderer.material = _selectedFlagMat;
    }

    // 绘制可以到达的tile
    public void ShowAllReachable(bool hide = false)
    {
        if (CombatMgr.Instance.CurClient == null)
        {
            return;
        }

        if (hide)
        {
            _tileFlagMgr.HideReachable();
        }
        else 
        {   
            if (_reachableTiles.Count == 0)
            {
                GetTilesReachable(ref _reachableTiles);                
            }
            _tileFlagMgr.CreateReachableFlags(ref _reachableTiles);
        }
    }

    // 绘制可以攻击的tile
    public void ShowAllAttackable(bool hide = false)
    {
        if (CombatMgr.Instance.CurClient == null)
        {
            return;
        }

        if (hide)
        {
            _tileFlagMgr.HideAttackable();            
        }
        else
        {
            if (_attackableTiles.Count == 0)
            {
                GetTilesAttackable(ref _attackableTiles);
            }
            _tileFlagMgr.CreateAttackableFlags(ref _attackableTiles);
        }
    }

    // 绘制备选攻击方位
    public void ShowAllAttackPoint(HexTileAgent attackTarget, bool hide = false)
    {
        if (CombatMgr.Instance.CurClient == null)
        {
            return;
        }

        if (hide)
        {
            _tileFlagMgr.HideAttackPoint();
        }
        else
        {
            if (_attackPointTiles.Count == 0 || _preAttackTarget != attackTarget)
            {
                GetTilesAttackPoint(attackTarget, ref _attackPointTiles);
                _preAttackTarget = attackTarget;
            }
            _tileFlagMgr.CreateAttackPointFlags(ref _attackPointTiles);
        }
    }

    // 绘制选中攻击目标
    public void ShowAttackTarget(HexTileAgent attackTarget)
    {
        if (CombatMgr.Instance.CurClient == null)
        {
            return;
        }

        _tileFlagMgr.CreateAttackableFlag(attackTarget.tile.Center);
    }

    // 删除所有标志对象
    public void ClearAllTileFlags()
    {
        _tileFlagMgr.HideAll();
        
        _reachableTiles.Clear();
        _attackableTiles.Clear();
        _attackPointTiles.Clear();
    }

    void GetTilesReachable(ref List<HexTileAgent> tiles)
    {
        tiles.Clear();
        if (CombatMgr.Instance.CurClient.tp.canFly)
        {
            List<HexTileAgent> tmp = new List<HexTileAgent>();
            TilesInRange(CombatMgr.Instance.CurClient.tileComp,
                CombatMgr.Instance.CurClient.tp.move, ref tmp);
            tiles.Clear();
            foreach (var item in tmp)
            {
                if (item.hero == null && IsBlock(item) == false)
                {
                    tiles.Add(item);
                }
            }
        }
        else
        {
            ReachableTilesInRange(CombatMgr.Instance.CurClient.tileComp,
                CombatMgr.Instance.CurClient.tp.move, ref tiles);
        }        
    }

    void GetTilesAttackable(ref List<HexTileAgent> tiles)
    {
        tiles.Clear();
        Hero hero = CombatMgr.Instance.CurClient;
        SkillTemplate skillTP = hero.CurSkill;
        List<HexTileAgent> path = new List<HexTileAgent>();
        foreach (var client in CombatMgr.Instance.Clients)
        {
            if (client.IsQuit)
            {
                // 忽略已经出局的hero
                continue;
            }

            if (client.side == hero.side)
            {
                // 暂不考虑队友
                continue;                
            }

            if (skillTP.useRange == 1)
            {
                // 近战技能                
                List<HexTileAgent> attackPoints = new List<HexTileAgent>();
                GetTilesAttackPoint(client.tileComp, ref attackPoints);
                if (attackPoints.Count == 0)
                {
                    continue;
                }
            }
            else
            {
                // 远程技能
                int dist = Distance(hero.tileComp, client.tileComp);
                if (dist > skillTP.useRange)
                {
                    continue;
                }
            }

            tiles.Add(client.tileComp);
        }
    }

    // 获得近程攻击（攻击距离==1）的攻击位置点
    void GetTilesAttackPoint(HexTileAgent attackTarget, ref List<HexTileAgent> tiles)
    {
        tiles.Clear();
        Hero hero = CombatMgr.Instance.CurClient;
        List<HexTileAgent> neighbors = new List<HexTileAgent>();
        WalkableNeighbors(attackTarget, ref neighbors);
        if (Distance(CombatMgr.Instance.CurClient.tileComp, attackTarget) == 1)
        {
            neighbors.Add(CombatMgr.Instance.CurClient.tileComp);
        }
        List<HexTileAgent> path = new List<HexTileAgent>();
        foreach (var neighbor in neighbors)
        {
            if (hero.tp.canFly)
            {
                // 飞行
                if (Distance(hero.tileComp, neighbor) > hero.tp.move)
                {
                    continue;
                }
            }
            else
            {
                // 行军
                bool hasPath = FindPath(hero.tileComp, neighbor, ref path);
                if (hasPath == false || path.Count > hero.tp.move)
                {
                    continue;
                }
            }
            
            tiles.Add(neighbor);
        }
    }
}
