using System.Collections.Generic;
using UnityEngine;
using Phenix.Unity.TurnBased;
using System.Collections;

public enum ManualThinkStep
{
    NONE = 0,
    MOVE_OR_ATTACK,     // 决定移动还是攻击
    WHERE_TO_ATTACK,    // 决定攻击点
}

public class Hero : TurnBasedClient//<HeroProp>
{   
    public HexTileAgent tileComp;   // 所在tile    
    public CombatSide side;         // 对战方标识

    public HeroTemplate tp;         // 模板数据

    public Animator Animator { get; private set; }

    [SerializeField]
    int _hp = 100;                  // 生命值
    [SerializeField]
    int _fury = 0;                  // 怒气值    
    [SerializeField]
    int _exp = 0;                   // 经验
    [SerializeField]
    int _level = 1;                 // 等级
    //int _remainExecuteCount = 1;    // 本回合剩余决策次数

    SkillTemplate _normalSkillTP, _furySkillTP, _counterSkillTP;

    public CommandAttack counterAttack;

    AIMgr _aiMgr;
    public Transform fxPoint;  // 武器特效部位
    public Transform hitPoint; // 受击部位

    public void Init(HeroTemplate tp, CombatSide side)
    {
        this.side = side;
        this.tp = tp;
        _normalSkillTP = StageMgr.Instance.skillAsset.GetData(tp.normalSkill);
        _furySkillTP = StageMgr.Instance.skillAsset.GetData(tp.furySkill);
        _counterSkillTP = StageMgr.Instance.skillAsset.GetData(tp.counterSkill);
    }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _aiMgr = new AIMgr(this);
    }

    public void AddExp(int val)
    {
        _exp += val;
        if (_exp < 0)
        {
            _exp = 0;
        }
    }

    void GetEffectHeros(HexTileAgent dstTile, SkillTemplate skillTP, ref List<Hero> effectHeros)
    {
        effectHeros.Clear();

        if (dstTile.hero)
        {
            effectHeros.Add(dstTile.hero);
        }

        // 群体伤害
        if (skillTP.effectRange > 0)
        {            
            List<HexTileAgent> tiles = new List<HexTileAgent>();
            HexGridAgent.Instance.TilesInRange(dstTile, skillTP.effectRange, ref tiles);
            foreach (var tile in tiles)
            {
                if (tile.hero && tile.hero.side != side)
                {
                    effectHeros.Add(tile.hero);
                }
            }
        }
    }

    // 计算技能效果
    public void CalcAttackResult(Vector3 dstPos, int skillID, bool isCounter/*当前是否是反击*/)
    {        
        HexTileAgent dstTile = HexGridAgent.Instance.TileCompFromPosition(dstPos);
        SkillTemplate skillTP = StageMgr.Instance.skillAsset.GetData(skillID);
        List<Hero> effectHeros = new List<Hero>();
        // 获得效果作用对象
        GetEffectHeros(dstTile, skillTP, ref effectHeros);
        foreach (var effectHero in effectHeros)
        {
            effectHero.OnHit(this, skillTP);
        }
    }

    public void OnHit(Hero fromHero, SkillTemplate skillTP)
    {
        // 此处可以扩展考虑免伤buff，攻守双方的attack/defend等等要素
        _hp = Mathf.Max(0, (int)(_hp - skillTP.effectVal));
        if (_hp == 0)
        {
            OnDead(fromHero, skillTP);
        }
        else
        {
            // 触发Hit动画
            Animator.SetTrigger("Hit");
        }
    }

    void OnDead(Hero fromHero, SkillTemplate skillTP)
    {
        // 触发Dead动画
        Animator.SetTrigger("Dead");
        // 延时隐藏
        Invoke("Hide", 5);

        IsQuit = true;
        tileComp.hero = null;
        tileComp = null;
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    protected override void ExecuteAI(int turnID)
    {
        Debug.Log(string.Format("第{0}回合：{1}方的{2}执行（AI）", turnID, side, name));
        _aiMgr.Execute();
    }

    protected override void ExecuteManual(int turnID)
    {
        Debug.Log(string.Format("第{0}回合：{1}方的{2}执行（手动）", turnID, side, name));
        StartCoroutine(DoManualThink());
    }

    IEnumerator DoManualThink()
    {
        /*if (hero有被动技能在回合开始前使用) // 对于isAI == true的，直接AddCommand(preAttack)
        {
            CommandAttack preAttack = new CommandAttack(...);
            preAttack.OnStart();
            while (preAttack.Finished == false)
            {
                preAttack.OnUpdate();
                yield return new WaitForEndOfFrame();
            }
            preAttack.OnEnd();
        }*/

        ManualThinkStep _step = ManualThinkStep.MOVE_OR_ATTACK;
        ManualThinkStep _preStep = ManualThinkStep.NONE;                
        HexGridAgent.Instance.ShowSelected();
        HexTileAgent attackTarget = null;
        while (CurCommand == null)
        {
            // 获得点击的tile
            HexTileAgent tileOnPointer = HexGridAgent.Instance.TileAgentOnPointer;
            switch (_step)
            {
                case ManualThinkStep.MOVE_OR_ATTACK:
                    if (_step != _preStep)
                    {
                        HexGridAgent.Instance.ShowAllAttackPoint(attackTarget, true);
                        HexGridAgent.Instance.ShowAllReachable();
                        HexGridAgent.Instance.ShowAllAttackable();
                        _preStep = _step;
                    }

                    // 如果pointer点击攻击对象
                    if (HexGridAgent.Instance.InAttackableList(tileOnPointer))
                    {   
                        if (CurSkill.useRange == 1)
                        {
                            // 如果是近战技能，进一步选择攻击位置                     
                            _step = ManualThinkStep.WHERE_TO_ATTACK;
                            attackTarget = tileOnPointer;
                            yield return new WaitForEndOfFrame();
                        }
                        else
                        {
                            // 直接添加攻击命令                 
                            if (tileOnPointer.hero)
                            {
                                AddCommand(new CommandAttack(this, tileOnPointer.hero.hitPoint.position,
                                    CurSkill.skillID, false));
                            }
                            else
                            {
                                AddCommand(new CommandAttack(this, tileOnPointer.tile.Center,
                                    CurSkill.skillID, false));
                            }                            
                            break;
                        }
                    }                    
                    else if (HexGridAgent.Instance.InReachableList(tileOnPointer))// 如果点击了移动点
                    {                        
                        AddCommandMove(tileOnPointer);
                        break;
                    }
                    else
                    {
                        // 等待点击
                        yield return new WaitForEndOfFrame();
                    }

                    break;
                case ManualThinkStep.WHERE_TO_ATTACK:
                    if (_step != _preStep)
                    {
                        HexGridAgent.Instance.ShowAllReachable(true);
                        HexGridAgent.Instance.ShowAllAttackable(true);
                        HexGridAgent.Instance.ShowAttackTarget(attackTarget);
                        HexGridAgent.Instance.ShowAllAttackPoint(attackTarget);
                        _preStep = _step;
                    }

                    if (HexGridAgent.Instance.InAttackPointList(tileOnPointer))
                    {
                        // 如果点击了攻击位置，添加移动命令和进攻命令
                        AddCommandMove(tileOnPointer);                        
                        AddCommandAttack(attackTarget, false);
                        break;
                    }
                    else if (tileOnPointer != null)
                    {
                        // 如果点击无效位置，选择返回
                        _step = ManualThinkStep.MOVE_OR_ATTACK;
                        attackTarget = null;
                        yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        // 等待点击
                        yield return new WaitForEndOfFrame();
                    }

                    break;
                default:
                    break;
            }
        }

        HexGridAgent.Instance.ClearAllTileFlags();
    }

    // 切换是否AI控制时
    protected override void OnIsAIChanged()
    {
        if (IsAI == false)
        {
            // 从AI =》manual
            return;
        }

        // 从manual =》AI
        if (CombatMgr.Instance.CurClient == this && CurCommand == null)
        {
            HexGridAgent.Instance.ClearAllTileFlags();
            // 如果轮到当前hero，且尚未执行命令时，直接执行AI
            ExecuteAI(CombatMgr.Instance.TurnID);
        }
    }

    public void AddCommandIdle()
    {
        if (CombatMgr.Instance.CurClient != this)
        {
            return;
        }
        if (CurCommand != null)
        {
            return;
        }
        AddCommand(new CommandIdle());
    }

    public void AddCommandAttack(HexTileAgent attackTarget, bool isCounter)
    {
        if (attackTarget.hero)
        {
            AddCommand(new CommandAttack(this, attackTarget.hero.hitPoint.position, 
                CurSkill.skillID, isCounter));
        }
        else
        {
            AddCommand(new CommandAttack(this, attackTarget.tile.Center, CurSkill.skillID, isCounter));
        }
    }

    // 添加移动命令
    public void AddCommandMove(HexTileAgent tileOnPointer)
    {
        if (tp.canFly && HexGridAgent.Instance.Distance(this.tileComp, tileOnPointer) > 2)
        {
            AddCommand(new CommandFly(this, tileOnPointer.tile.Center));
        }
        else
        {
            // 寻路并添加march命令
            List<HexTileAgent> path = new List<HexTileAgent>();
            HexGridAgent.Instance.FindPath(tileComp, tileOnPointer, ref path);
            AddCommand(new CommandMarch(this, path));
        }
    }    

    public SkillTemplate CurSkill
    {
        get
        {
            if (_fury >= tp.maxFury && _furySkillTP != null)
            {
                return _furySkillTP;
            }
            else
            {
                return _normalSkillTP;
            }
        }
    }
}