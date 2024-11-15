using UnityEngine;

public class RangeAttackState : AttackState
{
    private BulletSpawner _bulletSpawner;

    public RangeAttackState(StateMashine stateMashine, Player target, Enemy enemy, float attackDistance, float attackDelay, float damage, 
        AnimationStateController animationController, BulletSpawner bulletSpawner) : base(stateMashine, target, enemy, attackDistance, attackDelay, damage, animationController)
    {
        _target = target;
        _attackRange = attackDistance;
        _enemy = enemy;
        _attackDelay = attackDelay;
        _animationController = animationController;
        _bulletSpawner = bulletSpawner;
        _animationController.Attacked += LaunchBullet;
        Debug.Log("addStae");
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    private void LaunchBullet()
    {
        _bulletSpawner.SpawnBullet();
    }
}