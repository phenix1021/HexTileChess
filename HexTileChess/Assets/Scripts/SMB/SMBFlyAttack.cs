using UnityEngine;
using Phenix.Unity.Anim.SMB;

public class SMBFlyAttack : SMBBase
{
    enum AttackStep
    {
        NONE = 0,
        FLY,        // 朝目标飞去
        FIRE,       // 攻击
        FLY_BACK,   // 返回
    }

    Hero _hero, _target;
    CommandAttack _commandAttack;
    Vector3 _oriPos, _srcPos;
    Vector3 _dstPos;
    
    float _flyHeight = 2f;
    float _progress;
    float _moveTime;
    float _flyBackTimer = 0;
    bool _landing = false;

    AttackStep _attackStep = AttackStep.NONE;

    GameObject _fire;       // 火焰    
    GameObject _fireImpact; // 火焰击中目标  

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        _hero = animator.GetComponent<Hero>();
        _commandAttack = _hero.CurCommand as CommandAttack;
        _oriPos = _srcPos = _hero.transform.position;
        _dstPos = _commandAttack.dstPos;
        _moveTime = _commandAttack.skillTP.animEffectStartTime * 2f;
        _progress = 0;
        _flyHeight = 2f;
        _flyBackTimer = 0;
        _landing = false;
        _attackStep = AttackStep.FLY;
        _hero.transform.LookAt(new Vector3(_dstPos.x, _hero.transform.position.y, _dstPos.z));

        _target = HexGridAgent.Instance.TileCompFromPosition(_commandAttack.dstPos).hero;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_attackStep == AttackStep.FLY)
        {
            DoFlying(animator);
            if (_commandAttack.Status == AttackStatus.HIT)
            {
                // 触发fire
                Fire(animator);
                // 设置返回时刻
                _flyBackTimer = Time.timeSinceLevelLoad + _commandAttack.skillTP.animEffectKeepTime;
            }
        }    
        else if (_attackStep == AttackStep.FIRE)
        {
            if (Time.timeSinceLevelLoad >= _flyBackTimer)
            {
                Destroy(_fire);
                Destroy(_fireImpact);

                _attackStep = AttackStep.FLY_BACK;
                _moveTime = _commandAttack.skillTP.animFinalizeTime;
                _srcPos = _hero.transform.position;
                _dstPos = _oriPos;
                _progress = 0;
                _flyHeight = 0.5f;
                _hero.transform.LookAt(new Vector3(_oriPos.x, _hero.transform.position.y, _oriPos.z));
                animator.SetTrigger("Fly");
            }
            else
            {
                if (_target)
                {
                    _fire.transform.LookAt(_target.hitPoint);
                }
                else
                {
                    _fire.transform.LookAt(_dstPos);
                }                
            }
        }
        else if (_attackStep == AttackStep.FLY_BACK)
        {
            DoFlying(animator);
        }
    }

    void Fire(Animator animator)
    {
        animator.SetTrigger("Fire");
        _attackStep = AttackStep.FIRE;
        _fire = GameObject.Instantiate(_commandAttack.skillTP.fx);
        _fire.transform.SetParent(_hero.fxPoint);
        _fire.transform.position = _hero.fxPoint.position;        

        // impact
        if (_target)
        {
            _fireImpact = GameObject.Instantiate(_commandAttack.skillTP.fxImpact);
            _fireImpact.transform.SetParent(_target.transform);
            _fireImpact.transform.position = _commandAttack.dstPos;
            _fireImpact.transform.rotation = Quaternion.identity;
        }
        else
        {
            _fireImpact = GameObject.Instantiate(_commandAttack.skillTP.fxImpact,
                _commandAttack.dstPos, Quaternion.identity, null);
        }
    }

    void DoFlying(Animator animator)
    {
        if (_progress == 1)
        {
            return;
        }
        float yOffset = _flyHeight * 4.0f * (_progress - _progress * _progress);
        _hero.transform.position = Vector3.Lerp(_srcPos, _dstPos, _progress) + 
            yOffset * Vector3.up;        
        _progress += Time.deltaTime / _moveTime;

        if (_progress > 0.85f && _landing == false)
        {
            _landing = true;
            animator.SetTrigger("Land");
        }

        if (_progress > 1)
        {
            _progress = 1;                                       
        }        
    }    
}
