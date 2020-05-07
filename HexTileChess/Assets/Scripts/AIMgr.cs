using UnityEngine;
using Phenix.Unity.Extend;
using System.Collections.Generic;

public class AIMgr
{
    Hero _hero;
    Hero _target;

    public AIMgr(Hero hero)
    {
        _hero = hero;
    }

    public void Execute()
    {
        int dist = 0;
        if (_target == null)
        {            
            _target = GetNearestTarget(out dist);
            if (_target == null)
            {
                return;
            }
        }

        dist = HexGridAgent.Instance.Distance(_hero.tileComp, _target.tileComp);
        if (_hero.CurSkill.useRange >= dist)
        {
            // 目标在攻击范围内，直接攻击
            _hero.AddCommandAttack(_target.tileComp, false);
            return;
        }

        // 目标在攻击范围外，朝目标随机移动
        List<HexTileAgent> tiles = new List<HexTileAgent>();
        HexGridAgent.Instance.ReachableTilesInRange(_hero.tileComp, _hero.tp.move, ref tiles);
        tiles.Shuffle();
        foreach (var tile in tiles)
        {
            int tmpDist = HexGridAgent.Instance.Distance(tile, _target.tileComp);
            if (tmpDist < dist)
            {
                _hero.AddCommandMove(tile);
                return;
            }
        }

        _hero.AddCommandIdle();
    } 

    Hero GetNearestTarget(out int dist)
    {
        Hero ret = null;
        int minDist = int.MaxValue;
        foreach (var client in CombatMgr.Instance.Clients)
        {
            if (client.side == _hero.side)
            {
                continue;
            }

            if (client.IsQuit)
            {
                continue;
            }

            dist = HexGridAgent.Instance.Distance(client.tileComp, _hero.tileComp);
            if (dist < minDist)
            {
                minDist = dist;
                ret = client;
            }
        }

        dist = minDist;
        return ret;
    }
}
