using UnityEngine;
using Phenix.Unity.Anim.SMB;

public class SMBFly : SMBBase
{
    Hero _hero;
    CommandFly _commandFly;
    Vector3 _srcPos;
    Vector3 _dstPos;
    //Vector3 _oriDir;
    float _flyHeight = 2f;
    float _progress;
    float _moveTime;
    bool _landing = false;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        _hero = animator.GetComponent<Hero>();
        _commandFly = _hero.CurCommand as CommandFly;
        _srcPos = _hero.transform.position;
        _dstPos = _commandFly.dstPos;
        // 飞行时长 = 起止点距离 / hero的speed
        _moveTime = Vector3.Distance(_srcPos, _dstPos) / _hero.tp.speed;
        //_oriDir = _hero.transform.forward;
        _progress = 0;
        _landing = false;
        _hero.transform.LookAt(new Vector3(_dstPos.x, _hero.transform.position.y, _dstPos.z));
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Flying"))
        {
            DoFlying(animator);
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

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        //_hero.transform.forward = _oriDir; // 恢复原来朝向
        _commandFly.Finished = true;
    }
}
