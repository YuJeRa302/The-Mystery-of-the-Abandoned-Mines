using Assets.Source.Game.Scripts;
using UnityEngine;

public class BossAdditionalAttackState : State
{
    protected Player _target;
    protected Enemy _enemy;
    protected EnemyAnimation _animationController;
    protected Vector3 _directionToTarget;
    protected float _distanceToTarget;

    public BossAdditionalAttackState(StateMashine stateMashine, Player target, Enemy enemy) : base(stateMashine)
    {
        _target = target;
        _enemy = enemy;
    }

    public override void EnterState()
    {
        base.EnterState();
        _canTransit = false;
    }

    public override void UpdateState()
    {
        if (_canTransit)
            _stateMashine.SetState<EnemyMoveState>();
    }

    protected virtual void AditionalAttackAppalyDamage()
    {
    }
}