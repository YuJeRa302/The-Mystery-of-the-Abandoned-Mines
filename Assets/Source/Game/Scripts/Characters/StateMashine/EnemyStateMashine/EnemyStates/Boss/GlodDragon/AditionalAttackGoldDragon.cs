using Assets.Source.Game.Scripts;
using UnityEngine;

public class AditionalAttackGoldDragon : BossAdditionalAttackState
{
    private float _attackRange;

    public AditionalAttackGoldDragon(StateMashine stateMashine, Player target, Enemy enemy) : base(stateMashine, target, enemy)
    {
        _target = target;
        _enemy = enemy;
        _animationController = _enemy.AnimationStateController;
        Boss boss = _enemy as Boss;
        _attackRange = boss.AdditionalAttackRange;

        _animationController.AdditionalAttacked += AditionalAttackAppalyDamage;
        _animationController.AnimationCompleted += OnAllowTransition;
    }

    public override void EnterState()
    {
        base.EnterState();
        AdditionalAttackEvent();
    }

    protected override void AditionalAttackAppalyDamage()
    {
        _directionToTarget = _enemy.transform.position - _target.transform.position;
        _distanceToTarget = _directionToTarget.magnitude;

        if (_distanceToTarget <= _attackRange)
        {
            Debug.Log("DamagePlayer");
        }
    }
}