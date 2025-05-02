using Assets.Source.Game.Scripts;
using UnityEngine;

public class SummonAttackState : State
{
    private Summon _summon;
    private Enemy _target;
    protected float _lastAttackTime = 0;
    protected float _attackDelay;
    protected float _attackRange;
    protected Vector3 _directionToTarget;
    protected float _distanceToTarget;
    protected DamageSource _damage;
    protected SummonAnimation _animationController;

    public SummonAttackState(StateMashine stateMashine, Summon summon) : base(stateMashine)
    {
        _summon = summon;
        _attackDelay = _summon.AttackDelay;
        _attackRange = _summon.DistanceToTarget;
        _damage = _summon.DamageSource;
        _animationController = _summon.Animation;
        _animationController.Attacked += ApplyDamage;
    }

    public override void EnterState()
    {
        base.EnterState();
        _canTransit = true;

        if (_summon.Target != null)
        {
            _target = _summon.Target;
        }
        else
        {
            _stateMashine.SetState<SummonIdleState>();
        }
    }

    public override void UpdateState()
    {
        if (_canTransit)
        {
            if (_target == null || _target.isActiveAndEnabled == false)
            {
                _summon.DisableTarget();
                _stateMashine.SetState<SummonIdleState>();
            }

            _directionToTarget = _summon.transform.position - _target.transform.position;
            _distanceToTarget = _directionToTarget.magnitude;

            if (_distanceToTarget > _attackRange)
                _stateMashine.SetState<SummonIdleState>();

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
            _summon.transform.LookAt(_target.transform.position);

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
        if (_target != null && _target.isActiveAndEnabled == true)
        {
            Vector3 directionToTarget = _summon.transform.position - _target.transform.position;
            float distance = directionToTarget.magnitude;
            
            if (distance <= _attackRange)
                _target.TakeDamage(_damage);
        }

        _canTransit = true;
    }
}