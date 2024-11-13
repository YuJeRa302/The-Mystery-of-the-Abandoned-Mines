using UnityEngine;

public class BossAttackState : AttackState
{
    private float _additionalAttackDelay = 7f;
    protected float _lastAdditionalAttackTime = 0;

    public BossAttackState(StateMashine stateMashine, Player target, Enemy enemy, float attackDistance, float attackDelay, float additionalAttackDelay) : base(stateMashine, target, enemy, attackDistance, attackDelay)
    {
        _target = target;
        _attackRange = attackDistance;
        _enemy = enemy;
        _attackDelay = attackDelay;
        _additionalAttackDelay = additionalAttackDelay;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (AdditionalAttack())
        {
            AdditionalAttackEvent();
        }
    }

    private bool AdditionalAttack()
    {
        if (_distanceToTarget <= _attackRange)
        {
            if (_lastAdditionalAttackTime <= 0)
            {
                _lastAdditionalAttackTime = _additionalAttackDelay;
                return true;
            }
        }

        _lastAdditionalAttackTime -= Time.deltaTime;
        return false;
    }
}