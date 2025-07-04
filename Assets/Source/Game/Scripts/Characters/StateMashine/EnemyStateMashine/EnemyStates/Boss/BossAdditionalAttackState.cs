using Assets.Source.Game.Scripts;
using UnityEngine;

public class BossAdditionalAttackState : State
{
    protected Player Target;
    protected Enemy Enemy;
    protected EnemyAnimation AnimationController;
    protected Vector3 DirectionToTarget;
    protected float DistanceToTarget;

    public BossAdditionalAttackState(StateMashine stateMashine, Player target, Enemy enemy) : base(stateMashine)
    {
        Target = target;
        Enemy = enemy;
    }

    public override void EnterState()
    {
        base.EnterState();
        CanTransit = false;
    }

    public override void UpdateState()
    {
        if (CanTransit)
            StateMashine.SetState<EnemyMoveState>();
    }

    protected virtual void AditionalAttackAppalyDamage()
    {
    }
}