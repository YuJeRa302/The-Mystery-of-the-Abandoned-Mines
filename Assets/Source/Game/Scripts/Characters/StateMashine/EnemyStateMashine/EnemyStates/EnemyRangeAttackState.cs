using Assets.Source.Game.Scripts;
using UnityEngine;

public class EnemyRangeAttackState : EnemyAttackState
{
    private BulletSpawner _bulletSpawner;

    public EnemyRangeAttackState(StateMashine stateMashine, Player target, Enemy enemy, BulletSpawner bulletSpawner) : base(stateMashine, target, enemy)
    {
        Enemy = enemy;
        Target = target;
        AttackRange = Enemy.AttackDistance;
        Damage = Enemy.Damage;
        AttackDelay = Enemy.AttackDelay;
        AnimationController = Enemy.AnimationStateController;
        _bulletSpawner = bulletSpawner;
    }

    public override void SubscrabeIvent()
    {
        AnimationController.Attacked += LaunchBullet;
    }

    public override void UpdateState()
    {
        if (CanTransit)
        {
            DirectionToTarget = Enemy.transform.position - Target.transform.position;
            DistanceToTarget = DirectionToTarget.magnitude;

            if (DistanceToTarget > AttackRange)
                StateMashine.SetState<EnemyMoveState>();

            if (Attack())
                AttackEvent();
        }
    }

    protected override bool Attack()
    {
        if (DistanceToTarget <= AttackRange)
        {
            Enemy.transform.LookAt(Target.transform.position);

            if (LastAttackTime <= 0)
            {
                LastAttackTime = AttackDelay;
                CanTransit = false;
                return true;
            }
        }

        LastAttackTime -= Time.deltaTime;
        return false;
    }

    private void LaunchBullet()
    {
        _bulletSpawner.SpawnBullet();
        CanTransit = true;
    }
}