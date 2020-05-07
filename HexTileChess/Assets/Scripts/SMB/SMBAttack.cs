using UnityEngine;
using Phenix.Unity.Anim.SMB;

public class SMBAttack : SMBBase
{
    Hero _hero;
    CommandAttack _commandAttack;    
    //Vector3 _oriDir;

    GameObject _projectile;     // 投掷对象
    float _projectileFlyTime;   // 投掷物飞行时长
    float _projectileProgress;  // 投掷物飞行进度
    float _projectileFlyHeight = 0.5f;// 投掷物抛物线高度
    Vector3 _projectileOriPos;  // 投掷物初始位置
    Vector3 _projectileTarPos;  // 投掷物目标位置
    Vector3 _projectilePrePos;  // 投掷物上一帧位置
    float _yOffset = 1;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
        _hero = animator.GetComponent<Hero>();
        _commandAttack = _hero.CurCommand as CommandAttack;
        // 记录原来朝向
        //_oriDir = _hero.transform.forward;
        // 转向目标
        _hero.transform.LookAt(new Vector3(_commandAttack.dstPos.x, _hero.transform.position.y, 
            _commandAttack.dstPos.z));

        // 处理投掷类attack
        if (_commandAttack.skillTP.projectile)
        {            
            _projectileOriPos = _hero.transform.position + new Vector3(0, _yOffset, 0);
            _projectileTarPos = _commandAttack.dstPos;
            _projectile = GameObject.Instantiate(_commandAttack.skillTP.projectile, 
                _hero.transform.position, _hero.transform.rotation, null);
            // 飞行时间由技能表animEffectStartTime决定
            _projectileFlyTime = _commandAttack.skillTP.animEffectStartTime;
            _projectilePrePos = _projectileOriPos;
            _projectileProgress = 0;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 处理投掷物的抛物线飞行过程
        if (_projectile && _projectileFlyTime > 0 && _projectileProgress < 1)
        {            
            float yOffset = _projectileFlyHeight * 4.0f * (_projectileProgress - 
                _projectileProgress * _projectileProgress);
            _projectile.transform.position = Vector3.Lerp(_projectileOriPos, _projectileTarPos, 
                _projectileProgress) + yOffset * Vector3.up;            
            _projectileProgress += Time.deltaTime / _projectileFlyTime;
            // 调整投掷物朝向
            _projectile.transform.forward = (_projectile.transform.position - _projectilePrePos).normalized;
            if (_projectileProgress > 1)
            {
                _projectileProgress = 1;
                // 到达目标点，删除投掷物
                Destroy(_projectile);             
            }
            else
            {
                _projectilePrePos = _projectile.transform.position;
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 恢复攻击前朝向
        //_hero.transform.forward = _oriDir;
    }
}
