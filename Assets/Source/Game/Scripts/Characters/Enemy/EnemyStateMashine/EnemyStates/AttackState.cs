using UnityEngine;

public class AttackState : State
{
    protected float _lastAttackTime = 0;
    protected float _attackDelay;
    protected float _attackRange;
    protected Vector3 _directionToTarget;
    protected float _distanceToTarget;
    protected Player _target;
    protected Enemy _enemy;
    private Player target;
    private Enemy enemy;
    private float attackDistance;

    public AttackState(StateMashine stateMashine, Player target, Enemy enemy,float attackDistance, float attackDelay) : base(stateMashine)
    {
        _target = target;
        _attackRange = attackDistance;
        _enemy = enemy;
        _attackDelay = attackDelay;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        _directionToTarget = _enemy.transform.position - _target.transform.position;
        _distanceToTarget = _directionToTarget.magnitude;
        _enemy.transform.LookAt(_target.transform.position);

        if (_distanceToTarget > _attackRange)
            _stateMashine.SetState<MoveState>();

        if (Attack())
        {
            AttackEvent();
        }
    }

    protected bool Attack()
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