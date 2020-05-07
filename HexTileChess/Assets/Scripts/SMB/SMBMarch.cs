using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Anim.SMB;

public class SMBMarch : SMBBase
{
    List<Vector3> _path;
    Hero _hero;
    CommandMarch _commandMarch;
    int _arrivedIdx = -1;
    //Vector3 _oriDir;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
        _hero = animator.GetComponent<Hero>();
        _commandMarch = _hero.CurCommand as CommandMarch;
        _path = _commandMarch.path;
        //_oriDir = _hero.transform.forward;
        _arrivedIdx = -1;
        if (_path.Count > 0)
        {
            Vector3 next = _path[0];
            _hero.transform.LookAt(new Vector3(next.x, _hero.transform.position.y, next.z));
        }        
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_arrivedIdx < _path.Count - 1)
        {
            _hero.transform.position = Vector3.MoveTowards(_hero.transform.position, 
                _path[_arrivedIdx+1], _hero.tp.speed * Time.deltaTime);
            if (_hero.transform.position == _path[_arrivedIdx + 1])
            {                
                // 到达路径点
                if (++_arrivedIdx == _path.Count - 1)
                {
                    // 到达终点
                    //_hero.transform.forward = _oriDir;
                    _commandMarch.Finished = true;                    
                    animator.SetBool("Marching", false);                    
                    return;
                }
                Vector3 next = _path[_arrivedIdx + 1];
                _hero.transform.LookAt(new Vector3(next.x, _hero.transform.position.y, next.z));                
            }
        }
    }
}
