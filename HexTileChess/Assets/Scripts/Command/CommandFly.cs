using UnityEngine;

public class CommandFly : CommandBase
{
    Hero _hero;
    public Vector3 dstPos;

    public CommandFly(Hero hero, Vector3 dstPos)
    {
        _hero = hero;
        this.dstPos = dstPos;
    }
        
    public override void OnStart()
    {
        _hero.GetComponent<Animator>().SetTrigger("Fly");
        //HexGridAgent.Instance.ClearSelectedClientFlag();
        _hero.tileComp.hero = null;
        _hero.tileComp = null;        
    }

    public  override void OnUpdate()
    {
        
    }

    public override void OnEnd()
    {
        _hero.tileComp = HexGridAgent.Instance.TileCompFromPosition(dstPos);
        _hero.tileComp.hero = _hero;        
    }
}
