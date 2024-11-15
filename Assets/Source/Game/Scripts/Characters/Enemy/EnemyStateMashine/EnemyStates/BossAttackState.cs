using UnityEngine;

public class BossAttackState : AttackState
{
    private float _additionalAttackDelay = 7f;
    protected float _lastAdditionalAttackTime = 0;

    public BossAttackState(StateMashine stateMashine, Player target, Enemy enemy, float attackDistance, float attackDelay, float damage, 
        float additionalAttackDelay, AnimationStateController animationController) : base(stateMashine, target, enemy, attackDistance, attackDelay, damage, animationController)
    {
        _target = target;
        _attackRange = attackDistance;
        _enemy = enemy;
        _attackDelay = attackDelay;
        _additionalAttackDelay = additionalAttackDelay;
        _animationController = animationController;
        _animationController.Attacked += ApplyDamage;
        _animationController.AdditionalAttacked += AditionalAttackAppalyDamage;
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

            if (AdditionalAttack())
            {
                AdditionalAttackEvent();
            }
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

    private void AditionalAttackAppalyDamage()
    {
        _canTransit = true;
        Debug.Log("TryAplayDamage");
    }
}