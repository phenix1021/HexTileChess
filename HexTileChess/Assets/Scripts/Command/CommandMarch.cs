using UnityEngine;
using System.Collections.Generic;

public class CommandMarch : CommandBase
{
    Hero _hero;
    public List<Vector3> path;

    public CommandMarch(Hero hero, List<Vector3> path)
    {
        _hero = hero;
        this.path = path;
    }

    public CommandMarch(Hero hero, List<HexTileAgent> path)
    {
        List<Vector3> vPath = new List<Vector3>();
        foreach (var item in path)
        {
            vPath.Add(item.tile.Center);
        }        
        _hero = hero;
        this.path = vPath;
    }
    
    public override void OnStart()
    {
        _hero.GetComponent<Animator>().SetBool("Marching", true);
        //HexGridAgent.Instance.ClearSelectedClientFlag();
        _hero.tileComp.hero = null;
        _hero.tileComp = null;
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnEnd()
    {
        _hero.tileComp = HexGridAgent.Instance.TileCompFromPosition(path[path.Count-1]);
        _hero.tileComp.hero = _hero;     
    }
}
