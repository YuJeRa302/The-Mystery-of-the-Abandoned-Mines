using UnityEngine;

public class AttackState : State
{
    protected float _lastAttackTime = 0;
    protected float _attackDelay;
    protected float _attackRange;
    protected Vector3 _directionToTarget;
    protected float _distanceToTarget;
    protected float _damage;
    protected Player _target;
    protected Enemy _enemy;
    protected AnimationStateController _animationController;

    protected bool _canTransit = true;

    public AttackState(StateMashine stateMashine, Player target, Enemy enemy,float attackDistance, float attackDelay, float damage,AnimationStateController animationController) : base(stateMashine)
    {
        _target = target;
        _attackRange = attackDistance;
        _damage = damage;
        _enemy = enemy;
        _attackDelay = attackDelay;
        _animationController = animationController;
        _animationController.Attacked += ApplyDamage;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        if (_canTransit)
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
    }

    protected bool Attack()
    {
        if (_distanceToTarget <= _attackRange)
        {
            _enemy.transform.LookAt(_target.transform.position);

            if (_lastAttackTime <= 0)
            {
                _lastAttackTime = _attackDelay;
                _canTransit = false;
                return true;
            }
        }

        _lastAttackTime -= Time.deltaTime;
        return false;
    }

    protected void ApplyDamage()
    {
        _canTransit = true;
    }
}