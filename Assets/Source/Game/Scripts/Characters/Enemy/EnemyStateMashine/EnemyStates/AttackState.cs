using UnityEngine;

public class AttackState : State
{
    private float _lastAttackTime = 0;
    private float _attackDelay = 2f;
    private float _attackRange;
    private Vector3 _directionToTarget;
    private float _distanceToTarget;
    private Player _target;
    private Enemy _enemy;

    public AttackState(StateMashine stateMashine, Player target, Enemy enemy,float attackDistance) : base(stateMashine)
    {
        _target = target;
        _attackRange = attackDistance;
        _enemy = enemy;
    }

    public override void UpdateState()
    {
        _directionToTarget = _enemy.transform.position - _target.transform.position;
        _distanceToTarget = _directionToTarget.magnitude;

        if (_distanceToTarget > _attackRange)
            _stateMashine.SetState<MoveState>();

        if (Attack())
        {
            AttackEvent();
        }
    }

    private bool Attack()
    {
        if (_distanceToTarget <= _attackRange)
        {
            if (_lastAttackTime <= 0)
            {
                _lastAttackTime = _attackDelay;
                return true;
            }
        }

        _lastAttackTime -= Time.deltaTime;
        return false;
    }
}