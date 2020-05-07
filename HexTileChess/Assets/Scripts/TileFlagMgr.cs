using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Collection;

public class TileFlagMgr
{    
    Material _selectedFlagMat,  // 当前Client所在tile对象的flag材质
        _reachableFlagMat,      // 可达tile的flag材质
        _attackableFlagMat,     // 可攻击tile的flag材质
        _attackPointFlagMat;    // 攻击点tile的flag材质

    GameObjectPool _flagPool;

    List<GameObject> _selected = new List<GameObject>();
    List<GameObject> _reachable = new List<GameObject>();
    List<GameObject> _attackable = new List<GameObject>();
    List<GameObject> _attackPoint = new List<GameObject>();

    public TileFlagMgr(GameObject tilePrefab, Material selectedFlagMat, Material reachableFlagMat,
        Material attackableFlagMat, Material attackPointFlagMat)
    {        
        _selectedFlagMat = selectedFlagMat;
        _reachableFlagMat = reachableFlagMat;
        _attackableFlagMat = attackableFlagMat;
        _attackPointFlagMat = attackPointFlagMat;

        _flagPool = new GameObjectPool(30, tilePrefab);
    }

    GameObject CreateFlag(Vector3 pos, Material mat)
    {
        GameObject flag = _flagPool.Get();
        flag.GetComponent<MeshRenderer>().material = mat;
        flag.transform.position = new Vector3(pos.x, flag.transform.position.y, pos.z);        
        flag.SetActive(true);
        flag.hideFlags = HideFlags.None;
        return flag;
    }

    public void CreateSelectedFlags(ref List<HexTileAgent> tiles)
    {
        foreach (var tile in tiles)
        {
            CreateSelectedFlag(tile.tile.Center);
        }
    }

    public void CreateReachableFlags(ref List<HexTileAgent> tiles)
    {
        foreach (var tile in tiles)
        {
            CreateReachableFlag(tile.tile.Center);
        }
    }

    public void CreateAttackableFlags(ref List<HexTileAgent> tiles)
    {
        foreach (var tile in tiles)
        {
            CreateAttackableFlag(tile.tile.Center);
        }
    }

    public void CreateAttackPointFlags(ref List<HexTileAgent> tiles)
    {
        foreach (var tile in tiles)
        {
            CreateAttackPointFlag(tile.tile.Center);
        }
    }

    public void CreateSelectedFlag(Vector3 pos)
    {
        _selected.Add(CreateFlag(pos, _selectedFlagMat));
    }

    void CreateReachableFlag(Vector3 pos)
    {
        _reachable.Add(CreateFlag(pos, _reachableFlagMat));
    }

    public void CreateAttackableFlag(Vector3 pos)
    {
        _attackable.Add(CreateFlag(pos, _attackableFlagMat));
    }

    void CreateAttackPointFlag(Vector3 pos)
    {
        _attackPoint.Add(CreateFlag(pos, _attackPointFlagMat));
    }

    void Hide(GameObject go)
    {
        go.SetActive(false);
        go.hideFlags = HideFlags.HideInHierarchy;
        _flagPool.Collect(go);
    }

    public void HideAll()
    {
        HideSelected();
        HideReachable();
        HideAttackable();
        HideAttackPoint();
    }

    public void HideSelected()
    {
        foreach (var item in _selected)
        {
            Hide(item);
        }
        _selected.Clear();
    }

    public void HideReachable()
    {
        foreach (var item in _reachable)
        {
            Hide(item);
        }
        _reachable.Clear();
    }

    public void HideAttackable()
    {
        foreach (var item in _attackable)
        {
            Hide(item);
        }
        _attackable.Clear();
    }

    public void HideAttackPoint()
    {
        foreach (var item in _attackPoint)
        {
            Hide(item);
        }
        _attackPoint.Clear();
    }
}