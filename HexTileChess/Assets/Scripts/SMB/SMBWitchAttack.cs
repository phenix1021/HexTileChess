using UnityEngine;
using Phenix.Unity.Anim.SMB;

public class SMBWitchAttack : SMBBase
{
    Hero _hero, _target;
    CommandAttack _commandAttack;        
    GameObject _lighting;       // 闪电    
    GameObject _lightingImpact; // 闪电击中目标  
    bool _inProgress = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
        _hero = animator.GetComponent<Hero>();
        _commandAttack = _hero.CurCommand as CommandAttack;
        
        // 转向目标
        _hero.transform.LookAt(new Vector3(_commandAttack.dstPos.x, _hero.transform.position.y, 
            _commandAttack.dstPos.z));
        _lighting = GameObject.Instantiate(_commandAttack.skillTP.fx);
        _lighting.transform.SetParent(_hero.fxPoint);
        _lighting.transform.position = _hero.fxPoint.position;
        _lighting.transform.rotation = _hero.fxPoint.rotation;
        _inProgress = false;

        _target = HexGridAgent.Instance.TileCompFromPosition(_commandAttack.dstPos).hero;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_commandAttack.Status == AttackStatus.HIT && _inProgress == false)
        {
            _inProgress = true;
            // line
            LineRenderer lineRenderer = _lighting.GetComponent<LineRenderer>();            
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new Vector3[] { _hero.fxPoint.position, _commandAttack.dstPos });
            // impact
            if (_target)
            {
                _lightingImpact = GameObject.Instantiate(_commandAttack.skillTP.fxImpact);
                _lightingImpact.transform.SetParent(_target.transform);
                _lightingImpact.transform.position = _commandAttack.dstPos;
                _lightingImpact.transform.rotation = Quaternion.identity;
            }
            else
            {
                _lightingImpact = GameObject.Instantiate(_commandAttack.skillTP.fxImpact,
                    _commandAttack.dstPos, Quaternion.identity, null);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _inProgress = false;
        Destroy(_lighting);
        Destroy(_lightingImpact);
    }
}
