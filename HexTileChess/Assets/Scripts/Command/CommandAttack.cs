using UnityEngine;
using Phenix.Unity.TurnBased;

public enum AttackStatus
{
    PREPARE = 0,        // 预备（可在此做招式的UI表现）
    ATTACKING,          // 攻击
    HIT,                // 伤害结算
    FINALIZE,           // 收尾
}

// 每种技能的特效显示放在具体动画的smb类里

public class CommandAttack : CommandBase
{
    public Hero hero { get; private set; }
    public Vector3 dstPos { get; private set; }
    public int skillID { get; private set; }
    public SkillTemplate skillTP { get; private set; }
    public bool isCounter { get; private set; } // 是否是反击
    public AttackStatus Status { get; private set; }

    float _timer;    
    Animator _animator;

    public CommandAttack(Hero hero, Vector3 dstPos, int skillID, bool isCounter)
    {
        this.hero = hero;
        this.dstPos = dstPos;
        this.skillID = skillID;
        this.isCounter = isCounter;
        _timer = Time.timeSinceLevelLoad;
        Status = AttackStatus.PREPARE;
        _animator = hero.GetComponent<Animator>();
        skillTP = StageMgr.Instance.skillAsset.GetData(skillID);
    }    

    public override void OnStart()
    {
        if (skillTP.animPrepareTime > 0)
        {
            _timer += skillTP.animPrepareTime;
        }
    }

    public override void OnUpdate()
    {
        if (Status == AttackStatus.PREPARE && Time.timeSinceLevelLoad >= _timer)
        {            
            Status = AttackStatus.ATTACKING;
            // 触发进攻动画，注意要确保Attack同时开始（SetTrigger不代表一定会立刻播放），
            // 否则会出现逻辑和画面不一致
            if (hero.CurSkill.skillID == hero.tp.normalSkill)
            {
                _animator.SetTrigger("Attack");
            }
            else if (hero.CurSkill.skillID == hero.tp.furySkill)
            {
                _animator.SetTrigger("HeavyAttack");
            }

            if (skillTP.animEffectStartTime > 0)
            {
                _timer = Time.timeSinceLevelLoad + skillTP.animEffectStartTime;                
            }
        }
        else if (Status == AttackStatus.ATTACKING && Time.timeSinceLevelLoad >= _timer)
        {
            Status = AttackStatus.HIT;
            // 伤害结算
            hero.CalcAttackResult(dstPos, skillID, isCounter);
            float timeToEnd = skillTP.animEffectKeepTime + skillTP.animFinalizeTime;
            if (timeToEnd > 0)
            {
                _timer = Time.timeSinceLevelLoad + timeToEnd;
            }
        }
        else if (Status == AttackStatus.HIT && Time.timeSinceLevelLoad >= _timer)
        {
            // 动画完成
            Status = AttackStatus.FINALIZE;
            Finished = true;
        }  
    }

    public override void OnEnd()
    {

    }
}
